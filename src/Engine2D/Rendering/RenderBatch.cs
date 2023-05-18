using System.Net.Mime;
using System.Threading.Channels;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Testing;
using KDBEngine.Core;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine2D.Rendering;

/// <summary>
///     Old code that I got from following GAMES WITH GABE'S How to create a game engine in Java Series!
///     Thanks Gabe, your code is now my property;)
/// </summary>
internal class RenderBatch: IComparable<RenderBatch>
{
    internal RenderBatch(int zIndex)
    {
        this.ZIndex = zIndex;
        _textureUnits = new int[GL.GetInteger(GetPName.MaxTextureImageUnits)];
        _textures = new Texture[GL.GetInteger(GetPName.MaxTextureImageUnits)];
        
        for (var i = 0; i < _textureUnits.Length; i++) _textureUnits[i] = i;

        var dat = new ShaderData();
        dat.VertexPath = Utils.GetBaseEngineDir() + "/Shaders/default-unlit-lighting.vert";
        dat.FragPath = Utils.GetBaseEngineDir() + "/Shaders/default-unlit-lighting.frag";

        _shader = ResourceManager.GetShader(dat);
        sprites = new SpriteRenderer[c_MaxBatchSize];

        _vertices = new float[c_MaxBatchSize * c_VertexSize * 4];
    }

    internal void Init()
    {
        _vaoID = GL.GenVertexArray();
        GL.BindVertexArray(_vaoID);

        _vboID = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboID);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
            BufferUsageHint.DynamicDraw);

        var eboID = GL.GenBuffer();
        var indices = GenerateIndices();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboID);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices,
            BufferUsageHint.StaticDraw);

        // Enable the buffer attribute pointers
        GL.VertexAttribPointer(0, c_PosSize, VertexAttribPointerType.Float, false, c_VertexSizeInBytes, c_PosOffset);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, c_ColorSize, VertexAttribPointerType.Float, false, c_VertexSizeInBytes,
            c_ColorOffset);
        GL.EnableVertexAttribArray(1);

        GL.VertexAttribPointer(2, c_TexCoordSize, VertexAttribPointerType.Float, false, c_VertexSizeInBytes,
            c_TexCoordOffset);
        GL.EnableVertexAttribArray(2);

        GL.VertexAttribPointer(3, c_TexIDSize, VertexAttribPointerType.Float, false, c_VertexSizeInBytes,
            c_TexIDOffset);
        GL.EnableVertexAttribArray(3);
    }

    public void AddSprite(SpriteRenderer spr)
    {
        var index = SpriteCount;
        sprites[index] = spr;
        SpriteCount++;

        if (spr.texture != null)
            if (!_textures.Contains(spr.texture))
            {
                for (int i = 0; i < _textures.Length; i++)
                {
                    if (_textures[i] == null)
                    {
                        _textures[i] = spr.texture;
                        break;
                    }
                }
            }

        LoadVertexProperties(index);
    }

    public void Render(Matrix4 projectionMatrix, Matrix4 viewMatrix)
    {
        // For now, we will rebuffer all data every frame
        var rebufferData = false;

        for (var i = 0; i < SpriteCount; i++)
            if (sprites[i].IsDirty)
            {
                sprites[i].IsDirty = false;
                LoadVertexProperties(i);
                rebufferData = true;
            }

        if (rebufferData)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboID);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, _vertices.Length * sizeof(float), _vertices);
        }
        
        _shader.use();
        _shader.uploadMat4f("uProjection", projectionMatrix);
        _shader.uploadMat4f("uView", viewMatrix);
        
        for (var i = 0; i < _textures.Length; i++)
        {
            if(_textures[i] != null){
                GL.ActiveTexture(TextureUnit.Texture0 + i + 1);
                _textures[i].bind();
            }
        }
        
        _shader.UploadIntArray("uTextures", _textureUnits);

        GL.BindVertexArray(_vaoID);
        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);
        
        

        GL.DrawElements(PrimitiveType.Triangles, SpriteCount * 6, DrawElementsType.UnsignedInt, 0);

        for (var i = 0; i < _textures.Length; i++)
        {
            if (_textures[i] != null)
            {
                _textures[i].unbind();
            }
        }

        GL.DisableVertexAttribArray(0);
        GL.DisableVertexAttribArray(1);
        GL.BindVertexArray(0);

        _shader.detach();
    }


    private void LoadVertexProperties(int index)
    {
        var sprite = sprites[index];

        // Find offset within array (4 vertices per sprite)
        var offset = index * 4 * c_VertexSize;

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

        Transform transform = sprite.Parent.transform;

        System.Numerics.Matrix4x4 t = System.Numerics.Matrix4x4.Identity;
        t =     System.Numerics.Matrix4x4.CreateTranslation(transform.position.X, transform.position.Y,0);
        t = t * System.Numerics.Matrix4x4.CreateRotationZ(-MathHelper.DegreesToRadians(transform.rotation));
        
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
                              
            
        t = new System.Numerics.Matrix4x4(
            m11,m12,m13,m14,
            m21,m22,m23,m24,
            m31,m32,m33,m34,
            m41,m42,m43,m44
        );
        
        for (var i = 0; i < 4; i++)
        {
            if (i == 1)
                yAdd = -0.5f;
            else if (i == 2)
                xAdd = -0.5f;
            else if (i == 3) yAdd = 0.5f;

            Vector4 currentPos = new Vector4(
                transform.position.X +
                (xAdd * transform.size.X),
                    
                transform.position.Y +
                (yAdd * transform.size.Y),
                    
                0, 1);

            if (transform.rotation != 0)
            {
                currentPos = MathUtils.Multiply(t, new(xAdd, yAdd, 0, 0));
            }
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

            offset += c_VertexSize;
        }
    }

    private int[] GenerateIndices()
    {
        // 6 indices per quad (3 per triangle)
        var elements = new int[6 * c_MaxBatchSize];

        for (var i = 0; i < c_MaxBatchSize; i++) loadElementIndices(elements, i);

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
        for (var i = 0; i < SpriteCount; i++)
            if (sprites[i] == spr)
            {
                for (var j = i; j < SpriteCount - 1; j++)
                {
                    sprites[j] = sprites[j + 1];
                    sprites[j].IsDirty = true;
                }

                SpriteCount--;
            }
    }

    #region fields

    private const int c_MaxBatchSize = 20000;


    private const int c_PosSize = 2;
    private const int c_ColorSize = 4;
    private const int c_TexCoordSize = 2;
    private const int c_TexIDSize = 1;

    private const int c_PosOffset = 0;
    private const int c_ColorOffset = c_PosOffset + c_PosSize * sizeof(float);
    private const int c_TexCoordOffset = c_ColorOffset + c_ColorSize * sizeof(float);
    private const int c_TexIDOffset = c_TexCoordOffset + c_TexCoordSize * sizeof(float);

    private const int c_VertexSize = 9;
    private const int c_VertexSizeInBytes = c_VertexSize * sizeof(float);

    private readonly SpriteRenderer[] sprites;
    public int SpriteCount { get; private set; }
    public bool HasRoom => SpriteCount < c_MaxBatchSize;

    private readonly float[] _vertices = new float[c_MaxBatchSize * c_VertexSize];

    private int _vaoID, _vboID;

    private readonly Shader _shader;

    private readonly Texture[] _textures;
    private readonly int[] _textureUnits;

    public int ZIndex = 0;

    #endregion

    public int CompareTo(RenderBatch? other)
    {
        if (this.ZIndex < other.ZIndex)
            return -1;
            
        if (this.ZIndex == other.ZIndex)
            return 0;
            
        return 1;
    }
}