using System.Drawing;
using System.Numerics;
using Box2DSharp.Common;
using Engine2D.Components;
using Engine2D.GameObjects;
using Engine2D.Testing;
using ImGuiNET;
using KDBEngine.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;

namespace Engine2D.Rendering;

internal static class Renderer
{
    //  internal static OrthographicCamera S_CurrentCamera = new(0,0);
    private static List<RenderBatch> _renderBatches = new();
    private static readonly Dictionary<SpriteRenderer, RenderBatch> _spriteBatchDict = new();
    
    
    public static GlobalLight GlobalLight { get; private set; }
    
    #region Debugging
    private static int _drawCalls = 0;
    #endregion

    private static LightMapRenderer _lightMapRenderer = new LightMapRenderer();

    public static Texture LightmapTexture = null;
    public static TestFrameBuffer GameBuffer = null;
    
    internal static void Init()
    {
        Flush();
    }

    
    internal static void Flush()
    {
        _renderBatches.Clear();
        _spriteBatchDict.Clear();
        _lightMapRenderer = new LightMapRenderer();
        GameBuffer = new TestFrameBuffer(Engine.Get().Size);
        _lightMapRenderer.Init();
        PointLights = new();
        GlobalLight = null;
    }
    
    internal static void Render()
    {
        _drawCalls = 0;
        //Render Lights
        {
            LightmapTexture = _lightMapRenderer.Render();
            _lightMapRenderer.BindLightMap();
        }
        //Render the scene
        {
            GameBuffer.Bind();
            GL.ClearColor(0,0,0,0);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            OpenTK.Graphics.OpenGL.GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
            foreach (var batch in _renderBatches)
            {
                batch.Render(Engine.Get().testCamera);
            }
            GameBuffer.UnBind();
        }
        
    }
    
    

    internal static void Update(double dt)
    {
    }

    internal static void OnClose()
    {
    }

    internal static void OnResize(ResizeEventArgs e)
    {
        _lightMapRenderer.Resize();
        GameBuffer = new TestFrameBuffer(Engine.Get().Size);
    }

    internal static List<PointLight> PointLights = new();

    internal static void AddPointLight(PointLight light)
    {
        if (PointLights.Count >= 11) return;
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
                // _spriteBatchDict.Add(spr, batch);
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
            ImGui.Text("Draw calls: " + _drawCalls);
            ImGui.Text("Render Batches: " + _renderBatches.Count);
        };
    }
    



}