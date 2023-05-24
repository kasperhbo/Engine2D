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

<<<<<<< HEAD
    //Editor
    
    private static LightMapRenderer s_lightMapRendererEditor = new();
    public static TestFrameBuffer? GameBufferEditor = null;
    public static TestFrameBuffer? GameBufferGame = null;
    
    //Game
    private static List<PointLight> _pointLights = new();
    private static readonly int _maxLights = 250;

    private static Vector2 _maxLightDistance = new(960, 540);


    public static GlobalLight GlobalLight { get; private set; }
    
=======
    private static LightMapRenderer _lightMapRenderer = new LightMapRenderer();
>>>>>>> parent of efcdaf4... AUTO REFACTORIO

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
<<<<<<< HEAD
        s_lightMapRendererEditor = new LightMapRenderer();
        
        GameBufferEditor = new TestFrameBuffer(Engine.Get().Size);
        GameBufferGame = new TestFrameBuffer(Engine.Get().Size);
        
        s_lightMapRendererEditor.Init();
        _pointLights = new List<PointLight>();
=======
        _lightMapRenderer = new LightMapRenderer();
        GameBuffer = new TestFrameBuffer(Engine.Get().Size);
        _lightMapRenderer.Init();
        PointLights = new();
>>>>>>> parent of efcdaf4... AUTO REFACTORIO
        GlobalLight = null;
    }
    
    internal static void Render()
    {
        _drawCalls = 0;
<<<<<<< HEAD
        
        //IF ENGINE RENDER EDITOR VIEWPORT
        if(Settings.s_IsEngine){
            Texture? LightmapTextureEditor;
            
            //Render Lights
            {
                LightmapTextureEditor = s_lightMapRendererEditor.Render(Engine.Get()._currentScene.EditorCamera);
            }
            //Render the scene
            {
                if (Settings.s_IsEngine)
                    GameBufferEditor.Bind();

                GL.ClearColor(0, 0, 0, 0);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                OpenTK.Graphics.OpenGL.GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
                foreach (var batch in _renderBatches)
                    batch.Render(Engine.Get()._currentScene.EditorCamera, LightmapTextureEditor);

                if (Settings.s_IsEngine)
                    GameBufferEditor.UnBind();
            }
=======
        //Render Lights
        {
            LightmapTexture = _lightMapRenderer.Render();
            _lightMapRenderer.BindLightMap();
>>>>>>> parent of efcdaf4... AUTO REFACTORIO
        }
        
        {
<<<<<<< HEAD
            Texture? LightmapTextureEditor;
            
            if(Engine.Get()._currentScene.GameCamera != null)
            {
                //Render Lights
                {
                    LightmapTextureEditor = s_lightMapRendererEditor.Render(Engine.Get()._currentScene.GameCamera);
                }
                //Render the scene
                {
                    if (Settings.s_IsEngine)
                        GameBufferGame.Bind();

                    GL.ClearColor(0, 0, 0, 0);
                    GL.Clear(ClearBufferMask.ColorBufferBit);
                    OpenTK.Graphics.OpenGL.GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);

                    foreach (var batch in _renderBatches)
                        batch.Render(Engine.Get()._currentScene.GameCamera, LightmapTextureEditor);

                    if (Settings.s_IsEngine)
                        GameBufferGame.UnBind();
                }
            }
=======
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
>>>>>>> parent of efcdaf4... AUTO REFACTORIO
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
        s_lightMapRendererEditor.Resize();
        GameBufferEditor = new TestFrameBuffer(Engine.Get().Size);
        GameBufferGame = new TestFrameBuffer(Engine.Get().Size);
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
    


<<<<<<< HEAD
        for (var i = 0; i < _pointLights.Count; i++)
        {
            var cam = Engine.Get()._currentScene.EditorCamera;
            // float xDist = (cam.position.X + _pointLights[i].Intensity * 100) - (_pointLights[i].LastTransform.position.X);
            // float yDist = cam.position.Y - _pointLights[i].LastTransform.position.Y;

            // if(!IsInRange(new Vector2(
            //        _pointLights[i].LastTransform.position.X +(_pointLights[i].Intensity * 100),
            //        _pointLights[i].LastTransform.position.Y +(_pointLights[i].Intensity * 100)
            //        ), cam.position, _maxLightDistance))
            // {
            pointLights.Add(_pointLights[i]);
            count++;
            //}

            if (count >= _maxLights)
                break;
        }


        return pointLights;
    }

=======

>>>>>>> parent of efcdaf4... AUTO REFACTORIO
}