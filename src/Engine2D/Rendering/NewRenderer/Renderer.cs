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

    public static List<Batch2D> Batches = new();
    public static Vector4 ClearColor = new(.2F, .2F, .2F, 1.0f);

    internal static void Init()
    {
        EditorFrameBuffer = new TestFrameBuffer(1920, 1080);
        GameFrameBuffer = new TestFrameBuffer(1920, 1080);
        
        // var batch = new Batch2D();
        // Batches.Add(batch);
        // batch.Init(new Shader(
        //         "Shaders\\ShaderFiles\\testshader.vert",
        //         "Shaders\\ShaderFiles\\testshader.frag"),
        //     0);
        //
        // var batch1 = new Batch2D();
        // Batches.Add(batch1);
        // batch1.Init(new Shader(
        //         "Shaders\\ShaderFiles\\testshader.vert",
        //         "Shaders\\ShaderFiles\\testshader.frag"),
        //     0);
        //
        // var batch2 = new Batch2D();
        // Batches.Add(batch2);
        // batch2.Init(new Shader(
        //         "Shaders\\ShaderFiles\\testshader.vert",
        //         "Shaders\\ShaderFiles\\testshader.frag"),
        //     0);
        //
        // var batch3 = new Batch2D();
        // Batches.Add(batch3);
        // batch3.Init(new Shader(
        //         "Shaders\\ShaderFiles\\testshader.vert",
        //         "Shaders\\ShaderFiles\\testshader.frag"),
        //     0);
    }
    
    
    internal static void BeginScene()
    {
        
    }

    internal static void EndScene()
    {
        
    }

    internal static void Render()
    {
        // GL.ClearColor(ClearColor.X, ClearColor.Y, ClearColor.Z, ClearColor.W);
        // foreach (var batch in Batches)
        // {
        //     batch.Render(Engine.Get().CurrentScene.GetEditorCamera());
        // }
        if(Settings.s_IsEngine)
        {
            RenderEditorbuffer();
            // RenderGamebuffer();
        }
        //TODO: MAKE THIS RENDER SCENE INSTEAD OF EDITOR CAMERA
        // else
        // {
        //     foreach (var batch in Batches)
        //     {
        //         batch.Render(Engine.Get().CurrentScene.GetEditorCamera());
        //     }
        // }     

        foreach (var toremove in m_ToRemoveEndOfFrame)
        {
            
        }
        m_ToRemoveEndOfFrame.Clear();
    }

    private static void RenderEditorbuffer()
    {
        EditorFrameBuffer.Bind();
        
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.ClearColor(ClearColor.X, ClearColor.Y, ClearColor.Z, ClearColor.W);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
  
        
        var cam = Engine.Get().CurrentScene.GetEditorCamera();
        if (cam == null)
        {
            EditorFrameBuffer.UnBind();
            //TODO:REANABLE THIS
            Log.Error("No Editor camera set!");
            return;
        }
        
        foreach (var batch in Batches)
        {
            batch.Render(cam);
        }
        
        GL.Disable(EnableCap.Blend);
        EditorFrameBuffer.UnBind();
    }

    private static void RenderGamebuffer()
    {
        GameFrameBuffer.Bind();

        
        // GL.Enable(EnableCap.Blend);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        
        var cam = Engine.Get().CurrentScene.GetMainCamera();
        if (cam == null)
        {
            GameFrameBuffer.UnBind();
            return;
        }
        foreach (var batch in Batches)
        {
            batch.Render(Engine.Get().CurrentScene.GetMainCamera());
        }
        
        GL.Disable(EnableCap.Blend);
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
        foreach (var batch in Batches)
        {
            if (batch.CanAdd(ent))
            {
                batch.AddSprite(ent);
                added = true;
            }
        }

        if (!added)
        {
            // Log.Error("Creating new batch");
            var batch = new Batch2D();
            batch.Init(new Shader(
                    "Shaders\\ShaderFiles\\testshader.vert",
                    "Shaders\\ShaderFiles\\testshader.frag"),
                0);
            Batches.Add(batch);
            batch.AddSprite(ent);
        }
    }

    
    static List<Entity> m_ToRemoveEndOfFrame = new();
    public static void RemoveSprite(Entity enttSpriteRenderer)
    {
        m_ToRemoveEndOfFrame.Add(enttSpriteRenderer);
    }
}