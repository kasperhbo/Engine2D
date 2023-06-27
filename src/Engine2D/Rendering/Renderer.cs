#region

using System.Diagnostics;
using Engine2D.Cameras;
using Engine2D.Components;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Testing;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

#endregion

namespace Engine2D.Rendering;

internal class Renderer
{
    internal List<RenderBatch> RenderBatches { get; private set; } = new();
    internal List<RenderBatch> RenderBatchesToRemoveEndOfFrame { get; set; } = new();
    
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
        RenderBatches.Clear();
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
        RenderBatchesToRemoveEndOfFrame = new();
        
        _drawCalls = 0;

        {
            //Render Lights
            {
                GL.ClearColor(0, 0, 0, 0);
                LightmapTexture = _lightMapRenderer.Render(this, gameCamera);
            }
            
            if(Settings.s_IsEngine)
                GameBuffer.Bind();

            if (gameCamera != null)
                GL.ClearColor(gameCamera.ClearColor.X / 255, gameCamera.ClearColor.Y / 255,
                    gameCamera.ClearColor.Z / 255,
                    gameCamera.ClearColor.W / 255);
            
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);

            foreach (var batch in RenderBatches) batch.Render(gameCamera, LightmapTexture, this);
            
            if(Settings.s_IsEngine)
                GameBuffer.UnBind();
        }
        
        if(Settings.s_IsEngine)
        {
            if (Settings.s_RenderDebugWindowSeperate)
            {
                {
                    if (editorCamera == null) return;
                    //Render Lights
                    {
                        GL.ClearColor(0, 0, 0, 0);
                        LightmapTexture = _lightMapRenderer.Render(this, editorCamera);
                    }

                    //Render the scene
                    {
                        if (editorCamera == null || EditorGameBuffer == null) return;
                
                        if (Settings.s_IsEngine)
                            EditorGameBuffer.Bind();

                        if (gameCamera != null)
                            GL.ClearColor(editorCamera.ClearColor.X / 255, editorCamera.ClearColor.Y / 255,
                                editorCamera.ClearColor.Z / 255,
                                editorCamera.ClearColor.W / 255);

                        GL.Clear(ClearBufferMask.ColorBufferBit);
                        GL.Disable(EnableCap.Blend);
                        GL.Enable(EnableCap.Blend);
                        GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
                        AddGridLines(editorCamera);
                        foreach (var batch in RenderBatches) batch.Render(editorCamera, LightmapTexture, this);

                        DebugDraw.Render(editorCamera);
                        if (Settings.s_IsEngine)
                            EditorGameBuffer.UnBind();
                    }
                }
            }
        }
        RemoveOldBatches();
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
    
    private void AddGridLines(Camera camera)
    {
        System.Numerics.Vector2 cameraPos = camera.Parent.GetComponent<Transform>().Position;
        System.Numerics.Vector2 projectionSize = camera.ProjectionSize;

        float originalWidth = Settings.GRID_WIDTH;
        float originalHeight = Settings.GRID_HEIGHT;

        float newWidth = originalWidth   * (float)Math.Floor(camera.Size);
        float newHeight = originalHeight * (float)Math.Floor(camera.Size);

        Console.WriteLine((float)Math.Floor(camera.Size));
        
        if (newWidth < originalWidth)
        {
            newWidth = originalWidth;
        }

        if (newHeight < originalHeight)
            newHeight = originalHeight;
            

        float firstX = ((int)Math.Floor(cameraPos.X / newWidth)) * newWidth;
        float firstY = ((int)Math.Floor(cameraPos.Y / newHeight)) * newHeight;

        firstX -= newWidth/2;
        firstY -=newHeight/2;
        
        
        
        
        int numVtLines = (int)(projectionSize.X * camera.Size / newWidth) + 2;
        int numHzLines = (int)(projectionSize.Y * camera.Size / newHeight) + 2;

        float width = (int)(projectionSize.X * camera.Size) + (5 * newWidth);
        float height = (int)(projectionSize.Y * camera.Size) + (5 * newHeight);

        int maxLines = Math.Max(numVtLines, numHzLines);
        OpenTK.Mathematics.Vector4 color = new OpenTK.Mathematics.Vector4(0.2f, 0.2f, 0.2f,1f);
        for (int i=0; i < maxLines; i++) {
            float x = firstX + (newWidth * i);
            float y = firstY + (newHeight * i);

            if (i < numVtLines) {
                DebugDraw.AddLine2D(new OpenTK.Mathematics.Vector2(x, firstY), new OpenTK.Mathematics.Vector2(x, firstY + height), color, camera);
            }

            if (i < numHzLines) {
                DebugDraw.AddLine2D(new OpenTK.Mathematics.Vector2(firstX, y), new OpenTK.Mathematics.Vector2(firstX + width, y), color, camera);
            }
        }
    }
}