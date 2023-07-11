using System.Numerics;
using Engine2D.Components.Sprites;
using Engine2D.Flags;
using Engine2D.Rendering.NewRenderer;
using Newtonsoft.Json;

namespace Engine2D.Components.ENTT;

public struct ENTTSpriteRenderer : IENTTComponent
{
    //serialization
    [JsonProperty][ShowUI (show = false)] internal bool    HasSpriteSheet         = false;
    [JsonProperty][ShowUI (show = false)] internal string? SpriteSheetPath        = "";
    
    [JsonProperty] internal                       int      ZIndex                 = 0;
    [JsonProperty] internal                       Vector4  Color                  = new(255,255,255,255);
    [JsonProperty] internal                       int      SpriteSheetSpriteIndex = 0;
    [JsonProperty] internal bool FlipX                                            = false;
    
    //runtime
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
    
    [JsonIgnore]  [ShowUI(show = false)] private Vector4 _lastColor { get; set; } = new();
    [JsonIgnore]internal Entity Parent { get; set; }

    [JsonIgnore]private int _lastSpriteSheetIndex = -1;
    
    public ENTTSpriteRenderer()
    {
        
    }

    /// <summary>
    /// Make sure to call this
    /// </summary>
    /// <param name="parent"></param>
    internal void SetParent(Entity parent)
    {
        Parent = parent;
    }
    
    public void SetSprite(int spriteSheetIndex, string spriteSheet)
    {
        Renderer.RemoveSprite(this);
        
        SpriteSheetPath = spriteSheet;
        SpriteSheetSpriteIndex = spriteSheetIndex;
        
        IsDirty = true;
        Renderer.AddSprite(this);
    }
}