﻿using System.Numerics;
using Engine2D.Components;
using Engine2D.GameObjects;
using Engine2D.Testing;
using ImGuiNET;
using KDBEngine.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;

namespace Engine2D.Rendering;

internal static class Renderer
{
    //  internal static OrthographicCamera S_CurrentCamera = new(0,0);
    private static readonly List<RenderBatch> _renderBatches = new();
    private static readonly Dictionary<SpriteRenderer, RenderBatch> _spriteBatchDict = new();

    #region Debugging

    private static int _drawCalls;

    #endregion

    private static LightMapRenderer _lightMapRenderer = new();

    public static Texture LightmapTexture;
    public static TestFrameBuffer GameBuffer;

    private static List<PointLight> _pointLights = new();
    private static readonly int _maxLights = 250;

    private static Vector2 _maxLightDistance = new(960, 540);


    public static GlobalLight GlobalLight { get; private set; }

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
        _pointLights = new List<PointLight>();
        GlobalLight = null;
    }

    internal static void Render()
    {
        _drawCalls = 0;
        //Render Lights
        {
            LightmapTexture = _lightMapRenderer.Render();
        }
        //Render the scene
        {
            GameBuffer.Bind();
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            OpenTK.Graphics.OpenGL.GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
            foreach (var batch in _renderBatches) batch.Render(Engine.Get().testCamera, LightmapTexture);
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

    internal static void AddPointLight(PointLight light)
    {
        _pointLights.Add(light);
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
            addedToBatch = batch;
            batch.Init();
            batch.AddSprite(spr);
            _renderBatches.Add(batch);
            _renderBatches.Sort();
        }

        _spriteBatchDict.Add(spr, addedToBatch);
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

    public static List<PointLight> GetPointLightsToRender()
    {
        var pointLights = new List<PointLight>();
        var count = 0;

        for (var i = 0; i < _pointLights.Count; i++)
        {
            var cam = Engine.Get().testCamera;
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
}