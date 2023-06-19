using System.Runtime.InteropServices;
using Engine2D.Components;
using Engine2D.Components.Sprites;
using Engine2D.Core;
using Engine2D.Flags;
using Engine2D.Rendering;
using Newtonsoft.Json;
using Engine2D.Components.TransformComponents;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.SavingLoading;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SixLabors.ImageSharp.Processing.Processors.Dithering;
using Vector2 = System.Numerics.Vector2;

namespace Engine2D.GameObjects;

public class KDBColor
{
    [JsonRequired]public float r;
    [JsonRequired]public float g;
    [JsonRequired]public float b;
    [JsonRequired]public float a;
                  
    [JsonIgnore]public float RNormalized
    {
        get { return r / 255; }
    }
    
    [JsonIgnore]public float GNormalized
    {
        get { return g / 255; }
    }
    
    [JsonIgnore]public float BNormalized
    {
        get { return b / 255; }
    }
    
    [JsonIgnore]public float ANormalized
    {
        get { return a / 255; }
    }
    
    public KDBColor()
    {
        this.r = 255;
        this.g = 255;
        this.b = 255;
        this.a = 255;
    }
    
    public KDBColor(KDBColor other)
    {
        this.r = other.r;
        this.g = other.g;
        this.b = other.b;
        this.a = other.a;
    }

    public KDBColor(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }


    public bool Equals(KDBColor? obj)
    {
        if(obj == null) { Log.Error("Obj to check against not set  'KDB COLOR' ");
            return false;
        }
        return (this.r == obj.r && this.g == obj.g && this.b == obj.b && this.a == obj.a);
    }

    public static void Copy(KDBColor from, KDBColor to)
    {
        from.r = to.r;
        from.g = to.g;
        from.b = to.b;
        from.a = to.a;
    }
}

[JsonConverter(typeof(ComponentSerializer))]
public class SpriteRenderer : Component
{
    [JsonProperty]private string _spriteSaveFile = "";
    [JsonProperty]public int ZIndex = 0;
    
    [JsonProperty]public KDBColor Color = new KDBColor();
    [JsonIgnore]public Sprite? Sprite { get; private set; } = null;
    [JsonIgnore]public bool IsDirty { get; set; }

    // 0.5f,   0.5f, 0.0f,    1.0f, 1.0f,   // top right
    // 0.5f,  -0.5f, 0.0f,    1.0f, 0.0f,   // bottom right
    // -0.5f, -0.5f, 0.0f,    0.0f, 0.0f,   // bottom left
    // -0.5f,  0.5f, 0.0f,    0.0f, 1.0f    // top left 
    [JsonIgnore] private Transform _lastTransform = new Transform();
    [JsonIgnore] private KDBColor _lastColor = new KDBColor();
    [JsonIgnore]private Vector2[] _defaultTextureCoords =
    {
        new(1.0f, 1.0f),
        new(1.0f, 0.0f),
        new(0.0f, 0.0f),
        new(0.0f, 1.0f),
    };
    
    public Vector2[] TextureCoords {
        get
        {
            if (Sprite == null) return _defaultTextureCoords;
            
            return Sprite.TextureCoords;
        }
    }

    [JsonConstructor]
    public SpriteRenderer()
    {
        IsDirty = true;
    }

    public override void Init(Gameobject parent, Renderer? renderer)
    {
        ResourceManager.SpriteRenderers.Add(this);
        base.Init(parent, renderer);
        
        if (_spriteSaveFile == "")
        {
            Engine.Get().CurrentScene.Renderer.AddSpriteRenderer(this);
        }
        else
        {
            SetSprite(_spriteSaveFile);
        }
    }

    public override void EditorUpdate(double dt)
    {
        base.EditorUpdate(dt);

        if (!_lastColor.Equals(Color) || !_lastTransform.Equals(Parent.GetComponent<Transform>()))
        {
            IsDirty = true;
            KDBColor.Copy(_lastColor, Color);
            Transform.Copy(_lastTransform, Parent.GetComponent<Transform>());
        }
        
    }

    private FileSystemWatcher _fileSystemWatcher;

    public void RefreshSprite()
    {
        SetSprite(_spriteSaveFile);
    }
    
    public void SetSprite(string newSprite)
    {
        Engine.Get().CurrentScene?.Renderer?.RemoveSprite(this);
        
        IsDirty = true;
        string oldSprite = _spriteSaveFile;
        _spriteSaveFile = newSprite;

        Sprite = ResourceManager.GetItem<Sprite>(_spriteSaveFile);
        Engine.Get().CurrentScene?.Renderer?.AddSpriteRenderer(this);
    }

    private void SetSprite(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine("Folder changed");
        SetSprite(e.FullPath);
    }

    private int index = 1;
    public override void ImGuiFields()
    {
        base.ImGuiFields();
        if (ImGui.Button("Sprite"))
        {
            if(index == 1)
            {
                index = 2;
                SetSprite(ProjectSettings.FullProjectPath + "\\Assets\\folder-open-icon.sprite");
            }
            else
            {
                index = 1;
                SetSprite(ProjectSettings.FullProjectPath + "\\Assets\\bigSpritesheet.sprite");
            }
        }
        
        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGui.AcceptDragDropPayload("sprite_drop");
            if (payload.IsValidPayload())
            {
                var filename = (string)GCHandle.FromIntPtr(payload.Data).Target;
                SetSprite(filename);
            }
            ImGui.EndDragDropTarget();
        }


        if (Sprite != null)
        {
            ImGui.InputFloat2("0", ref Sprite.TextureCoords[0]);
            ImGui.InputFloat2("1", ref Sprite.TextureCoords[1]);
            ImGui.InputFloat2("2", ref Sprite.TextureCoords[2]);
            ImGui.InputFloat2("3", ref Sprite.TextureCoords[3]);
        }
    }

    public override string GetItemType()
    {
        return "SpriteRenderer";
    }
}