#region

using Engine2D.Cameras;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Testing;
using Engine2D.UI;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

#endregion

namespace Engine2D.Rendering;

internal class Renderer
{
    public static int RenderedObjects = 0;
    public static int DrawCalls = 0;
    public static bool DebugRender = false;
    public static bool RenderEverything = true;
    public static bool RenderAnything = true;
    
    internal List<RenderBatch> RenderBatches { get; private set; } = new();
    internal List<RenderBatch> RenderBatchesToRemoveEndOfFrame { get; set; } = new();
    
    private static readonly Dictionary<int, RenderBatch> _spriteBatchDict = new();
    private readonly int _maxLights = 250;
    
   
    
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
        RenderBatches.Clear();
        _spriteBatchDict.Clear();

        //Create light data
        _lightMapRenderer = new LightMapRenderer();
        _lightMapRenderer.Init();
        _pointLights = new List<PointLightComponent>();


        //Create frame buffers
        GameBuffer = new TestFrameBuffer(Engine.Get().Size);
    }

    internal void Render()
    {
        if(!RenderAnything) return;
        RenderedObjects = 0;
        Camera gameCamera = Engine.Get().CurrentScene.GetMainCamera();
        
        RenderBatchesToRemoveEndOfFrame = new();
        
        DrawCalls = 0;
        if(RenderEverything || !Settings.s_IsEngine){
            {
                // //Render Lights
                // {
                //     GL.ClearColor(0, 0, 0, 0);
                //     LightmapTexture = _lightMapRenderer.Render(this, gameCamera);
                // }

                if (Settings.s_IsEngine)
                    GameBuffer.Bind();

                if (gameCamera != null)
                    GL.ClearColor(gameCamera.ClearColor.X / 255, gameCamera.ClearColor.Y / 255,
                        gameCamera.ClearColor.Z / 255,
                        gameCamera.ClearColor.W / 255);

                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);

                foreach (var batch in RenderBatches)
                {
                    batch.Render(gameCamera, this);
                }

                if (Settings.s_IsEngine)
                    GameBuffer.UnBind();
            }
            
            if (Settings.s_IsEngine && UiRenderer.CurrentEditorViewport.IsFocused())
            {
                {
                    
                    Camera editorCamera = Engine.Get().CurrentScene.GetEditorCamera();
                    if (editorCamera == null) return;
                    //Render Lights
                    // {
                    //     GL.ClearColor(0, 0, 0, 0);
                    //     LightmapTexture = _lightMapRenderer.Render(this, editorCamera);
                    // }

                    //Render the scene
                    {
                        if (editorCamera == null || EditorGameBuffer == null) return;

                        if (Settings.s_IsEngine)
                            EditorGameBuffer.Bind();

                        if (gameCamera != null)
                            GL.ClearColor(gameCamera.ClearColor.X / 255, gameCamera.ClearColor.Y / 255,
                                gameCamera.ClearColor.Z / 255,
                                gameCamera.ClearColor.W / 255);


                        GL.Clear(ClearBufferMask.ColorBufferBit);
                        GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);

                        foreach (var batch in RenderBatches)
                        {
                            batch.Render(editorCamera, this);
                        }

                        if (DebugRender)
                            DebugDraw.Render(editorCamera);

                        if (Settings.s_IsEngine)
                            EditorGameBuffer.UnBind();
                    }
                }
            }
        }
        //RemoveOldBatches();
    }

   

    private void RemoveOldBatches()
    {
        foreach (var batch in RenderBatchesToRemoveEndOfFrame)
        {
            RenderBatches.Remove(batch);
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
        if(spr.Parent == null) return;
        
        if(_spriteBatchDict.ContainsKey(spr.Parent.UID))
            if (spr.Parent != null)
            {
                RemoveSprite(spr.Parent);
            }

        foreach (var batch in RenderBatches)
        {
            if (batch.HasRoom && batch.ZIndex == spr.ZIndex && (batch.HasTexture(spr)))
            {
                added = true;
                batch.AddSprite(spr);
                addedToBatch = batch;
            }
        }

        if (!added)
        {
            var batch = new RenderBatch(spr.ZIndex);
            addedToBatch = batch;
            batch.Init();
            batch.AddSprite(spr);
            RenderBatches.Add(batch);
            RenderBatches.Sort();
        }

        if(!_spriteBatchDict.ContainsKey(spr.Parent.UID))
            _spriteBatchDict.Add(spr.Parent.UID, addedToBatch);
    }
    
    public void RemoveSprite(Gameobject go)
    {
        if (go == null) return;
        if (go.GetComponent<SpriteRenderer>() == null) return;
        foreach (RenderBatch batch in RenderBatches) {
            if (batch.DestroyIfExists(go))
            {
                _spriteBatchDict.Remove(go.UID);
                return;
            }
        }
    }
    
    public void RemoveSprite(SpriteRenderer go)
    {
        if (go == null) return;
        foreach (RenderBatch batch in RenderBatches) {
            if (batch.DestroyIfExists(go))
            {
                return;
            }
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

    internal void OnGui()
    {
        for (int i = 0; i < RenderBatches.Count; i++)
        {
            RenderBatchWindow(RenderBatches[i], i);
        }
    }

    private void RenderBatchWindow(RenderBatch batch, int index)
    {
        ImGui.Begin("Batch " + index);
        ImGui.Text("ZIndex: " + batch.ZIndex);
        // ImGui.Text("Sprites: " + batch.SpriteCount);
        // ImGui.Text("Free texture room" + batch.GetTextureRoom());
        ImGui.End();
    }
}