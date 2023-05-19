using System.Drawing;
using System.Numerics;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Engine2D.Testing;
using Engine2D.UI;
using ImGuiNET;
using KDBEngine.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine2D.Scenes;


internal class Scene
{
    private bool _isPlaying;

    #region onplay

    private World physicsWorld;

    #endregion

    internal string ScenePath { get; private set; } = "NoScene";
    internal List<Gameobject> Gameobjects { get; } = new();
   
    
    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            if (value)
            {
                SaveLoad.SaveScene(this);
                StartPlay();
            }
            else
            {
                StopPlay();
            }

            _isPlaying = value;
        }
    }

    private void StartPlay()
    {
        SaveLoad.SaveScene(this);
        
        physicsWorld = new World(new Vector2(0, -9.8f));
        foreach (var gameobject in Gameobjects)
        foreach (var item in gameobject.components)
            if (item.Type == "Rigidbody")
            {
                var bodyDef = new BodyDef();
                var rb = (RigidBody)item;
                bodyDef.BodyType = rb.BodyType;
                bodyDef.Position = rb.Parent.transform.position;

                var body = physicsWorld.CreateBody(bodyDef);
                body.IsFixedRotation = rb.FixedRotation;

                rb.runtimeBody = body;


                var shape = new PolygonShape();

                shape.SetAsBox(gameobject.transform.size.X * .5f, gameobject.transform.size.Y * .5f);
                var fixtureDef = new FixtureDef();
                fixtureDef.Shape = shape;
                fixtureDef.Density = 1.0f;
                fixtureDef.Friction = 0.5f;
                fixtureDef.Restitution = .0f;
                fixtureDef.RestitutionThreshold = 0.5f;

                body.CreateFixture(fixtureDef);
            }
    }

    internal void GameUpdate(double dt)
    {
        var velocityItterations = 6;
        var positionItterations = 2;

        physicsWorld.Step((float)dt, velocityItterations, positionItterations);

        foreach (var obj in Gameobjects) obj.GameUpdate(dt);
    }

    private void StopPlay()
    {
        SaveLoad.LoadScene(this.ScenePath);
    }

    private bool createdefault = false;
    internal virtual void Init(Engine engine, string scenePath, int width, int height)
    {
        TestRenderer t = new TestRenderer();
        t.Init();
        t.Render();
        
        ScenePath = scenePath;
        {
            var SpriteRenderer = new SpriteRenderer();
            
            Texture wrapTexture = Texture.Wrap(t.FetchColorAttachment(0));
            
            SpriteRenderer.texture = wrapTexture;
            var components = new List<Component>
            {
                SpriteRenderer
            };

            Gameobject pepper = new Gameobject("Pepper", components,
                new Transform(0,  0, 10, 10));
            AddGameObjectToScene(pepper);
        }
        
        if(createdefault)
        {
            {
                var pl = new PointLight();
                pl.Color = new SpriteColor(1, 1, 0, 1);
                var components = new List<Component>
                {
                    pl
                };
                var go = new Gameobject(
                    ("Point Light: " + Engine.Get()._currentScene?.Gameobjects.Count + 1).ToString(),
                    components, new Transform());
                Engine.Get()._currentScene?.AddGameObjectToScene(go);
            }
            {
                var spriteRenderer = new SpriteRenderer();
                spriteRenderer.textureData = new TextureData(
                    "D:\\dev\\EngineDev\\Engine2D\\src\\ExampleGame\\Images\\TestImage.png",
                    true,
                    TextureMinFilter.Nearest,
                    TextureMagFilter.Nearest
                );
                var components2 = new List<Component>
                {
                    spriteRenderer
                };

                var go2 = new Gameobject(("Mario: " + Engine.Get()._currentScene?.Gameobjects.Count + 1).ToString(),
                    components2, new Transform());
                Engine.Get()._currentScene?.AddGameObjectToScene(go2);
            }

            
        }
        
        renderer.Init();
    }

    private DefaultRenderer renderer = new DefaultRenderer(); 
    
    internal virtual void EditorUpdate(double dt)
    {
        if (TestInput.KeyDown(Keys.LeftControl))
            if (TestInput.KeyPress(Keys.S))
            {
                if (IsPlaying) return;
                SaveLoad.SaveScene(this);
            }

        if (TestInput.KeyDown(Keys.A))
        {
            Engine.Get().testCamera.position.X -= 100 * (float)dt;
        }
        if (TestInput.KeyDown(Keys.D))
        {
            Engine.Get().testCamera.position.X += 100 * (float)dt;
        }
        if (TestInput.KeyDown(Keys.W))
        {
            Engine.Get().testCamera.position.Y += 100 * (float)dt;
        }
        if (TestInput.KeyDown(Keys.S))
        {
            Engine.Get().testCamera.position.Y -= 100 * (float)dt;
        }
        
        foreach (var obj in Gameobjects) obj.EditorUpdate(dt);
        if (IsPlaying) GameUpdate(dt);
    }

    internal virtual void Render(double dt)
    {
        renderer.Render();
    }

    internal void AddGameObjectToScene(Gameobject go)
    {
        Gameobjects.Add(go);
        go.Init();
        go.Start();
        Engine.Get().CurrentSelectedAsset = go;
        renderer.Add(go);
    }

    internal virtual void OnClose()
    {
        if(EngineSettings.SaveOnClose)
            SaveLoad.SaveScene(this);
        
    }

    internal void OnGui()
    {
        ImGui.Begin("Scene Settings");


        ImGui.End();
    }

    #region inputs

    internal virtual void OnResized(ResizeEventArgs newSize)
    {
        Engine.Get().ImGuiController.WindowResized(newSize.Size.X, newSize.Size.Y);
    }

    internal virtual void OnMouseWheel(MouseWheelEventArgs mouseWheelEventArgs)
    {
        Engine.Get().ImGuiController.MouseScroll(mouseWheelEventArgs.Offset);
    }

    internal virtual void OnTextInput(TextInputEventArgs inputEventArgs)
    {
        Engine.Get().ImGuiController.PressChar((char)inputEventArgs.Unicode);
    }

    #endregion
}