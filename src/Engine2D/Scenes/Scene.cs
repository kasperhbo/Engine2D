using System.Numerics;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Engine2D.Components;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Engine2D.Testing;
using ImGuiNET;
using KDBEngine.Core;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine2D.Scenes;

internal class Scene
{
    public Renderer Renderer { get; private set; }

    #region onplay
    
    private bool _isPlaying;

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
    
    private World physicsWorld;

    #endregion

    internal string ScenePath { get; private set; } = "NoScene";
    
    public List<Gameobject> Gameobjects = new List<Gameobject>();
    public GlobalLight GlobalLight { get; set; } = null;
    
    internal TestCamera? EditorCamera = null;
    internal TestCamera? CurrentMainGameCamera = null;
    
    private void StartPlay()
    {
        SaveLoad.SaveScene(this);

        physicsWorld = new World(new Vector2(0, -9.8f));
        foreach (var gameobject in Gameobjects)
        foreach (var item in gameobject.components)
            if (item.Type == "Rigidbody")
            {
                throw new NotImplementedException();
                var bodyDef = new BodyDef();
                var rb = (RigidBody)item;
                bodyDef.BodyType = rb.BodyType;
                //bodyDef.Position = rb.Parent.transform.position;

                var body = physicsWorld.CreateBody(bodyDef);
                body.IsFixedRotation = rb.FixedRotation;

                rb.RuntimeBody = body;


                var shape = new PolygonShape();

                // shape.SetAsBox(gameobject.transform.size.X * .5f, gameobject.transform.size.Y * .5f);
                var fixtureDef = new FixtureDef();
                fixtureDef.Shape = shape;
                fixtureDef.Density = 1.0f;
                fixtureDef.Friction = 0.5f;
                fixtureDef.Restitution = .0f;
                fixtureDef.RestitutionThreshold = 0.5f;

                body.CreateFixture(fixtureDef);
            }
    }

    public void GameUpdate(double dt)
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

    /// <summary>
    /// Runs before anything
    /// </summary>
    /// <param name="scenePath"></param>
    internal virtual void Init(string scenePath)
    {
        Renderer = new Renderer();
        Renderer.Init();
        
        if(Settings.s_IsEngine)
        {
            EditorCamera = new TestCamera(Engine.Get().Size);
        }
        
        ScenePath = scenePath;
        LoadDataFromDisk();
        
        foreach (var go in Gameobjects)
        {
            go.Init(Renderer);
        }
        
        Start();
    }

    private void LoadDataFromDisk()
    {
        if (File.Exists(ScenePath))
        {
            Log.Succes("found: " + ScenePath + " On Disk");
            Gameobjects = SaveLoad.LoadScene(ScenePath);
        }
    }

    /// <summary>
    /// Runs at scene start | NOT GAME PLAY!
    /// </summary>
    /// <param name="dt"></param>

    public virtual void Start()
    {
        foreach (var go in Gameobjects)
        {
            go.Start();
        }

        //Set childs to parents based on UID
        foreach (var go in Gameobjects)
        {
            if (go.PARENT_UID == -1) return;
            go.SetParent(go.PARENT_UID);
        }
    }
    
    public virtual void EditorUpdate(double dt)
    {
        if (TestInput.KeyDown(Keys.LeftControl))
            if (TestInput.KeyPress(Keys.S))
            {
                if (IsPlaying) return;
                SaveLoad.SaveScene(this);
            }

        if (Engine.Get().IsKeyPressed(Keys.F))
        {
            Gameobject go = (Gameobject)Engine.Get().CurrentSelectedAsset;
            if (go != null)
            {
                EditorCamera.Transform.position = go.Transform.position;
            }
        }
        
        foreach (var obj in Gameobjects) obj.EditorUpdate(dt);
        if (IsPlaying) GameUpdate(dt);
    }

    public virtual void Render()
    {
        Renderer.Render(EditorCamera, CurrentMainGameCamera);
    }

    public void AddGameObjectToScene(Gameobject go)
    {
        if (go.GetComponent<TestCamera>() != null)
        {
            Log.Succes("New Main Camera");
            CurrentMainGameCamera = go.GetComponent<TestCamera>();
            CurrentMainGameCamera.adjustProjection((1920, 1080));
        }

        Gameobjects.Add(go);
        go.Init(Renderer);
        go.Start();
        Engine.Get().CurrentSelectedAsset = go;
    }

    public virtual void OnClose()
    {
        if (EngineSettings.SaveOnClose)
            SaveLoad.SaveScene(this);
    }

    public void OnGui()
    {
        EditorCamera.CameraSettingsGUI();
        
        ImGui.Begin("Scene Settings");
        ImGui.End();
    }

    #region inputs

    internal virtual void OnResized(ResizeEventArgs newSize)
    {
        Renderer.OnResize(newSize);
    }

    internal virtual void OnMouseWheel(MouseWheelEventArgs mouseWheelEventArgs)
    {
        EditorCamera?.addZoom(mouseWheelEventArgs.OffsetY/10);
        EditorCamera?.adjustProjection();
    }

    internal virtual void OnTextInput(TextInputEventArgs inputEventArgs)
    {
        
    }

    #endregion

    public Gameobject FindObjectByUID(int uid)
    {
        foreach (var go in Gameobjects)
        {
            if (go.UID == uid)
            {
                return go;
            }
        }

        throw new Exception(string.Format("Gameobject with {0} not found", uid));
    }
}