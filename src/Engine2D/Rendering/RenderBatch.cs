#region

using Engine2D.Cameras;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Managers;
using Engine2D.Utilities;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using TextureUnit = OpenTK.Graphics.OpenGL4.TextureUnit;

#endregion

namespace Engine2D.Rendering;

internal class RenderBatch : IComparable<RenderBatch>
{
    private const int c_maxBatchSize = 20000;

    private const int c_posSize = 2;
    private const int c_colorSize = 4;
    private const int c_texCoordSize = 2;
    private const int c_texIdSize = 1;

    private const int c_posOffset = 0;
    private const int c_colorOffset = c_posOffset + c_posSize * sizeof(float);
    private const int c_texCoordOffset = c_colorOffset + c_colorSize * sizeof(float);
    private const int c_texIdOffset = c_texCoordOffset + c_texCoordSize * sizeof(float);

    private const int c_vertexSize = 9;
    private const int c_vertexSizeInBytes = c_vertexSize * sizeof(float);
    private readonly Shader? _shader;

    private SpriteRenderer[] _sprites = new SpriteRenderer[c_maxBatchSize];
    
    private Dictionary<SpriteRenderer, Texture> _spriteTextureMap = new();

    public readonly Texture?[] Textures = new Texture[(int)ShaderDefaultSlots.AVAILABLETEXTUREUNITS];
    private readonly int[] _textureUnits = new int[(int)ShaderDefaultSlots.AVAILABLETEXTUREUNITS];
    private readonly float[] _vertices = new float[c_maxBatchSize * c_vertexSize];

    // 0.5f,   0.5f, 0.0f,    1.0f, 1.0f,   // top right
    // 0.5f,  -0.5f, 0.0f,    1.0f, 0.0f,   // bottom right
    // -0.5f, -0.5f, 0.0f,    0.0f, 0.0f,   // bottom left
    // -0.5f,  0.5f, 0.0f,    0.0f, 1.0f    // top left 

    private readonly Vector4[] quadVertexPositions =
    {
        new(0.5f, 0.5f, 0.0f, 1.0f),
        new(0.5f, -0.5f, 0.0f, 1.0f),
        new(-0.5f, -0.5f, 0.0f, 1.0f),
        new(-0.5f, 0.5f, 0.0f, 1.0f)
    };

    private int _vaoId, _vboId;
    internal int ZIndex;

    internal RenderBatch(int zIndex)
    {
        ZIndex = zIndex;

        for (var i = 0; i < _textureUnits.Length; i++) _textureUnits[i] = i;

        var dat = new ShaderData();
        dat.VertexPath = Utils.GetBaseEngineDir() + "/Shaders/default.vert";
        dat.FragPath = Utils.GetBaseEngineDir() + "/Shaders/default.frag";

        _shader = ResourceManager.GetShader(dat);
        _sprites = new SpriteRenderer[c_maxBatchSize];

        _vertices = new float[c_maxBatchSize * c_vertexSize * 4];
    }

    internal bool HasRoom => _spriteCount < c_maxBatchSize;
    private int _spriteCount { get; set; }


    public int CompareTo(RenderBatch? other)
    {
        if (ZIndex < other.ZIndex)
            return -1;

        if (ZIndex == other.ZIndex)
            return 0;

        return 1;
    }

