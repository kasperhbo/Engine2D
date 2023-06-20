﻿using System.Numerics;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Engine2D.Cameras;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Engine2D.Utilities;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine2D.Scenes;

public class Scene
{
    public Renderer? Renderer { get; private set; }

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
    private World? _physicsWorld;

    #endregion

    internal string ScenePath { get; private set; } = "NoScene";
    
    public List<Gameobject> GameObjects = new List<Gameobject>();
    public GlobalLight GlobalLight { get; set; } = null;
    
    internal Camera? EditorCamera;
    internal Camera? CurrentMainGameCamera;
    
    public List<Action<FrameEventArgs>> GetDefaultUpdateEvents()
    {
        List<Action<FrameEventArgs>> res = new();
        
        if(Settings.s_IsEngine|| Settings.s_RenderDebugWindowSeperate)
            res.Add(EditorUpdate);
        
        if(_isPlaying)
            res.Add(GameUpdate);
        
        return res;
    }
    
    private void StartPlay()
    {
        Engine.Get().UpdateFrame += GameUpdate;
        
        SaveLoad.SaveScene(this);

        _physicsWorld = new World(new Vector2(0, -9.8f));
        foreach (var gameobject in GameObjects)
        foreach (var item in gameobject.components)
            if (item.Type == "Rigidbody")
            {
                throw new NotImplementedException();
                var bodyDef = new BodyDef();
                var rb = (RigidBody)item;
                bodyDef.BodyType = rb.BodyType;
                //bodyDef.Position = rb.Parent.transform.position;

                var body = _physicsWorld.CreateBody(bodyDef);
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

    public void GameUpdate(FrameEventArgs args)
    {
        var velocityItterations = 6;
        var positionItterations = 2;

        _physicsWorld.Step((float)args.Time, velocityItterations, positionItterations);

        foreach (var obj in GameObjects) obj.GameUpdate(args.Time);
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
        
        ScenePath = scenePath;
        
        //Try to load scene if non existent this returns a new list 
        var gos = SaveLoad.LoadScene(ScenePath);
        foreach (var go in gos)
        {
            AddGameObjectToScene(go);
        }

        Start();
    }

    /// <summary>
    /// Runs at scene start | NOT GAME PLAY!
    /// </summary>
    public virtual void Start()
    {
        foreach (var go in GameObjects)
        {
            go.Start();
        }

        EditorCameraGO editorCameraGO = new EditorCameraGO("EDITORCAMERA");
        EditorCamera = editorCameraGO.GetComponent<Camera>();
        editorCameraGO.Serialize = false;
        editorCameraGO.AddComponent(AssemblyUtils.GetComponent("ExampleGame.Assets.TestClass"));
        AddGameObjectToScene(editorCameraGO);

        Console.WriteLine(GameObjects[0].Name + " gameobject name");
        GameObjects[0].AddComponent(AssemblyUtils.GetComponent("ExampleGame.Assets.TestClass"));
    }
    
    public virtual void EditorUpdate(FrameEventArgs args)
    {
        foreach (var obj in GameObjects) obj.EditorUpdate(args.Time);
        
        if(Engine.Get().IsKeyPressed(Keys.B))
            Engine.Get().SwitchScene("new");
    }
    
    public virtual void Render(FrameEventArgs args)
    {
        Renderer.Render(EditorCamera, CurrentMainGameCamera);
    }
    
    public void AddGameObjectToScene(Gameobject go)
    {
        if (go.GetComponent<Camera>() != null && (go.Name != "EDITORCAMERA" || CurrentMainGameCamera == null))
        {
            Log.Succes(go.Name  + " Is the new Main Camera");
            CurrentMainGameCamera = go.GetComponent<Camera>();
        }

        GameObjects.Add(go);
        
        go.Init(Renderer);
        go.Start();
        
        Engine.Get().CurrentSelectedAsset = go;
    }

    public virtual void Close()
    {
        if (EngineSettings.SaveOnClose)
            SaveLoad.SaveScene(this);
    }

    public void OnGui()
    {
        
    }

    #region inputs

    internal virtual void OnResized(ResizeEventArgs newSize)
    {
        Renderer.OnResize(newSize);
    }

    internal virtual void OnMouseWheel(MouseWheelEventArgs mouseWheelEventArgs)
    {
        // EditorCamera?.addZoom(mouseWheelEventArgs.OffsetY/10);
        // EditorCamera?.adjustProjection();
    }

    internal virtual void OnTextInput(TextInputEventArgs inputEventArgs)
    {
        
    }

    #endregion

    public Gameobject FindObjectByUID(int uid)
    {
        foreach (var go in GameObjects)
        {
            if (go.UID == uid)
            {
                return go;
            }
        }

        throw new Exception(string.Format("Gameobject with {0} not found", uid));
    }
}