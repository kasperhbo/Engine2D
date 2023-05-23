using System.Numerics;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Testing;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using TextureUnit = OpenTK.Graphics.OpenGL4.TextureUnit;
using Vector4 = OpenTK.Mathematics.Vector4;

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
    private readonly Shader _shader;

    private readonly SpriteRenderer[] _sprites = new SpriteRenderer[c_maxBatchSize];

    private readonly Texture[] _textures = new Texture[(int)ShaderDefaultSlots.AVAILABLETEXTUREUNITS];
    private readonly int[] _textureUnits = new int[(int)ShaderDefaultSlots.AVAILABLETEXTUREUNITS];
    private readonly float[] _vertices = new float[c_maxBatchSize * c_vertexSize];

    private int _vaoId, _vboId;
    public int ZIndex;

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

    public bool HasRoom => _spriteCount < c_maxBatchSize;
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

    public void AddSprite(SpriteRenderer spr)
    {
        var index = _spriteCount;
        _sprites[index] = spr;
        _spriteCount++;

        if (spr.texture != null)
            if (!_textures.Contains(spr.texture))
                for (var i = 0; i < _textures.Length; i++)
                    if (_textures[i] == null)
                    {
                        _textures[i] = spr.texture;
                        break;
                    }

        LoadVertexProperties(index);
    }

    public void Render(TestCamera camera, Texture lightmapTexture)
    {
        var projectionMatrix = camera.getProjectionMatrix();
        var viewMatrix = camera.getViewMatrix();

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
        lightmapTexture.Use(TextureUnit.Texture0 + (int)ShaderDefaultSlots.LIGHTMAPTEXTURESLOT);
        _shader.uploadInt("uLightmap", (int)ShaderDefaultSlots.LIGHTMAPTEXTURESLOT);

        //TEXTURES
        for (var i = 0; i < _textures.Length; i++)
            if (_textures[i] != null)
            {
                GL.ActiveTexture(OpenTK.Graphics.OpenGL.TextureUnit.Texture0 + i + 1);
                _textures[i].bind();
            }

        _shader.UploadIntArray("uTextures", _textureUnits);

        GL.BindVertexArray(_vaoId);
        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);

        GL.DrawElements(PrimitiveType.Triangles, _spriteCount * 6, DrawElementsType.UnsignedInt, 0);

        for (var i = 0; i < _textures.Length; i++)
            if (_textures[i] != null)
                _textures[i].unbind();

        GL.DisableVertexAttribArray(0);
        GL.DisableVertexAttribArray(1);
        GL.BindVertexArray(0);

        _shader.detach();
    }


    private void LoadVertexProperties(int index)
    {
        var sprite = _sprites[index];

        // Find offset within array (4 vertices per sprite)
        var offset = index * 4 * c_vertexSize;

        Vector4 color = new(sprite.Color.Color.X, sprite.Color.Color.Y, sprite.Color.Color.Z, sprite.Color.Color.W);

        var texID = -1;
        var texCoords = sprite.TextureCoords;

        //Find texture
        if (sprite.texture != null)
            for (var i = 0; i < _textures.Length; i++)
                if (_textures[i].Equals(sprite.texture))
                {
                    texID = i + 1;
                    break;
                }


        // Add vertices with the appropriate properties
        var xAdd = 0.5f;
        var yAdd = 0.5f;

        var transform = sprite.Parent.transform;

        var t = Matrix4x4.Identity;
        t = Matrix4x4.CreateTranslation(transform.position.X, transform.position.Y, 0);
        t = t * Matrix4x4.CreateRotationZ(-MathHelper.DegreesToRadians(transform.rotation));

        t.M41 = transform.position.X;
        t.M42 = transform.position.Y;

        var m11 = t.M11 * transform.size.X;
        var m12 = t.M12 * transform.size.X;
        var m13 = t.M13 * transform.size.X;
        var m14 = t.M14 * transform.size.X;

        var m21 = t.M21 * transform.size.Y;
        var m22 = t.M22 * transform.size.Y;
        var m23 = t.M23 * transform.size.Y;
        var m24 = t.M24 * transform.size.Y;

        var m31 = t.M31 * 1;
        var m32 = t.M32 * 1;
        var m33 = t.M33 * 1;
        var m34 = t.M34 * 1;

        var m41 = t.M41;
        var m42 = t.M42;
        var m43 = t.M43;
        var m44 = t.M44;


        t = new Matrix4x4(
            m11, m12, m13, m14,
            m21, m22, m23, m24,
            m31, m32, m33, m34,
            m41, m42, m43, m44
        );

        for (var i = 0; i < 4; i++)
        {
            if (i == 1)
                yAdd = -0.5f;
            else if (i == 2)
                xAdd = -0.5f;
            else if (i == 3) yAdd = 0.5f;

            var currentPos = new Vector4(
                transform.position.X +
                xAdd * transform.size.X,
                transform.position.Y +
                yAdd * transform.size.Y,
                0, 1);

            if (transform.rotation != 0) currentPos = MathUtils.Multiply(t, new Vector4(xAdd, yAdd, 0, 0));
            // Load position
            _vertices[offset] = currentPos.X;
            _vertices[offset + 1] = currentPos.Y;

            // Load color
            _vertices[offset + 2] = color.X;
            _vertices[offset + 3] = color.Y;
            _vertices[offset + 4] = color.Z;
            _vertices[offset + 5] = color.W;

            //Load tex coords
            _vertices[offset + 6] = texCoords[i].X;
            _vertices[offset + 7] = texCoords[i].Y;

            //Load tex id
            _vertices[offset + 8] = texID;

            offset += c_vertexSize;
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


    public void RemoveSprite(SpriteRenderer spr)
    {
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
}