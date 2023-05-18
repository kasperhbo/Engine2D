using Engine2D.Components;
using Engine2D.GameObjects;
using Engine2D.Testing;
using ImGuiNET;
using KDBEngine.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine2D.Rendering;

internal static class GameRenderer
{
    //  internal static OrthographicCamera S_CurrentCamera = new(0,0);
    private static List<RenderBatch> _renderBatches = new();
    private static readonly Dictionary<SpriteRenderer, RenderBatch> _spriteBatchDict = new();
    private static TestFrameBuffer? _frameBuffer;
    
    public static GlobalLight GlobalLight { get; private set; }
    
    #region Debugging
    private static int _drawCalls = 0;
    #endregion
    
    public static int FrameBufferToRenderer()
    {
        if (_frameBuffer != null) return _frameBuffer.GetTextureID;
        return -1;
    }

    internal static void Init()
    {
        _frameBuffer = new TestFrameBuffer(Engine.Get().ClientSize.X, Engine.Get().ClientSize.Y);
        Flush();
        GL.ClearColor(1,1,1,1);
        
    }

    internal static void Flush()
    {
        _renderBatches.Clear();
        _spriteBatchDict.Clear();
        PointLights = new();
        GlobalLight = null;
    }
    internal static void Render()
    {
        
        
        _drawCalls = 0;
        _frameBuffer?.Bind();
        
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        foreach (var batch in _renderBatches)
        {
            _drawCalls++;
            batch.Render(Engine.Get().testCamera.getProjectionMatrix(), Engine.Get().testCamera.getViewMatrix());
        }
        _frameBuffer?.UnBind();
    }

    internal static void Update(double dt)
    {
    }

    internal static void OnClose()
    {
    }

    internal static void OnResize(ResizeEventArgs e)
    {
        _frameBuffer = new TestFrameBuffer(e.Size.X, e.Size.Y);
    }

    internal static List<PointLight> PointLights = new();

    internal static void AddGlobalLight(GlobalLight light)
    {
        GlobalLight = light;
    }
    
    internal static void AddPointLight(PointLight light)
    {
        if (PointLights.Count >= 300) return;
        PointLights.Add(light);
    }

    internal static void AddSpriteRenderer(SpriteRenderer spr)
    {
        var added = false;
        RenderBatch addedToBatch = null;
        foreach (var batch in _renderBatches)
            if (batch.HasRoom && batch.ZIndex == spr.ZIndex)
            {
                added = true;
                _spriteBatchDict.Add(spr, batch);
                batch.AddSprite(spr);
                addedToBatch = batch;
            }

        if (!added)
        {
            var batch = new RenderBatch(spr.ZIndex);
            _spriteBatchDict.Add(spr, batch);
            batch.Init();
            batch.AddSprite(spr);
            _renderBatches.Add(batch);
            _renderBatches.Sort();
        }
    }
    
    internal static void RemoveSprite(SpriteRenderer spr)
    {
        _spriteBatchDict[spr].RemoveSprite(spr);
        _spriteBatchDict.Remove(spr);
    }

    public static Action GetDebugGUI()
    {
        return NewDebugUI();
    }

    private static Action NewDebugUI()
    {
        return () =>
        {
            var windowSize = TestViewportWindow.getLargestSizeForViewport() / 3;
            ImGui.Text("Draw calls: " + GameRenderer._drawCalls);
            ImGui.Text("Render Batches: " + _renderBatches.Count);
            // ImGui.Text("Light Frame Buffer");
            // // ImGui.Image((IntPtr)GameRenderer.LightFrameBuffer.GetTextureID, new Vector2(windowSize.X, windowSize.Y),
            // //     new Vector2(0, 1), new Vector2(1, 0));
        };
    }
    



}