#region

using System.Numerics;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using Engine2D.Components;
using Engine2D.Components.Sprites;
using Engine2D.Core;
using Engine2D.Flags;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Engine2D.UI.ImGuiExtension;
using Engine2D.Utilities;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ResourceManager = Engine2D.Managers.ResourceManager;

#endregion

namespace Engine2D.GameObjects;


[JsonConverter(typeof(ComponentSerializer))]
internal class SpriteRenderer : Component
{
    [JsonIgnore][ShowUI (show = false)] internal bool IsDirty = true;
    [JsonIgnore] internal SpriteSheetSprite? Sprite;    
    [JsonIgnore] private SpriteSheet? _spriteSheet;
    
    [JsonIgnore]
    internal Vector2[] TextureCoords
    {
        get
        {
            if (Sprite != null)
                return Sprite.TextureCoords;
            else
                return _defaultTextureCoords;
        }
    }

    
    
    [JsonIgnore] private Renderer? _renderer;
    [JsonIgnore]
    private Vector2[] _defaultTextureCoords =
    {
        new(1, 1),
        new(1, 0),
        new(0, 0),
        new(0, 1f)
    };
    [JsonProperty][ShowUI (show = false)] internal bool HasSpriteSheet = false;
    [JsonProperty][ShowUI (show = false)] internal string? SpriteSheetPath = "";

    [JsonProperty] internal int ZIndex = 0;
    [JsonProperty] internal KDBColor Color = new();
    [JsonProperty] internal int SpriteSheetSpriteIndex = 0;
    

    internal override void Init(Gameobject parent, Renderer? renderer)
    {
        base.Init(parent, renderer);
        _renderer = renderer;
        Initialize();
    }

    internal override void Init(Gameobject parent)
    {
        base.Init(parent);
        
        Initialize();
    }

    private void Initialize()
    {
        if (_renderer == null)
        {
            _renderer = Engine.Get().CurrentScene.Renderer;
        }
        ResourceManager.SpriteRenderers.Add(this);
        
        Refresh();
    }

    public override void EditorUpdate(double dt)
    {
        base.EditorUpdate(dt);
        this.IsDirty = true;
    }

    private void SetSprite(int spriteSheetIndex, string spriteSheet)
    {
        var sprs = ResourceManager.GetItem<SpriteSheet>(spriteSheet);
        if (sprs == null)
        {
            Log.Error("Couldn't find sprite sheet: " + spriteSheet);
            return;
        }
        
        SetSprite(spriteSheetIndex, sprs);
    }

    private void SetSprite(int spriteSheetIndex, SpriteSheet spriteSheet)
    {
        _renderer.RemoveSprite(this);
        HasSpriteSheet = true;
        
        Sprite = spriteSheet.Sprites[spriteSheetIndex];
        SpriteSheetPath = spriteSheet.SavePath;
        SpriteSheetSpriteIndex = spriteSheetIndex;

        _spriteSheet = spriteSheet;
        
        _renderer.AddSpriteRenderer(this);
    }

    public void Refresh()
    {
        if (SpriteSheetPath != "" && HasSpriteSheet == true)
        {
            //Load sprite sheet
            var spriteSheet = ResourceManager.GetItem<SpriteSheet>(SpriteSheetPath);
            SetSprite(SpriteSheetSpriteIndex, spriteSheet);
        }
        else
        {
            _renderer.AddSpriteRenderer(this);
        }
    }

    public override unsafe void ImGuiFields()
    {
        Gui.DrawProperty("ZIndex", ref ZIndex);
        Gui.DrawProperty("Color", ref Color);
        
        if(SpriteSheetPath != "")
            if (Gui.DrawProperty("Sprite sheet index", ref SpriteSheetSpriteIndex, 0, 
                    _spriteSheet.Sprites.Count - 1))
            {
                Refresh();
            }
        
        ImGui.Button("set sprite");
           
        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGui.AcceptDragDropPayload("spritesheet_drop");
            if (payload.IsValidPayload())
            {
                // Retrieve the payload data as byte array
                byte[] payloadData = new byte[payload.DataSize];
                Marshal.Copy(payload.Data, payloadData, 0, payloadData.Length);

                // Deserialize the sprite from the payload data
                SpriteSheetSprite droppedSprite = DeserializeSprite(payloadData);

                // Do something with the dropped sprite...
                // For example, display a message with the sprite name
                SetSprite(droppedSprite.Index, droppedSprite.FullSavePath);
            }

            ImGui.EndDragDropTarget();
        }
    }
    
    
    /// <summary>
    /// For loading the sprites from the sprite sheet for drag and drop
    /// </summary>
    /// <param name="sprite"></param>
    /// <returns>byte[] with sprite data</returns>
    // Serialize a sprite into a byte array
    internal static byte[] SerializeSprite(SpriteSheetSprite sprite)
    {
        string jsonString = JsonConvert.SerializeObject(sprite);
        return Encoding.UTF8.GetBytes(jsonString);
    }

    // Deserialize a sprite from a byte array
    internal static SpriteSheetSprite DeserializeSprite(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<SpriteSheetSprite>(jsonString);
    }
}