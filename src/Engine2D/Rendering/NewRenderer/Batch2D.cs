using System.Numerics;
using Engine2D.Components.Cameras;
using Engine2D.Components.ENTT;
using Engine2D.Logging;
using Engine2D.Utilities;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL;
using TextureUnit = OpenTK.Graphics.OpenGL4.TextureUnit;

namespace Engine2D.Rendering.NewRenderer;

internal class Batch2D
{
    internal bool DebuggerOpened = false;
    
    internal int ZIndex { get; private set; }
    internal Shader Shader { get; private set; }

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
    
    //Texture IDS
    public int[] TextureIDS { get; private set; } = new int[7]
    {
        -1,
        -1,
        -1,
        -1,
        -1,
        -1,
        -1
    };
    
    private readonly int[] _textureUnits = new int[8]
    {
        0,1,2,3,4,5,6,7
    };
    
    private Vector4[] _quadVertexPositions =
    {
        new(-0.5f, -0.5f, 0.0f, 1.0f),
        new(0.5f,  -0.5f, 0.0f, 1.0f),
        new(0.5f,   0.5f, 0.0f, 1.0f),
        new(-0.5f,  0.5f, 0.0f, 1.0f)
    };
    
    private bool _hasRoom => _quadCount < c_maxBatchSize;

    public bool CanAdd(Entity ent)
    {
        if(!_hasRoom)
            return false;

        if (!ent.HasComponent<ENTTSpriteRenderer>()) return false;
        
        var comp = ent.GetComponent<ENTTSpriteRenderer>();
        if(comp.Sprite != null)
        {
            if (comp.Sprite?.Texture == null) return false;
                
            var textureIdSpriteRenderer = comp.Sprite.Texture.TexID;

            //Check if batch contains the texture already, if it does, then we can add the sprite renderer
            //else make an new batch
            if (comp.Sprite?.Texture?.TexID != -1)
            {
                for (int i = 0; i < TextureIDS.Length; i++)
                {
                    if (TextureIDS[i] == textureIdSpriteRenderer || TextureIDS[i] == -1)
                    {
                        return true;
                    }
                }
            }
            //If the sprite renderer has no texture, then its fine to add it
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
    
    
    
    internal void AddSprite(Entity ent)
    {
        _sprites.Add(ent);
        ChangeEntityAtIndex(_quadCount);
        _quadCount++;
    }
    
    internal bool ChangeEntityAtIndex(int index)
    {
        // return true;
        var ent = _sprites[index];
        var spriteRenderer = ent.GetComponent<ENTTSpriteRenderer>();

        var transformComponent = ent.GetComponent<ENTTTransformComponent>();
        
        var transform = transformComponent.Transform();//spriteRenderer.Parent.Transform.Position;
        var color           = spriteRenderer.Color;//new Vector4(1,1,1,1);//spriteRenderer.Color;//spriteRenderer.Color;
        Vector2[] textureCoords    = spriteRenderer.TextureCoords;

        var textureID = -1; //spriteRenderer.Sprite.Texture.TexID;
        int slot = 0;
        
        if(spriteRenderer.Sprite?.Texture != null)
        {
            if (spriteRenderer.Sprite.Texture.TexID != -1)
            {
                textureID = spriteRenderer.Sprite.Texture.TexID;

                bool addTextureNewToList = true;
                
                //First check if the texture is already in the batch
                if(TextureIDS.Contains(textureID))// == textureID)
                {
                    for (int i = 0; i < TextureIDS.Length; i++)
                    {
                        if (TextureIDS[i] == textureID)
                        {
                            slot = i + 1;
                            addTextureNewToList = false;
                            break;
                        }
                    }
                }
                
                if(addTextureNewToList)
                {
                    for (int i = 0; i < TextureIDS.Length; i++)
                    {
                        //If the texture is not in the batch, then add it to the first empty slot
                        if (TextureIDS[i] == -1)
                        {
                            slot = i + 1;
                            TextureIDS[i] = textureID;
                            break;
                        }
                    }
                }

                transform = transformComponent.Transform(
                    spriteRenderer
                );
            }
        }
        
        LoadVertices(index, transform, color, textureCoords, slot, spriteRenderer);

        return true;
    }
    
    internal void Init(Shader shader, int zIndex)
    {
        if(!s_IndicesFilled)
        {
            s_Indices = GenerateIndices();
            s_IndicesFilled = true;
        }
        
        Shader = shader;
        ZIndex = zIndex;
        
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

    internal void Render(Camera camera)
    {
        if (camera == null)
        {
            Log.Error("Camera is not set!");
            return;
        }
        
        bool rebufferData = true;
        
        for (var i = 0; i < _quadCount; i++)
        {
            if (_sprites[i].IsDirty)
            {
                _sprites[i].IsDirty = false;
                ChangeEntityAtIndex(i);
                rebufferData = true;
            }
        }
        
        if (rebufferData)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboId);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 
                IntPtr.Zero, _vertices.Length * sizeof(float), _vertices);
        }
        //
        Shader.use();
        Shader.uploadMat4f("u_viewMatrix", camera.GetViewMatrix());
        Shader.uploadMat4f("u_projectionMatrix", camera.GetProjectionMatrix());
        Shader.UploadIntArray("uTextures", _textureUnits);

        // //TEXTURES


        GL.BindVertexArray(_vaoId);
        for (var i = 0; i < TextureIDS.Length; i++)
        {
            if (TextureIDS[i] != -1)
            {
                var unit = TextureUnit.Texture0 + i + 1;
                var id = (int)TextureIDS[i];

                Texture.Use(unit, id);
            }
        }
        
        //
        // if (_textureIDS[0] != -1)
        // {
        //     Texture.Use(TextureUnit.Texture1, 1);
        // }

        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);
        GL.EnableVertexAttribArray(2);
        GL.EnableVertexAttribArray(3);
        
