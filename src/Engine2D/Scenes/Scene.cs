#region

using System.Numerics;
using Box2DSharp.Dynamics;
using Engine2D.Cameras;
using Engine2D.Components;
using Engine2D.Components.Cameras;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Physics;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Newtonsoft.Json;
using OpenTK.Graphics.ES30;
using OpenTK.Windowing.Common;

#endregion

namespace Engine2D.Scenes;

public class Scene
{
    [JsonProperty]internal List<Gameobject?> GameObjects = new();
    [JsonIgnore]internal Renderer? Renderer { get; private set; }
    
    [JsonProperty]internal string ScenePath { get; private set; } = "NoScene";
    [JsonProperty]internal GlobalLight GlobalLight { get; set; } = null;

    [JsonIgnore] public Physics2DWorld? _physics2DWorld;
    [JsonIgnore] private List<Gameobject?> _clonesOnStart = new();

    private void StartPlay()
    {
        SaveLoad.SaveScene(this);

        _clonesOnStart = new();
        foreach (var go in GameObjects)
        {
            if(go is { Serialize: true }) 
                _clonesOnStart.Add((Gameobject)go.Clone());
        }
        
        _physics2DWorld = new();
        
        foreach (var go in GameObjects)
        {
            go.StartPlay(_physics2DWorld);
        }
    }
  
    private void StopPlay()
    {
        foreach (var go in GameObjects)
        {
            go.StopPlay();
        }
        ReloadScene(_clonesOnStart);
    }

    public Camera? GetMainCamera()
    {
        foreach (var go in GameObjects)
        {
            var cam = go.GetComponent<Camera>();
            if (cam != null)
            {
                if (cam._isMainCamera) return cam;
            }
        }

        return null;
    }
    public Gameobject? GetMainCameraGO()
    {
        foreach (var go in GameObjects)
        {
            var cam = go.GetComponent<Camera>();
            if (cam != null)
            {
                if (cam._isMainCamera) return cam.Parent;
            }
        }

        return null;
    }

    internal Camera? GetEditorCamera()
    {
        foreach (var go in GameObjects)
        {
            var cam = go.GetComponent<Camera>();
            if (cam != null)
            {
                if (cam._isEditorCamera) return cam;
            }
        }

        return null;
    }

    internal void ReloadScene()
    {
        var clones = new List<Gameobject?>();
        foreach (var go in GameObjects)
        {
            if(go.Serialize)
            {
                clones.Add((Gameobject)go.Clone());
            }
        }
        ReloadScene(clones);
    }

    private void ReloadScene(List<Gameobject?> clones)
    {
        var prevEditorCamera = GetEditorCamera();
        foreach (var go in GameObjects)
        {
            go.Destroy();
        }
        
        GameObjects = new();
        foreach (var go in clones)
        {
            AddGameObjectToScene(go);
        }
        
        if(Settings.s_IsEngine)
            CreateEditorCamera(prevEditorCamera);
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
        
        if(Settings.s_IsEngine)
            CreateEditorCamera();

        //Try to load scene if non existent this returns a new list 
        var gos = SaveLoad.LoadScene(ScenePath);
        foreach (var go in gos)
        {
            AddGameObjectToScene(go);
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
                    obj.Destroy();
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
    }
    
    /// <summary>
    /// Runs every frame that the game is running
    /// </summary>
    /// </summary>
    /// <param name="args"></param>
    private void GameUpdate(FrameEventArgs args)
    {
        foreach (var obj in GameObjects) obj.GameUpdate((float)Engine.DeltaTime);
        _physics2DWorld?.GameUpdate((float)args.Time);
    }
    
    internal virtual void Render(float dt)
    {
        Renderer.Render();
    }

    internal virtual void Close()
    {
        if (EngineSettings.SaveOnClose)
            SaveLoad.SaveScene(this);
    }

    internal void OnGui()
    {
    }

    internal Gameobject? FindObjectByUID(int uid)
    {
        foreach (var go in GameObjects)
            if (go.UID == uid)
                return go;

        return null;
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

    #endregion
    

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
    
    public void RemoveGameObject(Gameobject? go)
    {
        GameObjects.Remove(go);   
    }

    private void CreateEditorCamera(Camera? prevEditorCamera = null)
    {
        Gameobject? editorCameraGo = new Gameobject("EDITORCAMERA");
        
        editorCameraGo.AddComponent(new CameraControls());
        editorCameraGo.CanBeSelected = false;
        
        Camera cam = (Camera)editorCameraGo.AddComponent(new Camera());

        editorCameraGo.Serialize = false;
        cam._isEditorCamera = true;

        AddGameObjectToScene(editorCameraGo);
        
        if (prevEditorCamera != null)
        {
            cam.CameraType = prevEditorCamera.CameraType;
            cam.Far = prevEditorCamera.Far;
            cam.Near = prevEditorCamera.Near;
            cam.ClearColor = prevEditorCamera.ClearColor;
            cam.Size = prevEditorCamera.Size;

            editorCameraGo.Transform.Position = prevEditorCamera.Parent.Transform.Position;

        }
    }
    
    public void AddGameObjectToScene(Gameobject? go)
    {
        GameObjects.Add(go);

        go.Init(Renderer);
        go.Start();
        
        if(go.Serialize)
            Engine.Get().CurrentSelectedAsset = go;
    }
}