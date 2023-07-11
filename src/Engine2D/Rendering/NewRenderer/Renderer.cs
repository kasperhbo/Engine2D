using System.Numerics;
using Engine2D.Components.Cameras;
using Engine2D.Components.ENTT;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Testing;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.Rendering.NewRenderer;

internal static class Renderer
{
    internal static TestFrameBuffer GameFrameBuffer   = new(1920, 1080);
    internal static TestFrameBuffer EditorFrameBuffer = new(1920, 1080);

    // private static float[] _vertices = new[]
    // {
    //     //Pos             // Color              // TexCoord         // TexId
    //     -1.5f, -.5f, 0,  .18F, .6F, .96F, 1F,   0f, 0f,             0f,      
    //     -.5f, -.5f, 0,   .18F, .6F, .96F, 1F,   1f, 0f,             0f,
    //     -.5f, .5f, 0,    .18F, .6F, .96F, 1F,   1f, 1f,             0f,
    //     -1.5f, .5f, 0,   .18F, .6F, .96F, 1F,   0f, 1f,             0f,
    //                      
    //     .5f, -.5f, 0,     1F, 1F, 1F, 1F,       0f, 0f,             1f,
    //     1.5f, -.5f, 0,    1F, 1F, 1F, 1F,       1f, 0f,             1f,
    //     1.5f, .5f, 0,     1F, 1F, 1F, 1F,       1f, 1f,             1f,
    //     .5f, .5f, 0,      1F, 1F, 1F, 1F,       0f, 1f,             1f,
    // };
    //
    // private static Shader _shader;
    // private static int _quadVA;
    //
    //
    // private const int c_posSize = 3;
    // private const int c_colorSize = 4;
    // private const int c_texCoordSize = 2;
    // private const int c_texIDSize = 1;
    //
    // private const int c_posOffset = 0;
    // private const int c_colorOffset = c_posOffset + c_posSize * sizeof(float);
    // private const int c_texCoordOffset = c_colorOffset + c_colorSize * sizeof(float);
    // private const int c_texIdOffset = c_texCoordOffset + c_texCoordSize * sizeof(float);
    //
    // private const int c_vertexSize = 10;
    // private const int c_vertexSizeInBytes = c_vertexSize * sizeof(float);
    
    private static List<Batch2D> _batches = new();
    public static Vector4 ClearColor = new(.2F, .2F, .2F, 1.0f);

    internal static void Init()
    {
        _batches.Add(new Batch2D());

        var shader = new Shader("Shaders\\ShaderFiles\\testshader.vert", "Shaders\\ShaderFiles\\testshader.frag");
        
        foreach (var batch in _batches)
        {
            batch.Init(shader, 0);
        }
        
        // GL.CreateVertexArrays(1, out
        //     _quadVA);
        // GL.BindVertexArray(_quadVA);
        //
        // GL.CreateBuffers(1, out int QuadVB);
        // GL.BindBuffer(BufferTarget.ArrayBuffer, QuadVB);
        // GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
        //
        // GL.EnableVertexArrayAttrib(QuadVB, 0);
        // GL.VertexAttribPointer(0, c_posSize, VertexAttribPointerType.Float, false, c_vertexSizeInBytes, c_posOffset);
        //
        // GL.EnableVertexArrayAttrib(QuadVB, 1);
        // GL.VertexAttribPointer(1, c_colorSize, VertexAttribPointerType.Float, false, c_vertexSizeInBytes, c_colorOffset);
        //
        // GL.EnableVertexArrayAttrib(QuadVB, 2);
        // GL.VertexAttribPointer(2, c_texCoordSize, VertexAttribPointerType.Float, false, c_vertexSizeInBytes, c_texCoordOffset);
        //   
        // GL.EnableVertexArrayAttrib(QuadVB, 3);
        // GL.VertexAttribPointer(3, c_texIDSize, VertexAttribPointerType.Float, false, c_vertexSizeInBytes, c_texIdOffset);
        //
        // int[] _indices = new[]
        // {
        //     0, 1, 2,
        //     2, 3, 0,
        //     
        //     4,5,6,
        //     6,7,4
        // };
        //
        // GL.CreateBuffers(1, out int QuadIB);
        // GL.BindBuffer(BufferTarget.ElementArrayBuffer, QuadIB);
        // GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(int), _indices, BufferUsageHint.StaticDraw);
        //
        // //Load the shader
        //
        EditorFrameBuffer = new TestFrameBuffer(1920, 1080);
        GameFrameBuffer = new TestFrameBuffer(1920, 1080);
        // Texture texture = new Texture("Images\\TestImages\\512x512 Texel Density Texture 1.png");
        // Texture texture2 = new Texture("Images\\TestImages\\512x512 Texel Density Texture 1.png");
    }
    
    
    internal static void BeginScene()
    {
        
    }

