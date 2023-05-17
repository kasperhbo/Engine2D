﻿using System.Numerics;
using Engine2D.Components;
using Engine2D.Components.Lights;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Scenes;
using Engine2D.Testing;
using KDBEngine.Core;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

namespace Engine2D.Rendering;

internal static class GameRenderer
{
    //  internal static OrthographicCamera S_CurrentCamera = new(0,0);
    private static List<RenderBatch> _renderBatches = new();
    private static List<SpriteLightRenderBatch> _lightRenderBatches = new();

    private static readonly Dictionary<SpriteRenderer, RenderBatch> _spriteBatchDict = new();
    private static readonly Dictionary<SpriteRenderer, SpriteLightRenderBatch> _lightRenderBatchesDict = new();

    public static TestFrameBuffer FrameBuffer { get; private set; }
    public static TestFrameBuffer LightFrameBuffer { get; private set; }
    public static TestFrameBuffer SceneFrameBuffer { get; private set; }
    public static int DrawCalls { get; private set; }
    public static int RenderBatches => _renderBatches.Count;
    
    private static readonly float[] _vertices =
    {
        // Position         Texture coordinates
        1f,  1f, 0.0f, 1.0f, 1.0f, // top right
        1f, -1f, 0.0f, 1.0f, 0.0f, // bottom right
        -1f, -1f, 0.0f, 0.0f, 0.0f, // bottom left
        -1f,  1f, 0.0f, 0.0f, 1.0f  // top left
    };

    private static readonly uint[] _indices =
    {
        0, 1, 3,
        1, 2, 3
    };
    
    private static int _vertexBufferObject;
    private static int _vertexArrayObject;
    private static int _elementBufferObject;
    
    internal static void Init()
    {
        Flush();

        _renderBatches = new List<RenderBatch>();
        _lightRenderBatches = new List<SpriteLightRenderBatch>();

        FrameBuffer = new TestFrameBuffer(Engine.Get().ClientSize.X, Engine.Get().ClientSize.Y);
        LightFrameBuffer = new TestFrameBuffer(Engine.Get().ClientSize.X, Engine.Get().ClientSize.Y);
        SceneFrameBuffer = new TestFrameBuffer(Engine.Get().ClientSize.X, Engine.Get().ClientSize.Y);
        
        var dat = new ShaderData();
        dat.VertexPath = Utils.GetBaseEngineDir() + "/Shaders/Lighting/FrameBuffer.vert";
        dat.FragPath = Utils.GetBaseEngineDir() + "/Shaders/Lighting/FrameBuffer.frag";

        _frameShader = ResourceManager.GetShader(dat);
        
        foreach (var rb in _lightRenderBatches) rb.Init();
        foreach (var rb in _renderBatches) rb.Init();
        
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
        _frameShader.use();
        var vertexLocation = _frameShader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        var texCoordLocation = _frameShader.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        
        SceneFrameBuffer.Texture.Use(TextureUnit.Texture0);
        LightFrameBuffer.Texture.Use(TextureUnit.Texture1);
        
        _frameShader.uploadInt("sceneTexture", 0);
        _frameShader.uploadInt("lightTexture", 1);
    }

    private static Shader _frameShader;
    
    internal static void Flush()
    {
        _renderBatches.Clear();
        _lightRenderBatches.Clear();
        _spriteBatchDict.Clear();
        _lightRenderBatchesDict.Clear();
    }
   
    
    internal static void Render()
    {
        var col = Engine.Get()._currentScene.LightSettings.ClearColor;
        DrawCalls = 0;
        LightFrameBuffer.Bind();
        
        GL.ClearColor(0,0,0,0);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.One, BlendingFactor.DstAlpha);
        
        foreach (var rb in _lightRenderBatches)
        {
            DrawCalls++;
            rb.Render(Engine.Get().testCamera.getProjectionMatrix(), Engine.Get().testCamera.getViewMatrix());
        }
        
        LightFrameBuffer.UnBind();
        
        SceneFrameBuffer.Bind();
        // GL.ClearColor(col.Color.X, col.Color.Y, col.Color.Z, col.Color.W);
        
        GL.ClearColor(0,0,0,0);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);

        List<RenderBatch> batchedToRemove = new List<RenderBatch>();

        foreach (var rb in _renderBatches)
        {
            bool removed = false;
            if (rb.SpriteCount <= 0)
            {
                removed = true;
                batchedToRemove.Add(rb);
            }

            if(!removed)
            {
                DrawCalls++;

                rb.Render(Engine.Get().testCamera.getProjectionMatrix(), Engine.Get().testCamera.getViewMatrix());
            }
        }

        foreach (var batchToRemove in batchedToRemove)
        {
            _renderBatches.Remove(batchToRemove);
        }
        
        SceneFrameBuffer.UnBind();
        
        FrameBuffer.Bind();
        GL.ClearColor(0,0,0,0);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        SceneFrameBuffer.Texture.Use(TextureUnit.Texture0);
        LightFrameBuffer.Texture.Use(TextureUnit.Texture1);
        
        _frameShader.use();
        
        GL.BindVertexArray(_vertexArrayObject);
        
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        //COMBINE TWO TEXTURES
        _frameShader.use();
        DrawCalls++;
        FrameBuffer.UnBind();

    }

    internal static void Update(double dt)
    {
    }

    internal static void OnClose()
    {
    }

    internal static void OnResize(ResizeEventArgs e)
    {
        FrameBuffer = new TestFrameBuffer(e.Size.X, e.Size.Y);
        LightFrameBuffer = new TestFrameBuffer(e.Size.X, e.Size.Y);
        SceneFrameBuffer = new TestFrameBuffer(e.Size.X, e.Size.Y);
    }

    internal static void AddSpriteRenderer(SpriteRenderer spr)
    {
        var added = false;
        RenderBatch addedToBatch = null;
        foreach (var batch in _renderBatches)
            if (batch.HasRoom && batch.ZIndex == spr.ZIndex)
            {
                added = true;
                _spriteBatchDict.Add(spr, batch);
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
    
    internal static void AddSpriteLightRenderer(SpriteLightRenderer spr)
    {
        var added = false;
        SpriteLightRenderBatch addedToBatch = null;
        foreach (var batch in _lightRenderBatches)
            if (batch.HasRoom)
            {
                added = true;
                batch.AddSprite(spr);
                addedToBatch = batch;
            }

        if (!added)
        {
            var batch = new SpriteLightRenderBatch();
            batch.Init();
            batch.AddSprite(spr);
            _lightRenderBatches.Add(batch);
            addedToBatch = batch;
        }
    }

    internal static void RemoveSprite(SpriteRenderer spr)
    {
        _spriteBatchDict[spr].RemoveSprite(spr);
        _spriteBatchDict.Remove(spr);
    }
}