using System.Numerics;
using Engine2D.Cameras;
using Engine2D.Components;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Testing;
using ImGuiNET;
using ImGuizmoNET;
using Engine2D.Core;
using Octokit;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

namespace Engine2D.Rendering;

public class Renderer
{
    public int LightmapTexture;
    
    public TestFrameBuffer? GameBuffer{get; private set;}
    public TestFrameBuffer? EditorGameBuffer{get; set;}

    private static readonly List<RenderBatch> _renderBatches = new();
    private static readonly Dictionary<int, RenderBatch> _spriteBatchDict = new();

    #region Debugging

    private int _drawCalls;

    #endregion

    private LightMapRenderer _lightMapRenderer = new();
    
    private List<PointLightComponent> _pointLights = new();
    private readonly int _maxLights = 250;

    public GlobalLight GlobalLight { get; set; }

    private DebugDraw _debugDraw;

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
        _pointLights = new List<PointLightComponent>();
        
        
        //Create frame buffers
        GameBuffer = new TestFrameBuffer(Engine.Get().Size);
        
        _debugDraw = new DebugDraw();
        _debugDraw.Init();
    }

    internal void Render(Camera editorCamera, Camera gameCamera)
    {
        _drawCalls = 0;
        
        if(Settings.s_IsEngine)
        {
            if (editorCamera == null) return;
            //Render Lights
            {
                GL.ClearColor(0,0,0,0);
                LightmapTexture = _lightMapRenderer.Render(this, editorCamera);
            }

            //Render the scene
            {
                if (editorCamera == null) return;
                
                EditorGameBuffer.Bind();
                
                if (gameCamera != null)
                {
                    GL.ClearColor(gameCamera.ClearColor.RNormalized, gameCamera.ClearColor.GNormalized, gameCamera.ClearColor.BNormalized,
                        gameCamera.ClearColor.ANormalized);
                }
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Disable(EnableCap.Blend);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
                
                _debugDraw.Render(editorCamera);
                
                foreach (var batch in _renderBatches) batch.Render(editorCamera, LightmapTexture);
                

                EditorGameBuffer.UnBind();
            }
        }
        
        if(gameCamera != null)
        {
            GL.ClearColor(gameCamera.ClearColor.RNormalized, gameCamera.ClearColor.GNormalized,gameCamera.ClearColor.BNormalized,gameCamera.ClearColor.ANormalized);
            _drawCalls = 0;
            //Render Lights
            {
                LightmapTexture = _lightMapRenderer.Render(this, gameCamera);
            }
            //Render the scene
            {
                if(Settings.s_IsEngine)
                    GameBuffer.Bind();
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
                foreach (var batch in _renderBatches) batch.Render(gameCamera, LightmapTexture);
                if(Settings.s_IsEngine)
                    GameBuffer.UnBind();
            }
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
        EditorGameBuffer = new TestFrameBuffer(Engine.Get().Size);
    }

    internal void AddPointLight(PointLightComponent lightComponent)
    {
        _pointLights.Add(lightComponent);
    }

    internal void AddSpriteRenderer(SpriteRenderer spr)
    {
        var added = false;
        RenderBatch addedToBatch = null;
        
        
        foreach (var batch in _renderBatches)
            if (batch.HasRoom && batch.ZIndex == spr.ZIndex)
            {
                added = true;
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
        
        _spriteBatchDict.Add(spr.Parent.UID, addedToBatch);
    }

    internal void RemoveSprite(SpriteRenderer spr)
    {
        _spriteBatchDict[spr.Parent.UID].RemoveSprite(spr);
        _spriteBatchDict.Remove(spr.Parent.UID);
    }

    public Action GetDebugGUI()
    {
        return NewDebugUI();
    }

    private Action NewDebugUI()
    {
        return () =>
        {
            ImGui.Text("Draw calls: " + _drawCalls);
            ImGui.Text("Render Batches: " + _renderBatches.Count);
        };
    }

    public List<PointLightComponent> GetPointLightsToRender()
    {
        var pointLights = new List<PointLightComponent>();
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