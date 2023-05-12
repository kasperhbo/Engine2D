using Engine2D.GameObjects;
using KDBEngine.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

namespace Engine2D.Rendering;

internal static class GameRenderer
{
    //  internal static OrthographicCamera S_CurrentCamera = new(0,0);
    private static List<RenderBatch> _renderBatches = new();

    private static readonly Dictionary<SpriteRenderer, RenderBatch> _spriteBatchDict = new();


    internal static void Init()
    {
        Flush();

        _renderBatches = new List<RenderBatch>();

        // Calculate the aspect ratio

        // Calculate the size of the camera
        //S_CurrentCamera = new OrthographicCamera(Engine.Get().TargetAspectRatio, 1);

        foreach (var rb in _renderBatches) rb.Init();
    }

    internal static void Flush()
    {
        _renderBatches.Clear();
        _spriteBatchDict.Clear();
        //S_CurrentCamera = null;
    }

    internal static void Render()
    {
        var col = Engine.Get()._currentScene.LightSettings.ClearColor;
        GL.ClearColor(col.Color.X, col.Color.Y, col.Color.Z, col.Color.W);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.Enable(EnableCap.Blend);

        GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);

        foreach (var rb in _renderBatches)
            rb.Render(Engine.Get().testCamera.getProjectionMatrix(), Engine.Get().testCamera.getViewMatrix());
    }

    internal static void Update(double dt)
    {
    }

    internal static void OnClose()
    {
    }

    internal static void OnResize(ResizeEventArgs size)
    {
    }

    internal static void AddSpriteRenderer(SpriteRenderer spr)
    {
        var added = false;
        RenderBatch addedToBatch = null;
        foreach (var batch in _renderBatches)
            if (batch.HasRoom)
            {
                added = true;
                batch.AddSprite(spr);
                addedToBatch = batch;
            }

        if (!added)
        {
            var batch = new RenderBatch();
            batch.Init();
            batch.AddSprite(spr);
            _renderBatches.Add(batch);
            addedToBatch = batch;
        }
    }

    internal static void RemoveSprite(SpriteRenderer spr)
    {
        _spriteBatchDict[spr].RemoveSprite(spr);
        _spriteBatchDict.Remove(spr);
    }
}