        GL.DrawElements(PrimitiveType.Triangles, _quadCount * 6, DrawElementsType.UnsignedInt, 0);
        
        for (var i = 0; i < TextureIDS.Length; i++)
        {
            if (TextureIDS[i] != -1)
            {
                var unit = TextureUnit.Texture0 + i + 1;
                Texture.Use(unit, 0);
            }
        }
        
        Shader.detach();
        GL.DisableVertexAttribArray(0);
        GL.DisableVertexAttribArray(1);
        GL.DisableVertexAttribArray(2);
        GL.DisableVertexAttribArray(3);
        GL.BindVertexArray(0);

    }
    
    private void LoadVertices(int index, Matrix4x4 transform, Vector4 color, Vector2[] textureCoords, float textureSlot, ENTTSpriteRenderer spriteRenderer)
    {
        var coords = spriteRenderer.TextureCoords;
        
        // //     -.5f, -.5f, 0,  .18F, .6F, .96F, 1F,   0f, 0f,             0f,      
        // //     .5f, -.5f, 0,   .18F, .6F, .96F, 1F,   1f, 0f,             0f,
        // //     .5f, .5f, 0,    .18F, .6F, .96F, 1F,   1f, 1f,             0f,
        // //     -.5f, .5f, 0,   .18F, .6F, .96F, 1F,   0f, 1f,             0f,
        
        var offset =  index * 4 * c_vertexSize;

      
        for (int i = 0; i < 4; i++)
        {
            var currentPos =
                MathUtils.Multiply(transform, _quadVertexPositions[i]); //quadVertexPositions[0] * translation;

            _vertices[offset]     = currentPos.X;
            _vertices[offset + 1] = currentPos.Y;
            _vertices[offset + 2] = currentPos.Z;
            
            _vertices[offset + 3] = color.X;
            _vertices[offset + 4] = color.Y;
            _vertices[offset + 5] = color.Z;
            _vertices[offset + 6] = color.W;
            
            // if(textureCoords == null)
            //     textureCoords = _defCoords;
            
            _vertices[offset + 7] = textureCoords[i].X;
            _vertices[offset + 8] = textureCoords[i].Y;
            
            //Textuer unit
            _vertices[offset + 9] = textureSlot;
            
            offset+= c_vertexSize;
        }
    }

}