    internal void Init()
    {
        _vaoId = GL.GenVertexArray();
        GL.BindVertexArray(_vaoId);

        _vboId = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboId);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
            BufferUsageHint.DynamicDraw);

        var eboID = GL.GenBuffer();
        var indices = GenerateIndices();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboID);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices,
            BufferUsageHint.StaticDraw);

        // Enable the buffer attribute pointers
        GL.VertexAttribPointer(0, c_posSize, VertexAttribPointerType.Float, false, c_vertexSizeInBytes, c_posOffset);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, c_colorSize, VertexAttribPointerType.Float, false, c_vertexSizeInBytes,
            c_colorOffset);
        GL.EnableVertexAttribArray(1);

        GL.VertexAttribPointer(2, c_texCoordSize, VertexAttribPointerType.Float, false, c_vertexSizeInBytes,
            c_texCoordOffset);
        GL.EnableVertexAttribArray(2);

        GL.VertexAttribPointer(3, c_texIdSize, VertexAttribPointerType.Float, false, c_vertexSizeInBytes,
            c_texIdOffset);
        GL.EnableVertexAttribArray(3);
    }

    /// <summary>
    /// TODO: Make this more efficient
    /// Hacky workaround for texture buffer getting overflown but its good enough for now
    /// </summary>
    /// <param name="spr"></param>
    /// <returns></returns>
    internal bool HasTexture(SpriteRenderer spr)
    {
        if (spr.Sprite?.Texture != null)
        {
            if (!Textures.Contains(spr.Sprite.Texture))
            {
                for (var i = 0; i < Textures.Length-1; i++)
                {
                    if (Textures[i] == null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }

        return false;
    }
    

    internal void AddSprite(SpriteRenderer spr)
    {
        var index = _spriteCount;
        _sprites[index] = spr;
        _spriteCount++;

            if (spr.Sprite?.Texture != null)
                if (!Textures.Contains(spr.Sprite.Texture))
                    for (var i = 0; i < Textures.Length; i++)
                        if (Textures[i] == null)
                        {
                            Textures[i] = spr.Sprite.Texture;
                            break;
                        }
        
        
        LoadVertexProperties(index);
    }

    internal void Render(Camera camera, int lightmapTexture, Renderer renderer)
    {
        if(this._spriteCount <= 0)
        {
            renderer.RenderBatchesToRemoveEndOfFrame.Add(this);
            return;
        }
        
        var projectionMatrix = camera.GetProjectionMatrix();
        var viewMatrix = camera.GetViewMatrix();

        // For now, we will rebuffer all data every frame
        var rebufferData = false;

        for (var i = 0; i < _spriteCount; i++)
            if (_sprites[i].IsDirty)
            {
                _sprites[i].IsDirty = false;
                LoadVertexProperties(i);
                rebufferData = true;
            }

        if (rebufferData)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboId);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, _vertices.Length * sizeof(float), _vertices);
        }

        _shader.use();
        _shader.uploadMat4f("uProjection", projectionMatrix);
        _shader.uploadMat4f("uView", viewMatrix);
        Texture.Use(TextureUnit.Texture0 + (int)ShaderDefaultSlots.LIGHTMAPTEXTURESLOT, lightmapTexture);
        _shader.uploadInt("uLightmap", (int)ShaderDefaultSlots.LIGHTMAPTEXTURESLOT);

        //TEXTURES
        for (var i = 0; i < Textures.Length; i++)
            if (Textures[i] != null)
            {
                GL.ActiveTexture(OpenTK.Graphics.OpenGL.TextureUnit.Texture0 + i + 1);
                Textures[i].bind();
            }

        _shader.UploadIntArray("uTextures", _textureUnits);

        GL.BindVertexArray(_vaoId);
        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);

        GL.DrawElements(PrimitiveType.Triangles, _spriteCount * 6, DrawElementsType.UnsignedInt, 0);

        for (var i = 0; i < Textures.Length; i++)
            if (Textures[i] != null)
                Textures[i].unbind();

        GL.DisableVertexAttribArray(0);
        GL.DisableVertexAttribArray(1);
        GL.BindVertexArray(0);

        _shader.detach();
    }

    private void LoadVertexProperties(int index)
    {
        var spriteRenderer = _sprites[index];
        var offset = index * 4 * c_vertexSize;

        Vector4 color = new(spriteRenderer.Color.X/255, spriteRenderer.Color.Y/255,
            spriteRenderer.Color.Z/255, spriteRenderer.Color.W/255);

        var texID = -1;
        var texCoords = spriteRenderer.TextureCoords;


        if (spriteRenderer.Sprite?.Texture != null)
        {
            for (var i = 0; i < Textures.Length; i++)
                if (Textures[i] != null)
                {
                    if (Textures[i].Equals(spriteRenderer.Sprite.Texture))
                    {
                        texID = i + 1;
                        break;
                    }
                }
        }

        var translation = spriteRenderer.Parent.GetComponent<Transform>().GetTranslation();

        if (spriteRenderer.Sprite != null)
        {
            translation = spriteRenderer.Parent.GetComponent<Transform>().GetTranslation();
        }


        {
            var currentPos =
                MathUtils.Multiply(translation, quadVertexPositions[0]); //quadVertexPositions[0] * translation;
            _vertices[offset + 0] = currentPos.X;
            _vertices[offset + 1] = currentPos.Y;

            // Load color
            _vertices[offset + 2] = color.X;
            _vertices[offset + 3] = color.Y;
            _vertices[offset + 4] = color.Z;
            _vertices[offset + 5] = color.W;

            //Load tex coords
            _vertices[offset + 6] = texCoords[0].X;
            _vertices[offset + 7] = texCoords[0].Y;

            //Load tex id
            _vertices[offset + 8] = texID;
            offset += c_vertexSize;
        }

        {
            var currentPos = MathUtils.Multiply(translation, quadVertexPositions[1]);
            _vertices[offset + 0] = currentPos.X;
            _vertices[offset + 1] = currentPos.Y;

            // Load color
            _vertices[offset + 2] = color.X;
            _vertices[offset + 3] = color.Y;
            _vertices[offset + 4] = color.Z;
            _vertices[offset + 5] = color.W;

            //Load tex coords
            _vertices[offset + 6] = texCoords[1].X;
            _vertices[offset + 7] = texCoords[1].Y;

            //Load tex id
            _vertices[offset + 8] = texID;

            offset += c_vertexSize;
        }

        {
            var currentPos = MathUtils.Multiply(translation, quadVertexPositions[2]);
            _vertices[offset + 0] = currentPos.X;
            _vertices[offset + 1] = currentPos.Y;

            // Load color
            _vertices[offset + 2] = color.X;
            _vertices[offset + 3] = color.Y;
            _vertices[offset + 4] = color.Z;
            _vertices[offset + 5] = color.W;

            //Load tex coords
            _vertices[offset + 6] = texCoords[2].X;
            _vertices[offset + 7] = texCoords[2].Y;

            //Load tex id
            _vertices[offset + 8] = texID;

            offset += c_vertexSize;
        }


        {
            var currentPos = MathUtils.Multiply(translation, quadVertexPositions[3]);
            _vertices[offset + 0] = currentPos.X;
            _vertices[offset + 1] = currentPos.Y;

            // Load color
            _vertices[offset + 2] = color.X;
            _vertices[offset + 3] = color.Y;
            _vertices[offset + 4] = color.Z;
            _vertices[offset + 5] = color.W;

            //Load tex coords
            _vertices[offset + 6] = texCoords[3].X;
            _vertices[offset + 7] = texCoords[3].Y;

            //Load tex id
            _vertices[offset + 8] = texID;
        }
    }

    private int[] GenerateIndices()
    {
        // 6 indices per quad (3 per triangle)
        var elements = new int[6 * c_maxBatchSize];

        for (var i = 0; i < c_maxBatchSize; i++) loadElementIndices(elements, i);

        return elements;
    }

    private void loadElementIndices(int[] elements, int index)
    {
        var offsetArrayIndex = 6 * index;
        var offset = 4 * index;

        // 3, 2, 0, 0, 2, 1        7, 6, 4, 4, 6, 5
        // Triangle 1
        elements[offsetArrayIndex] = offset + 3;
        elements[offsetArrayIndex + 1] = offset + 2;
        elements[offsetArrayIndex + 2] = offset + 0;

        // Triangle 2
        elements[offsetArrayIndex + 3] = offset + 0;
        elements[offsetArrayIndex + 4] = offset + 2;
        elements[offsetArrayIndex + 5] = offset + 1;
    }


    internal void RemoveSprite(SpriteRenderer spr)
    {
        if(spr.Sprite?.Texture != null)RemoveTexture(spr.Sprite.Texture);
        
        for (var i = 0; i < _spriteCount; i++)
            if (_sprites[i] == spr)
            {
                for (var j = i; j < _spriteCount - 1; j++)
                {
                    _sprites[j] = _sprites[j + 1];
                    _sprites[j].IsDirty = true;
                }

                _spriteCount--;
            }
    }

    internal void RemoveTexture(Texture tex)
    {
        for (int i = 0; i < Textures.Length; i++)
        {
            if (Textures[i]?.TexID == (tex.TexID))
                Textures[i] = null;
        }
    }
}