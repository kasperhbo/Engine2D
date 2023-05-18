using System.Drawing;
using System.Numerics;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Engine2D.Components;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Engine2D.Testing;
using Engine2D.UI;
using ImGuiNET;
using KDBEngine.Core;
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

    internal virtual void Init(Engine engine, string scenePath, int width, int height)
    {
        ScenePath = scenePath;
        GameRenderer.Init();
        
        int count = 0;
        float r = 0;
        float g = 255;
        float pos = -925;
        float posy = -524;
        
        for (int i = 0; i < 22; i++)
        {
            for (int x = 0; x < 15; x++)
            {
                // Console.WriteLine(i);
                var pl = new PointLight();
                pl.Color = new SpriteColor(r/255,g/255,1,1);
                pl.Intensity = .1f;
                r += 1.307f;
                g -= 1.307f;
                var components = new List<Component>
                {
                    pl
                };
                Transform transform = new Transform();
                transform.position = new Vector2(pos, posy);
            
                var go = new Gameobject(("Point Light: " + Gameobjects.Count).ToString(),
                    components, transform);
            
                AddGameObjectToScene(go);
                pos += 30;
                count++;
            }

            pos = -925;
            posy += 60;

        }
        
        Console.WriteLine(count);
        
    }

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
        
    }

    internal void AddGameObjectToScene(Gameobject go)
    {
        Gameobjects.Add(go);
        go.Init();
        go.Start();
        Engine.Get().CurrentSelectedAsset = go;
    }

    internal virtual void OnClose()
    {
        if(EngineSettings.SaveOnClose)
            SaveLoad.SaveScene(this);
        
        GameRenderer.OnClose();
    }

    internal void OnGui()
    {
        ImGui.Begin("Scene Settings");


        ImGui.End();
    }

    #region inputs

    internal virtual void OnResized(ResizeEventArgs newSize)
    {
        GameRenderer.OnResize(newSize);
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