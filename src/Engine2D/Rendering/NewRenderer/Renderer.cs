using System.Numerics;
using Engine2D.Components.Cameras;
using Engine2D.Components.ENTT;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.Testing;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.Rendering.NewRenderer;

internal static class Renderer
{
    internal static TestFrameBuffer GameFrameBuffer   = new(1920, 1080);
    internal static TestFrameBuffer EditorFrameBuffer = new(1920, 1080);

    internal static List<Batch2D> Batches = new();
    internal static Vector4 ClearColor = new(.2F, .2F, .2F, 1.0f);

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
        if(Settings.s_IsEngine)
        {
            RenderEditorbuffer();
            RenderGamebuffer();
        }
        else
        {
            RenderScene();
        }
        m_ToRemoveEndOfFrame.Clear();
    }

    private static void RenderEditorbuffer()
    {
        EditorFrameBuffer.Bind();
        RenderScene();
        EditorFrameBuffer.UnBind();
    }

    private static void RenderGamebuffer()
    {
        GameFrameBuffer.Bind();
        RenderScene();
        GameFrameBuffer.UnBind();
    }

    private static void RenderScene()
    {
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
    }

    internal static void Resize()
    {
        GameFrameBuffer = new TestFrameBuffer(Engine.Get().Size);
        EditorFrameBuffer = new TestFrameBuffer(Engine.Get().Size);
    }

    public static bool DestroyEntity(Entity ent)
    {
        bool found = false;

        foreach (var batch in Batches)
        {
            if (batch.DestroyIfExists(ent))
            {
                found = true;
            }
        }
        
        return found;
    }

    public static void AddSprite(Entity ent)
    {
        bool added = false;
        
        for (int i = 0; i < Batches.Count; i++)
        {
            var batch = Batches[i];
            if (batch.CanAdd(ent))
            {
                batch.AddSprite(ent);
                ent.AddedToBatch = i + 1;
                added = true;
            }
        }

        if (!added)
        {
            // Log.Error("Creating new batch");
            var batch = new Batch2D();
            ShaderData data = new()
            {
                VertexPath = "Shaders\\ShaderFiles\\testshader.vert",
                FragPath = "Shaders\\ShaderFiles\\testshader.frag"
            };
            
            batch.Init(ResourceManager.GetShader(data), 0);
            
            Batches.Add(batch);
            ent.AddedToBatch = Batches.Count;
            batch.AddSprite(ent);
        }
    }

    
    static List<Entity> m_ToRemoveEndOfFrame = new();
    public static void RemoveSprite(Entity enttSpriteRenderer)
    {
        m_ToRemoveEndOfFrame.Add(enttSpriteRenderer);
    }
}