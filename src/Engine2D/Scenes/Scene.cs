using System.Net.Mime;
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
    internal List<GameObject> Gameobjects { get; } = new();

    public GlobalLight GlobalLight { get; set; } = null;

    public TestCamera EditorCamera;
    public TestCamera GameCamera = null;

    private List<TestCamera> _cameras = new();
    
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

                rb.RuntimeBody = body;


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
        SaveLoad.LoadScene(ScenePath);
    }

    internal virtual void Init(string scenePath)
    {
        EditorCamera = new TestCamera();
       
        _cameras.Add(EditorCamera);
        
        ScenePath = scenePath;
        Renderer.Init();
    }

    internal virtual void CreateDefaultGameObjects()
    {
        //Create main camera
        {
            List<Component> components = new List<Component>
            {
                new TestCamera()
            };
        
            GameObject gameCamera = new GameObject("Main Camera", new Transform(),components);
            AddGameObjectToScene(gameCamera);
        }
        
        //Create global light
        {
            GlobalLight globalLight = new GlobalLight();
            
            List<Component> defaultComponents =new List<Component> {
                globalLight
            };
            
            GameObject go = new GameObject("Global Light", new Transform(), defaultComponents);
            AddGameObjectToScene(go);
        }
    }
    
    internal virtual void EditorUpdate(double dt)
    {
        if(TestInput.Focussed)
        {
            if (TestInput.KeyDown(Keys.LeftControl))
                if (TestInput.KeyPress(Keys.S))
                {
                    if (IsPlaying) return;
                    SaveLoad.SaveScene(this);
                }
        }
        
        if(TestInput.MouseDown(MouseButton.Right))
        {
            float difX = TestInput.xPos - TestInput.lastX;
            float difY = TestInput.yPos - TestInput.lastY;

            EditorCamera.position += new Vector2(difX, difY);
        }

        foreach (var obj in Gameobjects) obj.EditorUpdate(dt);
        if (IsPlaying) GameUpdate(dt);
    }

    internal virtual void Render(double dt)
    {
    }
    
    // internal void AddGameObjectToScene(GameCamera go)
    // {
    //     GameCamera = go;
    //     AddGameObjectToScene((Gameobject)go);
    // }
    //
    internal void AddGameObjectToScene(GameObject go)
    {
        if (go.GetComponent<TestCamera>() != null)
        {
            _cameras.Add(go.GetComponent<TestCamera>());
            GameCamera = go.GetComponent<TestCamera>();
        }
        
        Gameobjects.Add(go);
        go.Init();
        go.Start();
        Engine.Get().CurrentSelectedAsset = go;
    }

    internal virtual void OnClose()
    {
        if (EngineSettings.SaveOnClose)
            SaveLoad.SaveScene(this);

        Renderer.OnClose();
    }

    internal void OnGui()
    {
        EditorCamera.CameraSettingsGUI();
        
        ImGui.Begin("Scene Settings");


        ImGui.End();
    }

    #region inputs

    internal virtual void OnResized(ResizeEventArgs newSize)
    {
        Renderer.OnResize(newSize);
        Engine.Get().ImGuiController.WindowResized(newSize.Size.X, newSize.Size.Y);
        foreach (var cam in _cameras)
        {
            cam.adjustProjection();
        }
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