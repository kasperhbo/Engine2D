using System.Numerics;
using Engine2D.Components.Cameras;
using Engine2D.Components.ENTT;
using Engine2D.Logging;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL;

namespace Engine2D.Rendering.NewRenderer;

internal class Batch2D : IComparable<Batch2D>
{
    private int _zIndex;
    private Shader _shader;

    private List<ENTTSpriteRenderer> _sprites = new();
    
    private float[] _vertices = new float[c_vertexSize * c_maxBatchSize];
    
    private int _quadVA = 0;
    private int _quadCount = 0;
    
    private const int c_maxBatchSize = 1000;
    
    private const int c_posSize = 3;
    private const int c_colorSize = 4;
    private const int c_texCoordSize = 2;
    private const int c_texIDSize = 1;
    
    private const int c_posOffset = 0;
    private const int c_colorOffset = c_posOffset + c_posSize * sizeof(float);
    private const int c_texCoordOffset = c_colorOffset + c_colorSize * sizeof(float);
    private const int c_texIdOffset = c_texCoordOffset + c_texCoordSize * sizeof(float);
    
    private const int c_vertexSize = 10;
    private const int c_vertexSizeInBytes = c_vertexSize * sizeof(float);

    private static int[] s_Indices = new int[c_maxBatchSize * 6];
    private static bool s_IndicesFilled = false;
    private static Vector4 _clearColor = new(1,1,1,1);

    internal bool AddSprite(ENTTSpriteRenderer spriteRenderer)
    {
        if(_quadCount >= c_maxBatchSize)
            return false;
        
        //TODO: ADD THE RIGHT PROPERTIES
        // Vector3 position, Vector4 color, Vector2[] textureCoords,int textureID
        
        var pos           = spriteRenderer.Parent.GetComponent<ENTTTransformComponent>().Position;//spriteRenderer.Parent.Transform.Position;
        var color         = new Vector4(1,1,1,1);//spriteRenderer.Color;
        var textureCoords = new []{new Vector2(0,0), new Vector2(0,1), new Vector2(1,1), new Vector2(1,0)};//spriteRenderer.TextureCoords;
        var textureID     = 1;//spriteRenderer.Sprite.Texture.TexID;
        
        LoadVertices(
            new Vector3(pos.X, pos.Y, pos.X), color, textureCoords, textureID);
        
        _sprites.Add(spriteRenderer);
        
        return true;
    }
    
