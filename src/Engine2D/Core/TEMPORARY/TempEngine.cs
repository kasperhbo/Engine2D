using Engine2D.Components;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Engine2D.Core;

public class TempEngine : GameWindow
{
    public TempEngine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
    {
    }

    public TempCamera camera;
    private static TempEngine _instance;
    public static TempEngine Get => _instance;
    
    TestRenderer t;
    Texture wrapTexture;
    // Gameobject pepper = new Gameobject("Pepper", new Transform((1080/2)-(720/2), 200, 200, 200));
    Gameobject two = new Gameobject("Two", new Transform((1080/2)-(720/2), 200, 100, 100));
    Gameobject light1 = new Gameobject("Light", new Transform(460, 360, 50, 50));
    Gameobject light2 = new Gameobject("Light", new Transform(620, 360, 50, 50));
    
    public DefaultRenderer renderer = new DefaultRenderer();
    
    protected override void OnLoad()
    {
        base.OnLoad();
        
        _instance = this;
        camera = new TempCamera();
        
        t = new TestRenderer();
        t.Init();
        t.Render();
        
        wrapTexture = Texture.Wrap(t.FetchColorAttachment(0));

        SpriteRenderer spr2 = new SpriteRenderer();
        TextureData texture = new TextureData(
            Utils.GetBaseEngineDir() + "/Images/testImage.png", false,TextureMinFilter.Linear, TextureMagFilter.Linear);
        spr2.texture = ResourceManager.GetTexture(texture);
        two.AddComponent(spr2);
        var pl = new PointLight();
        pl.Color = new SpriteColor(1, 1, 1, 1);
        pl.Intensity = 100;
        light1.AddComponent(pl);
        light2.AddComponent(new PointLight());
        
        renderer.Init();

        // add(pepper);
        add(two);
        add(light1);
        add(light2);
    }

    private void add(Gameobject go)
    {
        go.Init();
        go.Start();
        // Engine.Get().CurrentSelectedAsset = go;
        renderer.Add(go);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        renderer.Render();
        SwapBuffers();
    }
    
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        camera.adjustProjection();
    }
}