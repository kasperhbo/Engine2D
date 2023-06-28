#region

using Box2DSharp.Dynamics;
using Engine2D.Cameras;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Physics;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Newtonsoft.Json;
using OpenTK.Windowing.Common;

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

    [JsonIgnore] private Physics2DWorld? _physics2DWorld;
    
    private void StartPlay()
    {
        SaveLoad.SaveScene(this);

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

        _physics2DWorld = null;
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