    internal void Init(Shader shader, int zIndex)
    {
        _shader = shader;
        _zIndex = zIndex;
        
        if(!s_IndicesFilled)
            FillElementArray();
        
        CreateTempObjects();
        
        GL.CreateVertexArrays(1, out
            _quadVA);
        GL.BindVertexArray(_quadVA);
        
        GL.CreateBuffers(1, out int QuadVB);
        GL.BindBuffer(BufferTarget.ArrayBuffer, QuadVB);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
        
        GL.EnableVertexArrayAttrib(QuadVB, 0);
        GL.VertexAttribPointer(0, c_posSize, VertexAttribPointerType.Float, false, c_vertexSizeInBytes, c_posOffset);
        
        GL.EnableVertexArrayAttrib(QuadVB, 1);
        GL.VertexAttribPointer(1, c_colorSize, VertexAttribPointerType.Float, false, c_vertexSizeInBytes, c_colorOffset);
        
        GL.EnableVertexArrayAttrib(QuadVB, 2);
        GL.VertexAttribPointer(2, c_texCoordSize, VertexAttribPointerType.Float, false, c_vertexSizeInBytes, c_texCoordOffset);
          
        GL.EnableVertexArrayAttrib(QuadVB, 3);
        GL.VertexAttribPointer(3, c_texIDSize, VertexAttribPointerType.Float, false, c_vertexSizeInBytes, c_texIdOffset);
        
        //
        GL.CreateBuffers(1, out int QuadIB);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, QuadIB);
        GL.BufferData(BufferTarget.ElementArrayBuffer,
            s_Indices.Length * sizeof(int), s_Indices, BufferUsageHint.DynamicDraw);
    }

    internal void Render(Camera camera)
    {
        if (camera == null)
        {
            Log.Error("Camera is not set!");
            return;
        }
        bool rebufferData = true;
        if (rebufferData)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _quadVA);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 
                IntPtr.Zero, _vertices.Length * sizeof(float), _vertices);
        }
        
        _shader.use();
        _shader.uploadMat4f("u_viewMatrix", camera.GetViewMatrix());
        _shader.uploadMat4f("u_projectionMatrix", camera.GetProjectionMatrix());
        
        GL.BindVertexArray(_quadVA);
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        
        _shader.detach();
    }
    
    private void CreateTempObjects()
    {

        var defaultTextureCoords = new Vector2[4]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(1f, 1f),
            new Vector2(0f, 1f),
        };
        
        for (int i = 0; i < 1; i++)
        {
            var texID = -1;
            
            LoadVertices(
                new Vector3(0,0,0), 
                new Vector4(1,1,1,1),
                defaultTextureCoords, 
                0
                );
            
            _quadCount++;
        }
    }

    private void LoadVertices(Vector3 position, Vector4 color, Vector2[] textureCoords,int textureID)
    {
        //     -.5f, -.5f, 0,  .18F, .6F, .96F, 1F,   0f, 0f,             0f,      
        //     .5f, -.5f, 0,   .18F, .6F, .96F, 1F,   1f, 0f,             0f,
        //     .5f, .5f, 0,    .18F, .6F, .96F, 1F,   1f, 1f,             0f,
        //     -.5f, .5f, 0,   .18F, .6F, .96F, 1F,   0f, 1f,             0f,
        
        for (int i = 0; i < 4; i++)
        {
            float xaDD = 0;
            float yaDD = 0;
            float zaDD = 0;
            switch (i)
            {
                case 0:
                    xaDD = -.5f;
                    yaDD = -.5f;
                    zaDD = 0;
                    break;
                case 1:
                    xaDD = .5f;
                    yaDD = -.5f;
                    zaDD = 0;
                    break;
                case 2:
                    xaDD = .5f;
                    yaDD = .5f;
                    zaDD = 0;
                    break;
                case 3:
                    xaDD = -.5f;
                    yaDD = .5f;
                    zaDD = 0;
                    break;
            }
            var offset = c_vertexSize * _quadCount + i * c_vertexSize;
            var xPos = position.X + xaDD;
            var yPos = position.Y + yaDD;
            var zPos = position.Z + zaDD;
            
            _vertices[offset]     = xPos;
            _vertices[offset + 1] = yPos;
            _vertices[offset + 2] = zPos;
            
            _vertices[offset + 3] = color.X;
            _vertices[offset + 4] = color.Y;
            _vertices[offset + 5] = color.Z;
            _vertices[offset + 6] = color.W;
            
            _vertices[offset + 7] = textureCoords[i].X;
            _vertices[offset + 8] = textureCoords[i].Y;
            
            _vertices[offset + 9] = textureID;
        }
    }
  
    private void FillElementArray()
    {
        // int[] _indices = new[]
        // {
        //     0, 1, 2, // first triangle
        //     2, 3, 0,
        //     
        //     4,5,6,  // second triangle
        //     6,7,4
        // };
        
        for (int i = 0; i < 1; i++)
        {
            var offsetArrayIndex = 6 * i;
            var offset = 4 * i;

            // 3, 2, 0, 0, 2, 1        7, 6, 4, 4, 6, 5
            // Triangle 1
            s_Indices[offsetArrayIndex] = offset + 0;
            s_Indices[offsetArrayIndex + 1] = offset + 1;
            s_Indices[offsetArrayIndex + 2] = offset + 2;

            // Triangle 2
            s_Indices[offsetArrayIndex + 3] = offset + 2;
            s_Indices[offsetArrayIndex + 4] = offset + 3;
            s_Indices[offsetArrayIndex + 5] = offset + 0;
        }
        
        Batch2D.s_IndicesFilled= true;
    }
    

    public int CompareTo(Batch2D? other)
    {
        if (_zIndex < other._zIndex)
            return -1;

        if (_zIndex == other._zIndex)
            return 0;

        return 1;
    }

    
}