using System.Numerics;
using Engine2D.Components;
using Engine2D.GameObjects;
using Engine2D.Testing;
using ImGuiNET;
using ImGuizmoNET;
using KDBEngine.Core;
using Octokit;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

namespace Engine2D.Rendering;

public class Renderer
{
    public Texture LightmapTexture;
    
    public TestFrameBuffer GameBuffer;
    public TestFrameBuffer EditorGameBuffer;

    private static readonly List<RenderBatch> _renderBatches = new();
    private static readonly Dictionary<SpriteRenderer, RenderBatch> _spriteBatchDict = new();

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
        EditorGameBuffer = new TestFrameBuffer(Engine.Get().Size);
        GameBuffer = new TestFrameBuffer(Engine.Get().Size);

        _debugDraw = new DebugDraw();
        _debugDraw.Init();
    }

    internal void Render(TestCamera editorCamera, TestCamera gameCamera)
    {
        _drawCalls = 0;
        //Render Lights
        {
            GL.ClearColor(1,1,1,1);
            LightmapTexture = _lightMapRenderer.Render(this, editorCamera);
        }
        //Render the scene
        {
            EditorGameBuffer.Bind();

            if (gameCamera != null)
            {
                GL.ClearColor(gameCamera.ClearColor.R, gameCamera.ClearColor.G,gameCamera.ClearColor.B,gameCamera.ClearColor.A);
            }
            
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Disable(EnableCap.Blend);
            AddGridLines(editorCamera);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
            foreach (var batch in _renderBatches) batch.Render(editorCamera, LightmapTexture);
            
            _debugDraw.Render(editorCamera);
            
            EditorGameBuffer.UnBind();
        }
        
        if(gameCamera != null)
        {
            _drawCalls = 0;
            //Render Lights
            {
                GL.ClearColor(1,1,1,1);
                LightmapTexture = _lightMapRenderer.Render(this, gameCamera);
            }
            //Render the scene
            {
                GameBuffer.Bind();
                GL.ClearColor(gameCamera.ClearColor.R, gameCamera.ClearColor.G,gameCamera.ClearColor.B,gameCamera.ClearColor.A);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
                foreach (var batch in _renderBatches) batch.Render(gameCamera, LightmapTexture);

                GameBuffer.UnBind();
            }
        }

        
        
    }

    public static float Y = 0;
    internal void Update(double dt)
    {
    }

    private void AddGridLines(TestCamera camera)
    {
        float GRID_WIDTH =32f;
        float GRID_HEIGHT =32f;
        
        Vector2 cameraPos = camera.Parent.Transform.Position;
        Vector2 projectionSize = new(camera.ProjectionSize.X, camera.ProjectionSize.Y);
        
        // float firstY = ((int)Math.Floor((cameraPos.Y)/ GRID_HEIGHT)) * GRID_HEIGHT;
        // float firstX = ((int)Math.Floor((cameraPos.X)/ GRID_WIDTH)) * GRID_HEIGHT;

        float firstX = (float)(((int)Math.Floor(cameraPos.X / GRID_WIDTH) * GRID_HEIGHT  ) - projectionSize.X*camera.Zoom);
        float firstY = (float)(((int)Math.Floor(cameraPos.Y / GRID_HEIGHT) * GRID_HEIGHT ) - projectionSize.Y*camera.Zoom);
        
        int numVtLines = (int)(projectionSize.X * camera.Zoom / GRID_WIDTH) + 2;
        int numHzLines = (int)(projectionSize.Y * camera.Zoom/ GRID_HEIGHT) + 2;
        
        float width = (int)((projectionSize.X * camera.Zoom) + (5 * GRID_WIDTH)) ;
        float height = (int)(projectionSize.Y * camera.Zoom) + (5 * GRID_HEIGHT) ;
        
        width *= 2;
        height *= 2;
        numVtLines *= 2;
        numHzLines *= 2;

        int maxLines = Math.Max(numVtLines, numHzLines);
        
        KDBColor color = new KDBColor(0, 0, 0, 255);
       
        for (int i=0; i < maxLines; i++) {
            float x = firstX + (GRID_WIDTH * i);
            float y = firstY + (GRID_HEIGHT * i);

            if (i < numVtLines) {
                _debugDraw.AddLine2D(new (x, firstY), new (x, firstY + height), color, camera);
            }

            if (i < numHzLines) {
                _debugDraw.AddLine2D(new (firstX, y), new (firstX + width, y), color, camera);
            }
        }
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