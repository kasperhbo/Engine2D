using System.Runtime.InteropServices;
using Engine2D.Components;
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
}

[JsonConverter(typeof(ComponentSerializer))]
public class SpriteRenderer : Component
{
    public KDBColor Color = new();
    
    [ShowUI(show = false)] private Transform? _lastTransform = new Transform();
    [ShowUI(show = false)] private KDBColor _lastColor = new();
    [JsonIgnore] [ShowUI(show = false)] private int _prevZIndex;
    [JsonIgnore]private Renderer _renderer;
    [JsonIgnore]public Sprite? Sprite = null;
    public string _spritePath = "";

    [ShowUI(show = false)] internal bool IsDirty = true;

    // 0.5f,   0.5f, 0.0f,    1.0f, 1.0f,   // top right
    // 0.5f,  -0.5f, 0.0f,    1.0f, 0.0f,   // bottom right
    // -0.5f, -0.5f, 0.0f,    0.0f, 0.0f,   // bottom left
    // -0.5f,  0.5f, 0.0f,    0.0f, 1.0f    // top left 
    private Vector2[] _defaultTextureCoords =
    {
        new(1.0f, 1.0f),   
        new(1.0f, 0.0f),   
        new(0.0f, 0.0f),   
        new(0.0f, 1.0f)    
    };

    public int ZIndex = 0;
    
    public Vector2[] GetTextureCoords()
    {
        if (Sprite != null)
        {
            return Sprite.TextureCoords;
        }

        return _defaultTextureCoords;
    }
    
    public override void Init(Gameobject parent, Renderer? renderer)
    {
        base.Init(parent, renderer);
        
        renderer.AddSpriteRenderer(this);
        _renderer = renderer;

        if (_spritePath != "")
        {
            SetSprite(SaveLoad.LoadSpriteFromJson(_spritePath));
        }
    }

    public override void Start()
    {
    }

    public override void EditorUpdate(double dt)
    {
        //Console.WriteLine(this.texture?.TexID);
        if (_lastTransform.Equals(Parent.GetComponent<Transform>())
            );
        {
            IsDirty = true;
            Parent.GetComponent<Transform>().Copy(_lastTransform);
        }

        if (!_lastColor.Equals(Color))
        {
            IsDirty = true;
            _lastColor = new KDBColor(Color.RNormalized, Color.GNormalized, Color.BNormalized, Color.ANormalized);
        }

        if (!_prevZIndex.Equals(ZIndex))
        {
            _prevZIndex = ZIndex;
            throw new NotImplementedException();
            IsDirty = true;
        }
    }

    public override void GameUpdate(double dt)
    {
    }

    public override float GetFieldSize()
    {
        return 120;
    }

    public override void ImGuiFields()
    {
        base.ImGuiFields();

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.Button("Sprite"))
        {
            Sprite sprite =
                SaveLoad.LoadSpriteFromJson("D:\\dev\\EngineDev\\Engine2D\\\\src\\ExampleGame\\sprite.sprite");
            SetSprite(sprite);
            
            // SpriterendererManager.AddSpriteRenderer(
            //     "D:\\dev\\EngineDev\\Engine2D\\\\src\\ExampleGame\\sprite.sprite",
            //     this
            //     );
        }
        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGui.AcceptDragDropPayload("Sprite_Drop");
            if (payload.IsValidPayload())
            {
                var filename = (string)GCHandle.FromIntPtr(payload.Data).Target;
                Sprite sprite = SaveLoad.LoadSpriteFromJson(filename);
                if (sprite != null)
                {
                    SetSprite(sprite);
                }
                else
                {
                    Log.Error("Cant load sprite " + filename);
                }
            }
            ImGui.EndDragDropTarget();
        }
        
    }

    public void SetSprite(Sprite sprite)
    {
        _spritePath = sprite.FullSavePath;
        SpriterendererManager.AddSpriteRenderer(sprite.FullSavePath, this);
        this._renderer.RemoveSprite(this);
        this.Sprite = sprite;
        this._renderer.AddSpriteRenderer(this);
    }

    public override string GetItemType()
    {
        return "SpriteRenderer";
    }
}