    internal static void EndScene()
    {
        
    }

    internal static void Render()
    {
        RenderEditorbuffer();
        RenderGamebuffer();
    }

    
    
    private static void RenderEditorbuffer()
    {
        EditorFrameBuffer.Bind();
        GL.ClearColor(ClearColor.X, ClearColor.Y, ClearColor.Z, ClearColor.W);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var cam = Engine.Get().CurrentScene.GetEditorCamera();
        if (cam == null)
        {
            EditorFrameBuffer.UnBind();
            //TODO:REANABLE THIS
            // Log.Error("No Editor camera set!");
            return;
        }
        foreach (var batch in _batches)
        {
            batch.Render(Engine.Get().CurrentScene.GetEditorCamera());
        }
        EditorFrameBuffer.UnBind();
    }

    private static void RenderGamebuffer()
    {
        GameFrameBuffer.Bind();
        GL.ClearColor(ClearColor.X, ClearColor.Y, ClearColor.Z, ClearColor.W);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var cam = Engine.Get().CurrentScene.GetMainCamera();
        if (cam == null)
        {
            GameFrameBuffer.UnBind();
            return;
        }
        foreach (var batch in _batches)
        {
            batch.Render(Engine.Get().CurrentScene.GetMainCamera());
        }
        GameFrameBuffer.UnBind();
    }

    private static void RenderScene(Camera? camera)
    {
        // if (camera == null)
        // {
        //     Log.Error("Camera is not set!");
        //     return;
        // }
        //
        // GL.ClearColor(_clearColor.X, _clearColor.Y, _clearColor.Z, _clearColor.W);
        // GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        //
        // _shader.use();
        // _shader.uploadMat4f("u_viewMatrix", camera.GetViewMatrix());
        // _shader.uploadMat4f("u_projectionMatrix", camera.GetProjectionMatrix());
        //
        // GL.BindVertexArray(_quadVA);
        // GL.DrawElements(PrimitiveType.Triangles, 12, DrawElementsType.UnsignedInt, 0);
        //
        // _shader.detach();
    }

    internal static void Resize()
    {
        GameFrameBuffer = new TestFrameBuffer(Engine.Get().Size);
        EditorFrameBuffer = new TestFrameBuffer(Engine.Get().Size);
    }

    public static void AddSprite(ENTTSpriteRenderer spriteRenderer)
    {
        bool added = false;
        foreach (var batch in _batches)
        {
            if (batch.AddSprite(spriteRenderer))
            {
                added = true;
                break;
            }
        }

        if (!added)
        {
            _batches.Add(new Batch2D());
            _batches[^1]
                .Init(new Shader("Shaders\\ShaderFiles\\testshader.vert", "Shaders\\ShaderFiles\\testshader.frag"),
                    _batches.Count - 1);
            _batches[^1].AddSprite(spriteRenderer);
        }
    }

    public static void RemoveSprite(ENTTSpriteRenderer enttSpriteRenderer)
    {
        
    }
}