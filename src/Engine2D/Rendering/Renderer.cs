using System.Numerics;
using Engine2D.Components;
using Engine2D.GameObjects;
using Engine2D.Testing;
using ImGuiNET;
using KDBEngine.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;

namespace Engine2D.Rendering;

public class Renderer
{
    //  internal static OrthographicCamera S_CurrentCamera = new(0,0);
    private static readonly List<RenderBatch> _renderBatches = new();
    private static readonly Dictionary<SpriteRenderer, RenderBatch> _spriteBatchDict = new();

    #region Debugging

    private int _drawCalls;

    #endregion

    private LightMapRenderer _lightMapRenderer = new();

    public Texture LightmapTexture;
    
    public TestFrameBuffer GameBuffer;

    private List<PointLight> _pointLights = new();
    private readonly int _maxLights = 250;

    public GlobalLight GlobalLight { get; set; }

    internal void Init()
    {
        Flush();
    }

    internal void Flush()
    {
        _renderBatches.Clear();
        _spriteBatchDict.Clear();
        
        //Create light data
        _lightMapRenderer = new LightMapRenderer();
        _lightMapRenderer.Init();
        _pointLights = new List<PointLight>();
        
        
        //Create frame buffers
        GameBuffer = new TestFrameBuffer(Engine.Get().Size);
    }

    internal void Render(TestCamera camera)
    {
        _drawCalls = 0;
        //Render Lights
        {
            LightmapTexture = _lightMapRenderer.Render(this, camera);
        }
        //Render the scene
        {
            GameBuffer.Bind();
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            OpenTK.Graphics.OpenGL.GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
            foreach (var batch in _renderBatches) batch.Render(camera, LightmapTexture);
            GameBuffer.UnBind();
        }
        
        
    }


    internal void Update(double dt)
    {
    }

    internal void OnClose()
    {
    }

    internal void OnResize(ResizeEventArgs e)
    {
        _lightMapRenderer.Resize();
        GameBuffer = new TestFrameBuffer(Engine.Get().Size);
    }

    internal void AddPointLight(PointLight light)
    {
        _pointLights.Add(light);
    }

    internal void AddSpriteRenderer(SpriteRenderer spr)
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
            addedToBatch = batch;
            batch.Init();
            batch.AddSprite(spr);
            _renderBatches.Add(batch);
            _renderBatches.Sort();
        }

        _spriteBatchDict.Add(spr, addedToBatch);
    }

    internal void RemoveSprite(SpriteRenderer spr)
    {
        _spriteBatchDict[spr].RemoveSprite(spr);
        _spriteBatchDict.Remove(spr);
    }

    public Action GetDebugGUI()
    {
        return NewDebugUI();
    }

    private Action NewDebugUI()
    {
        return () =>
        {
            var windowSize = TestViewportWindow.getLargestSizeForViewport() / 3;
            ImGui.Text("Draw calls: " + _drawCalls);
            ImGui.Text("Render Batches: " + _renderBatches.Count);
        };
    }

    public List<PointLight> GetPointLightsToRender()
    {
        var pointLights = new List<PointLight>();
        var count = 0;

        for (var i = 0; i < _pointLights.Count; i++)
        {
            pointLights.Add(_pointLights[i]);
            count++;
            if (count >= _maxLights)
                break;
        }


        return pointLights;
    }
}