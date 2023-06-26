﻿#region

using System.Numerics;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Engine2D.Cameras;
using Engine2D.Components;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.Core.Inputs;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Engine2D.UI;
using Engine2D.Utilities;
using Newtonsoft.Json;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

#endregion

namespace Engine2D.Scenes;

public class Scene
{
    [JsonProperty]internal Camera? CurrentMainGameCamera;
    [JsonProperty]internal List<Gameobject> GameObjects = new();
    [JsonIgnore]internal Renderer? Renderer { get; private set; }
    [JsonIgnore]internal Camera? EditorCamera;

    [JsonProperty]internal string ScenePath { get; private set; } = "NoScene";
    [JsonProperty]internal GlobalLight GlobalLight { get; set; } = null;
    
    
    private void StartPlay()
    {
        SaveLoad.SaveScene(this);
        
        
        //_physicsWorld = new World(new Vector2(0, -9.8f));

        foreach (var go in GameObjects)
        {
            go.StartPlay();
        }
    }
  
    private void StopPlay()
    {
        foreach (var go in GameObjects)
        {
            go.StopPlay();
        }
    }
    
     
    

    /// <summary>
    ///     Runs before anything
    /// </summary>
    /// <param name="scenePath"></param>
    internal virtual void Init(string scenePath)
    {
        Renderer = new Renderer();
        Renderer.Init();

        ScenePath = scenePath;

        //Try to load scene if non existent this returns a new list 
        var gos = SaveLoad.LoadScene(ScenePath);
        foreach (var go in gos) AddGameObjectToScene(go);

        Start();
    }

    /// <summary>
    ///     Runs at scene start | NOT GAME PLAY!
    /// </summary>
    internal virtual void Start()
    {
        foreach (var go in GameObjects) go.Start();

        if(Settings.s_IsEngine || CurrentMainGameCamera == null)
        {
            var editorCameraGO = new EditorCameraGO("EDITORCAMERA");
            EditorCamera = editorCameraGO.GetComponent<Camera>();
            editorCameraGO.Serialize = false;
            AddGameObjectToScene(editorCameraGO);
        }
        
    }
    
   
    /// <summary>
    /// Runs every frame
    /// </summary>
    /// <param name="args"></param>
    internal void Update(FrameEventArgs args)
    {
        if (Settings.s_IsEngine) EditorUpdate(args);
        if (IsPlaying) GameUpdate(args);
        for (int i=0; i < GameObjects.Count; i++) {
            {
                var obj = GameObjects[i];
                obj.Update(args);
                if (obj.IsDead)
                {
                    GameObjects.Remove(obj);
                    RemoveGameObject(obj);
                    i--;
                }
            }
        }
    }
    
    /// <summary>
    /// Runs every frame on editor update, so only if game is in the editor
    /// </summary>
    /// <param name="args"></param>
    private void EditorUpdate(FrameEventArgs args)
    {
        foreach (var obj in GameObjects) obj.EditorUpdate((float)Engine.DeltaTime);

        if (Engine.Get().CurrentSelectedAsset != null)
        {
            if (Input.KeyPressed(Keys.F))
            {
                Gameobject? go = (Gameobject)Engine.Get().CurrentSelectedAsset;

                if (go != null)
                    EditorCamera.Parent.GetComponent<Transform>().Position = go.GetComponent<Transform>().Position;
            }
        }

        //Console.WriteLine(MouseListener.GetWorld());
        var pos =  Input.MouseEditorPos;
        Console.WriteLine(pos);

        GameObjects[4].GetComponent<Transform>().Position = new(pos.X, pos.Y);


    }
    
    /// <summary>
    /// Runs every frame that the game is running
    /// </summary>
    /// <param name="args"></param>
    private void GameUpdate(FrameEventArgs args)
    {
        foreach (var obj in GameObjects) obj.GameUpdate((float)Engine.DeltaTime);
    }
    
    internal virtual void Render(float dt)
    {
        if(CurrentMainGameCamera != null)
            Renderer.Render(EditorCamera, CurrentMainGameCamera);
        else
        {
            Log.Error("No Main Camera Found!");
        }
    }

    public void AddGameObjectToScene(Gameobject go)
    {
        if (go.GetComponent<Camera>() != null && (CurrentMainGameCamera == null && go.Name != "EDITORCAMERA"))
        {
            Log.Succes(go.Name + " Is the new Main Camera");
            if(go.GetComponent<Camera>().IsMainCamera)
            {
                CurrentMainGameCamera = go.GetComponent<Camera>();
            }
        }

        GameObjects.Add(go);

        go.Init(Renderer);
        go.Start();

        Engine.Get().CurrentSelectedAsset = go;
    }

    internal virtual void Close()
    {
        if (EngineSettings.SaveOnClose)
            SaveLoad.SaveScene(this);
    }

    internal void OnGui()
    {
    }

    internal Gameobject FindObjectByUID(int uid)
    {
        foreach (var go in GameObjects)
            if (go.UID == uid)
                return go;

        throw new Exception(string.Format("Gameobject with {0} not found", uid));
    }

    #region onplay

    private bool _isPlaying;

    internal bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            if (value)
            {
                if (IsPlaying) return;
                SaveLoad.SaveScene(this);
                StartPlay();
            }
            else
            {
                if (!_isPlaying) return;
                StopPlay();
            }

            _isPlaying = value;
        }
    }

    private World? _physicsWorld;

    #endregion

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

    public void RemoveGameObject(Gameobject go)
    {
        Renderer.RemoveSprite(go);
        Engine.Get().CurrentSelectedAsset = null;
    }
}