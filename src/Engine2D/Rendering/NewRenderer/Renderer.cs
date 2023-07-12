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
        EditorFrameBuffer = new TestFrameBuffer(1920, 1080);
        GameFrameBuffer = new TestFrameBuffer(1920, 1080);
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
        
    }

    internal static void Resize()
    {
        GameFrameBuffer = new TestFrameBuffer(Engine.Get().Size);
        EditorFrameBuffer = new TestFrameBuffer(Engine.Get().Size);
    }

    public static void AddSprite(Entity ent)
    {
        bool added = false;
        foreach (var batch in _batches)
        {
            if (batch.AddSprite(ent))
            {
                added = true;
                break;
            }
        }

        if (!added)
        {
            var batch = new Batch2D();
            _batches.Add(batch);
            batch
                .Init(new Shader(
                        "Shaders\\ShaderFiles\\testshader.vert",
                        "Shaders\\ShaderFiles\\testshader.frag"),
                    0);
            batch.AddSprite(ent);
        }
    }

    public static void RemoveSprite(Entity enttSpriteRenderer)
    {
        throw new NotImplementedException();
    }
}