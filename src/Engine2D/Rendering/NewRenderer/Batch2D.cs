using System.Numerics;
using Engine2D.Components.Cameras;
using Engine2D.Components.ENTT;
using Engine2D.Logging;
using Engine2D.Utilities;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL;

namespace Engine2D.Rendering.NewRenderer;

internal class Batch2D
{
    private int _zIndex;
    private Shader _shader;

    private List<Entity> _sprites = new();
    
    private int _quadCount = 0;
    private int _vaoId, _vboId;

    private float[] _vertices      = new float[(c_vertexSize * 4) * c_maxBatchSize];
    
    private Vector2[] _defCoords = new Vector2[4]
    {
        new Vector2(0f, 0f),
        new Vector2(1f, 0f),
        new Vector2(1f, 1f),
        new Vector2(0f, 1f),
    };

    private const int c_maxBatchSize = 10000;
    
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
    
    private static int[] s_Indices = new int[6 * c_maxBatchSize];
    private bool s_IndicesFilled = false;
    private Vector4[] _quadVertexPositions =
    {
        new(-0.5f, -0.5f, 0.0f, 1.0f),
        new(0.5f,  -0.5f, 0.0f, 1.0f),
        new(0.5f,   0.5f, 0.0f, 1.0f),
        new(-0.5f,  0.5f, 0.0f, 1.0f)
    };
    
    public bool HasRoom => _quadCount < c_maxBatchSize;
    
    internal void AddSprite(Entity ent)
    {
        var spriteRenderer = ent.GetComponent<ENTTSpriteRenderer>();
        
        var transform           = ent.GetComponent<ENTTTransformComponent>().Transform;//spriteRenderer.Parent.Transform.Position;
        var color                 = spriteRenderer.Color;//new Vector4(1,1,1,1);//spriteRenderer.Color;//spriteRenderer.Color;
        var textureCoords       = spriteRenderer.TextureCoords;
        var textureID                 = -1;//spriteRenderer.Sprite.Texture.TexID;
        
        LoadVertices(_quadCount, transform, color, textureCoords, textureID);
        
        _sprites.Add(ent);
        _quadCount++;
    }
    
    internal bool ChangeEntityAtIndex(int index)
    {
        // return true;
        var ent = _sprites[index];
        //TODO: ADD THE RIGHT PROPERTIES
        // Vector3 position, Vector4 color, Vector2[] textureCoords,int textureID
        
        var spriteRenderer = ent.GetComponent<ENTTSpriteRenderer>();
        
        var transform            = ent.GetComponent<ENTTTransformComponent>().Transform;//spriteRenderer.Parent.Transform.Position;
        var color          = spriteRenderer.Color;//new Vector4(1,1,1,1);//spriteRenderer.Color;//spriteRenderer.Color;
        var textureCoords = spriteRenderer.TextureCoords;
        float textureID          = -1;//spriteRenderer.Sprite.Texture.TexID;
        
        LoadVertices(index, transform, color, textureCoords, textureID);
        
        return true;
    }
    
