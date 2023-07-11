﻿#region

using Engine2D.Cameras;
using Engine2D.Components.ENTT;
using Engine2D.Core;
using Engine2D.Core.Inputs;
using Engine2D.Logging;
using Engine2D.Physics;
using Engine2D.Rendering.NewRenderer;
using Engine2D.SavingLoading;
using Engine2D.UI.Debug;
using EnTTSharp.Entities;
using Newtonsoft.Json;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

#endregion

namespace Engine2D.Scenes;

public class Scene
{
    // [JsonProperty]internal List<Gameobject?> GameObjects = new();
    // [JsonIgnore] private List<Gameobject?> _clonesOnStart = new();
    // [JsonIgnore]internal Renderer? Renderer { get; private set; }
    
    [JsonProperty]internal string ScenePath { get; private set; } = "NoScene";
    [JsonProperty]public static bool DoUpdate = true;
    [JsonProperty]public List<Entity> Entities = new();
    
    [JsonIgnore] public EntityRegistry<EntityKey> EntityRegistry { get; private set; }

    [JsonIgnore] public Physics2DWorld? _physics2DWorld;
    

    [JsonIgnore]private int _totalTimesTimeCounted = 0;
    [JsonIgnore]private double _totalTime = 0;
    
    [JsonIgnore]private bool _isPlaying;

    internal bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            if (value)
            {
                if (IsPlaying) return;
                
            }
            else
            {
                if (!_isPlaying) return;
                
            }

            _isPlaying = value;
        }
    }
    
    /// <summary>
    ///     Runs before anything
    /// </summary>
    /// <param name="scenePath"></param>
    internal virtual void Init(string scenePath)
    {
        CreateEntityRegistry();

        ScenePath = scenePath;

        //Load Scene if exists
        
        if (Settings.s_IsEngine)
            CreateEditorCamera();

        Renderer.Init();
    }

    private void CreateEntityRegistry()
    {
        EntityRegistry =  
            new EntityRegistry<EntityKey>
            (EntityKey.MaxAge, 
                (age, id) 
                    => new EntityKey(age, id));
        
        EntityRegistry.Register<ENTTTransformComponent>();
        EntityRegistry.Register<ENTTSpriteRenderer>();
        EntityRegistry.Register<ENTTTagComponent>();
        
        //Temp code
        CreateEntity("test");
        CreateEntity("test2");
        
    }

    internal Entity CreateEntity(string name)
    {
        var key = EntityRegistry.Create();
        
        Entity en = new Entity(key,this);
        en.AddComponent(new ENTTTagComponent(name));
        en.AddComponent(new ENTTTransformComponent());
      
        Entities.Add(en);
        
        return en;
    }

    /// <summary>
    /// Runs every frame
    /// </summary>
    /// <param name="args"></param>
    internal void Update(FrameEventArgs args)
    {
        Engine.Get().Title = string.Format($"Frame Time: {args.Time * 1000:0.00}ms" + "  |  " + $"FPS: {1 / args.Time:0.00}");
        
        _totalTime += args.Time;
        _totalTimesTimeCounted++;
        

        if (IsPlaying&&DoUpdate)
        {
            if (Settings.s_IsEngine) 
                EditorUpdate(args);
            GameUpdate(args);
            GameFixedUpdate();
        }
        
        //TEMP CODE
        if (Input.KeyDown(Keys.P))
        {
            EntityRegistry.GetComponent<ENTTTransformComponent>(
                Entities[0].m_EntityHandle, out ENTTTransformComponent transform);
            
            transform.Position.X += 1;
            
            Entities[0].SetComponent<ENTTTransformComponent>(transform);
        }

        AfterUpdate();
    }

    /// <summary>
    /// Runs every frame on editor update, so only if game is in the editor
    /// </summary>
    /// <param name="args"></param>
    private void EditorUpdate(FrameEventArgs args)
    {
      
    }
    
    /// <summary>
    /// Runs every frame that the game is running
    /// </summary>
    /// </summary>
    /// <param name="args"></param>
    private void GameUpdate(FrameEventArgs args)
    {
     
    }

    private float _physicsTime = 0.0f;
    private float _physicsTimeStep = 1.0f / 60.0f;
    
    private void GameFixedUpdate()
    {
        _physicsTime += (float)Engine.DeltaTime;
        if (_physicsTime >= 0.0f) {
            _physicsTime -= _physicsTimeStep;
        }
    }
    
      
    /// <summary>
    /// After update is ran after all the updates are done
    /// </summary>
    private void AfterUpdate()
    {
        
    }
    
    /// <summary>
    /// Render the scene
    /// </summary>
    /// <param name="dt">deltatime</param>
    internal virtual void Render(float dt)
    {
       Renderer.BeginScene();
       Renderer.Render();       
       Renderer.EndScene();
    }

    internal virtual void Close()
    {
        if (EngineSettings.SaveOnClose && !IsPlaying)
            SaveLoad.SaveScene(this);
        
        var scenename = "stresstest";
        var Time = (_totalTime / _totalTimesTimeCounted);

        var currentTime = System.DateTime.Now;
        string data = debug_data.GetDebugData(Time);
        
        Log.Succes("LOG: \n" + data);
        var path = "..\\..\\..\\Logs";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        var fileName = scenename + "--" +currentTime.Second + "-" + currentTime.Minute + "-" + currentTime.Hour + "-" + currentTime.Day + ".txt";
        var fullPath = Path.Combine(path, fileName);
        using (var fs = File.Create(fullPath))
        {
            fs.Close();
        }

        File.WriteAllText(fullPath, data);
        
    }

    internal void OnGui()
    {
    }

    internal virtual void OnResized(ResizeEventArgs newSize)
    {
        
    }

    internal virtual void OnMouseWheel(MouseWheelEventArgs mouseWheelEventArgs)
    {
        
    }

    internal virtual void OnTextInput(TextInputEventArgs inputEventArgs)
    {
    }
    
    private void CreateEditorCamera(Camera? prevEditorCamera = null)
    {
       
        if (prevEditorCamera != null)
        {
         
        }
    }


    public Camera? GetMainCamera()
    {
        // foreach (var go in GameObjects)
        // {
        //     var cam = go.GetComponent<Camera>();
        //     if (cam != null)
        //     {
        //         if (cam._isMainCamera) return cam;
        //     }
        // }
        return null;
    }
    
    internal Camera? GetEditorCamera()
    {
        // foreach (var go in GameObjects)
        // {
        //     var cam = go.GetComponent<Camera>();
        //     if (cam != null)
        //     {
        //         if (cam._isEditorCamera) return cam;
        //     }
        // }

        return null;
    }
}