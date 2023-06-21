#region

using Engine2D.Cameras;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Testing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

#endregion

namespace Engine2D.Rendering;

internal class Renderer
{
    private static readonly List<RenderBatch> _renderBatches = new();
    private static readonly Dictionary<int, RenderBatch> _spriteBatchDict = new();
    private readonly int _maxLights = 250;

    #region Debugging

    private int _drawCalls;

    #endregion

    private LightMapRenderer _lightMapRenderer = new();

    private List<PointLightComponent> _pointLights = new();
    private int LightmapTexture;

    internal TestFrameBuffer? GameBuffer { get; private set; }
    internal TestFrameBuffer? EditorGameBuffer { get; set; }

    internal GlobalLight GlobalLight { get; set; }

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
    }

    internal void Render(Camera editorCamera, Camera gameCamera)
    {
        _drawCalls = 0;

        if (Settings.s_RenderDebugWindowSeperate)
        {
            if (editorCamera == null) return;
            //Render Lights
            {
                GL.ClearColor(0, 0, 0, 0);
                LightmapTexture = _lightMapRenderer.Render(this, editorCamera);
            }

            //Render the scene
            {
                if (editorCamera == null) return;

                if (Settings.s_IsEngine)
                    EditorGameBuffer.Bind();
                else
                    Engine.Get().Title = "EDITOR";


                if (gameCamera != null)
                    GL.ClearColor(gameCamera.ClearColor.RNormalized, gameCamera.ClearColor.GNormalized,
                        gameCamera.ClearColor.BNormalized,
                        gameCamera.ClearColor.ANormalized);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Disable(EnableCap.Blend);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);

                foreach (var batch in _renderBatches) batch.Render(editorCamera, LightmapTexture);

                if (Settings.s_IsEngine)
                    EditorGameBuffer.UnBind();
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

        if (_spriteBatchDict.ContainsKey(spr.Parent.UID)) return;
        _spriteBatchDict.Add(spr.Parent.UID, addedToBatch);
    }

    internal void RemoveSprite(SpriteRenderer spr)
    {
        if (_spriteBatchDict.ContainsKey(spr.Parent.UID))
        {
            _spriteBatchDict[spr.Parent.UID].RemoveSprite(spr);
            _spriteBatchDict.Remove(spr.Parent.UID);
        }
    }

    internal List<PointLightComponent> GetPointLightsToRender()
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