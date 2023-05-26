using System.Numerics;
using Engine2D.Components;
using Engine2D.GameObjects;
using Engine2D.Testing;
using ImGuiNET;
using ImGuizmoNET;
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
    public TestFrameBuffer EditorGameBuffer;

    private List<PointLight> _pointLights = new();
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
        _pointLights = new List<PointLight>();
        
        
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
            LightmapTexture = _lightMapRenderer.Render(this, editorCamera);
        }
        //Render the scene
        {
            EditorGameBuffer.Bind();
            
            GL.ClearColor(1, 1, 1, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            OpenTK.Graphics.OpenGL.GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
            foreach (var batch in _renderBatches) batch.Render(editorCamera, LightmapTexture);
            AddGridLines(editorCamera);
            _debugDraw.Render(editorCamera);
            
            EditorGameBuffer.UnBind();
        }
        
        if(gameCamera != null)
        {
            _drawCalls = 0;
            //Render Lights
            {
                LightmapTexture = _lightMapRenderer.Render(this, gameCamera);
            }
            //Render the scene
            {
                GameBuffer.Bind();

                GL.ClearColor(0, 0, 0, 0);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                OpenTK.Graphics.OpenGL.GL.Enable(EnableCap.Blend);
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
        Vector2 cameraPos = camera.getPosition();
        Vector2 projectionSize = new(camera.projectionSize.X, camera.projectionSize.Y);
        
        // float firstY = ((int)Math.Floor((cameraPos.Y)/ GRID_HEIGHT)) * GRID_HEIGHT;
        // float firstX = ((int)Math.Floor((cameraPos.X)/ GRID_WIDTH)) * GRID_HEIGHT;

        float firstX = ((int)Math.Floor(cameraPos.X / GRID_WIDTH) * GRID_HEIGHT  ) - projectionSize.X*camera.zoom;
        float firstY = ((int)Math.Floor(cameraPos.Y / GRID_HEIGHT) * GRID_HEIGHT ) - projectionSize.Y*camera.zoom;
        
        int numVtLines = (int)(projectionSize.X * camera.zoom / GRID_WIDTH) + 2;
        int numHzLines = (int)(projectionSize.Y * camera.zoom/ GRID_HEIGHT) + 2;
        
        float width = (int)((projectionSize.X * camera.zoom) + (5 * GRID_WIDTH)) ;
        float height = (int)(projectionSize.Y * camera.zoom) + (5 * GRID_HEIGHT) ;
        
        width *= 2;
        height *= 2;
        numVtLines *= 2;
        numHzLines *= 2;

        int maxLines = Math.Max(numVtLines, numHzLines);
        
        SpriteColor color = new SpriteColor(.2f, 0.2f, 0.2f, 1);
       
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