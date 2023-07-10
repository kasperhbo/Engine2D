﻿#region

using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Engine2D.Components;
using Engine2D.Components.Sprites;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.Flags;
using Engine2D.Logging;
using Engine2D.Rendering;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Windowing.Common;
using ResourceManager = Engine2D.Managers.ResourceManager;

#endregion

namespace Engine2D.GameObjects;


[JsonConverter(typeof(ComponentSerializer))]
public class SpriteRenderer : Component
{
    [JsonIgnore][ShowUI (show = false)] internal bool IsDirty = true;
    [JsonIgnore] internal Sprite? Sprite;    
    [JsonIgnore] private SpriteSheet? _spriteSheet;
    
    [JsonIgnore]
    internal Vector2[] TextureCoords
    {
        get
        {
            
            if (Sprite != null)
            {
                if (FlipX)
                {
                    return new Vector2[]
                    {
                        Sprite.TextureCoords[3],
                        Sprite.TextureCoords[2],
                        Sprite.TextureCoords[1],
                        Sprite.TextureCoords[0]
                    };
                }
                return Sprite.TextureCoords;
            }
            else
                return _defaultTextureCoords;
        }
    }

       
    [JsonIgnore] private Vector2[] _defaultTextureCoords =
    {
        new(1, 1),
        new(1, 0),
        new(0, 0),
        new(0, 1f)
    };

    [JsonIgnore] private Vector2 _lastPosition = new Vector2();
    [JsonIgnore] private Vector2 _currentPosition = new Vector2();
    
    // [JsonIgnore] [ShowUI(show = false)]private Matrix4x4 _lastTranslation { get; set; } = new();
    // [JsonIgnore] [ShowUI(show = false)]private Matrix4x4 _currentTranslation { get; set; } = new();
    [JsonIgnore]  [ShowUI(show = false)] private Vector4 _lastColor { get; set; } = new();
    [JsonIgnore]private int _lastSpriteSheetIndex = -1;
    //[JsonIgnore] private Renderer? _renderer = null;
    
    [JsonProperty][ShowUI (show = false)] internal bool    HasSpriteSheet         = false;
    [JsonProperty][ShowUI (show = false)] internal string? SpriteSheetPath        = "";

    [JsonProperty] internal                       int      ZIndex                 = 0;
    [JsonProperty] internal                       Vector4  Color                  = new(255,255,255,255);
    [JsonProperty] internal                       int      SpriteSheetSpriteIndex = 0;
    [JsonProperty] internal bool FlipX = false;

    private Transform? _transform = null;
    
    public override void Init()
    {
    //     if (_renderer == null)
    //     {
    //         _renderer = Engine.Get().CurrentScene.Renderer;
    //     }
        
        // _renderer.AddSpriteRenderer(this);
        
        // if(Parent?.GetComponent<Transform>()!=null)
        //     _currentTranslation = Parent.GetComponent<Transform>().GetTranslation();
        
        Refresh();
        _transform = Parent.GetComponent<Transform>();
    }
    

    public override void StartPlay()
    {
        
    }


    public override void Update(FrameEventArgs args)
    {
        if (Parent.IsStatic) return;
        if (Color != (_lastColor))
        {
            _lastColor = Color;
            IsDirty = true;
        }
        
        var pos = _transform.Position;
        if (pos != _lastPosition)
        {
            Log.Message("is dirty" + Parent.Name);
            _lastPosition = pos;
            IsDirty = true;
        }
        
        if (SpriteSheetSpriteIndex != _lastSpriteSheetIndex)
        {
            _lastSpriteSheetIndex = SpriteSheetSpriteIndex;
            IsDirty = true;
        }
    }

    public override void EditorUpdate(double dt)
    {
        base.EditorUpdate(dt);
    }
    
    public void SetSprite(int spriteSheetIndex, string spriteSheet)
    {
        // if (_renderer == null)
        // {
        //     _renderer = Engine.Get().CurrentScene.Renderer;
        // }
        // //_renderer.RemoveSprite(this.Parent);
        //
        // var sprs = ResourceManager.GetItem<SpriteSheet>(spriteSheet);
        //
        // if (sprs == null)
        // {
        //     Log.Error("Couldn't find sprite sheet: " + spriteSheet);
        //     return;
        // }
        //
        // HasSpriteSheet = true;
        //
        // _spriteSheet = ResourceManager.GetItem<SpriteSheet>(spriteSheet);
        //
        //
        // Sprite = _spriteSheet.GetSprite(spriteSheetIndex);
        // SpriteSheetPath = spriteSheet;
        // SpriteSheetSpriteIndex = spriteSheetIndex;
        //
        // //_renderer.AddSpriteRenderer(this);
        // IsDirty = true;
    }
    
    public void Refresh()
    {
        if (SpriteSheetPath != "" && HasSpriteSheet == true)
        {
            SetSprite(SpriteSheetSpriteIndex, SpriteSheetPath);
        }
        else
        {
           // _renderer.AddSpriteRenderer(this);
        }

        IsDirty = true;
    }

    public override void Destroy()
    {
        //Engine.Get().CurrentScene.Renderer.RemoveSprite(this);
        base.Destroy();
    }

    public override unsafe void ImGuiFields()
    {
        base.ImGuiFields();
        
        // if(SpriteSheetPath != "")
        //     if (Gui.DrawProperty("Sprite sheet index", ref SpriteSheetSpriteIndex, 0, 
        //             _spriteSheet.Sprites.Count - 1))
        //     {
        //         Refresh();
        //     }
        //
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
                Sprite droppedSprite = DeserializeSprite(payloadData);

                // Do something with the dropped sprite...
                // For example, display a message with the sprite name
                SetSprite(droppedSprite.Index, droppedSprite.FullSavePath);
            }

            ImGui.EndDragDropTarget();
        }
    }

    public override SpriteRenderer Clone()
    {
        SpriteRenderer clone = new SpriteRenderer();

        clone.HasSpriteSheet = this.HasSpriteSheet;
        clone.SpriteSheetPath = this.SpriteSheetPath;
        clone.ZIndex = this.ZIndex;
        clone.Color = this.Color;
        clone.SpriteSheetSpriteIndex = this.SpriteSheetSpriteIndex;

        clone.SetSprite(clone.SpriteSheetSpriteIndex, clone.SpriteSheetPath);
        
        return clone;
    }


    /// <summary>
    /// For loading the sprites from the sprite sheet for drag and drop
    /// </summary>
    /// <param name="sprite"></param>
    /// <returns>byte[] with sprite data</returns>
    // Serialize a sprite into a byte array
    internal static byte[] SerializeSprite(Sprite sprite)
    {
        string jsonString = JsonConvert.SerializeObject(sprite);
        return Encoding.UTF8.GetBytes(jsonString);
    }

    // Deserialize a sprite from a byte array
    public static Sprite DeserializeSprite(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<Sprite>(jsonString);
    }   
}