    internal void Init(Shader shader, int zIndex)
    {
        if(!s_IndicesFilled)
        {
            s_Indices = GenerateIndices();
            s_IndicesFilled = true;
        }
        
        _shader = shader;
        _zIndex = zIndex;
        
        // CreateTestObjects();

        _vaoId  = GL.GenVertexArray();
        GL.BindVertexArray(_vaoId );
        
        // GL.CreateBuffers(1, out int QuadVB);
        _vboId = GL.GenBuffer(); 
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboId);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
            BufferUsageHint.DynamicDraw);

        var eboID  = GL.GenBuffer();//GL.CreateBuffers(1, out int );
        
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboID); 
        GL.BufferData(BufferTarget.ElementArrayBuffer,
            s_Indices.Length * sizeof(int), s_Indices, BufferUsageHint.StaticDraw);
        
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, c_posSize,
            VertexAttribPointerType.Float, false, 
            c_vertexSizeInBytes, c_posOffset);
        
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, c_colorSize, 
            VertexAttribPointerType.Float, false, 
            c_vertexSizeInBytes, c_colorOffset);
        
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, c_texCoordSize,
            VertexAttribPointerType.Float, false, 
            c_vertexSizeInBytes, c_texCoordOffset);
          
        GL.EnableVertexAttribArray(3);
        GL.VertexAttribPointer(3, c_texIDSize, 
            VertexAttribPointerType.Float, false,
            c_vertexSizeInBytes, c_texIdOffset);
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

    private void CreateTestObjects()
    {
        for (int i = 0; i < 1; i++)
        {
            Vector3 pos = new Vector3(Renderer.Batches.Count, 0, 0);
            Console.WriteLine(pos.X);
            Matrix4x4 Transform =   Matrix4x4.CreateScale(1,1, 1) *
                                    Matrix4x4.CreateFromQuaternion(Quaternion.Identity) *
                                    Matrix4x4.CreateTranslation(pos.X, pos.Y, 0);
            
            LoadVertices(_quadCount, Transform, 
                new Vector4(1,0,0,1), _defCoords, -1);
            _quadCount++;
        }
    }
    
    internal void Render(Camera camera)
    {
        if (camera == null)
        {
            Log.Error("Camera is not set!");
            return;
        }
        
        bool rebufferData = true;
        for (var i = 0; i < _quadCount; i++)
            if (_sprites[i].IsDirty)
            {
                //Log.Message("IsDirty: " + _sprites[i].GetComponent<ENTTTagComponent>().Tag);
                _sprites[i].IsDirty = false;
                ChangeEntityAtIndex(i);
                rebufferData = true;
            }

        
        if (rebufferData)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboId);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 
                IntPtr.Zero, _vertices.Length * sizeof(float), _vertices);
        }
        
        _shader.use();
        _shader.uploadMat4f("u_viewMatrix", camera.GetViewMatrix());
        _shader.uploadMat4f("u_projectionMatrix", camera.GetProjectionMatrix());
        
        // GL.BindVertexArray(_quadVA);

        GL.BindVertexArray(_vaoId);
        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);
        GL.EnableVertexAttribArray(2);
        GL.EnableVertexAttribArray(3);
        
        GL.DrawElements(PrimitiveType.Triangles, _quadCount * 6, DrawElementsType.UnsignedInt, 0);
        
        GL.DisableVertexAttribArray(0);
        GL.DisableVertexAttribArray(1);
        GL.DisableVertexAttribArray(2);
        GL.DisableVertexAttribArray(3);
        GL.BindVertexArray(0);

        _shader.detach();
    }
    
    private void LoadVertices(int index, Matrix4x4 transform, Vector4 color, Vector2[]? textureCoords,float textureID)
    {
        // //     -.5f, -.5f, 0,  .18F, .6F, .96F, 1F,   0f, 0f,             0f,      
        // //     .5f, -.5f, 0,   .18F, .6F, .96F, 1F,   1f, 0f,             0f,
        // //     .5f, .5f, 0,    .18F, .6F, .96F, 1F,   1f, 1f,             0f,
        // //     -.5f, .5f, 0,   .18F, .6F, .96F, 1F,   0f, 1f,             0f,
        //
        var offset =  index * 4 * c_vertexSize;

      
        for (int i = 0; i < 4; i++)
        {
            // float xaDD = 0;
            // float yaDD = 0;
            // float zaDD = 0;
            // switch (i)
            // {
            //     case 0:
            //         xaDD = -.5f;
            //         yaDD = -.5f;
            //         zaDD = 0;
            //         break;
            //     case 1:
            //         xaDD = .5f;
            //         yaDD = -.5f;
            //         zaDD = 0;
            //         break;
            //     case 2:
            //         xaDD = .5f;
            //         yaDD = .5f;
            //         zaDD = 0;
            //         break;
            //     case 3:
            //         xaDD = -.5f;
            //         yaDD = .5f;
            //         zaDD = 0;
            //         break;
            // }
            //
           
            var currentPos = MathUtils.Multiply(transform, _quadVertexPositions[i]); //quadVertexPositions[0] * translation;

            // Vector3 currentPos = new();
            // Vector3 position = new(transform.M41, transform.M42, transform.M43);
            // currentPos.X = position.X + xaDD;
            // currentPos.Y = position.Y + yaDD;
            // currentPos.Z = position.Z + zaDD;
            //
            _vertices[offset]     = currentPos.X;
            _vertices[offset + 1] = currentPos.Y;
            _vertices[offset + 2] = currentPos.Z;
            
            _vertices[offset + 3] = color.X;
            _vertices[offset + 4] = color.Y;
            _vertices[offset + 5] = color.Z;
            _vertices[offset + 6] = color.W;
            
            if(textureCoords == null)
                textureCoords = _defCoords;
            
            _vertices[offset + 7] = textureCoords[i].X;
            _vertices[offset + 8] = textureCoords[i].Y;
            
            _vertices[offset + 9] = textureID;
            
            offset+= c_vertexSize;
        }
    }

}