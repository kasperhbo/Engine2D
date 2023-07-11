namespace Engine2D.Components.ENTT;

public struct ENTTSpriteRenderer : IENTTComponent
{
    public Entity Parent;
    // // [JsonIgnore][ShowUI (show = false)] internal bool IsDirty = true;
    // // [JsonIgnore] internal Sprite? Sprite;    
    // // [JsonIgnore] internal Entity Parent;
    // //
    // // [JsonIgnore] private SpriteSheet? _spriteSheet;
    //
    // [JsonIgnore]
    // internal Vector2[] TextureCoords
    // {
    //     get
    //     {
    //         
    //         if (Sprite != null)
    //         {
    //             if (FlipX)
    //             {
    //                 return new Vector2[]
    //                 {
    //                     Sprite.TextureCoords[3],
    //                     Sprite.TextureCoords[2],
    //                     Sprite.TextureCoords[1],
    //                     Sprite.TextureCoords[0]
    //                 };
    //             }
    //             return Sprite.TextureCoords;
    //         }
    //         else
    //             return _defaultTextureCoords;
    //     }
    // }
    //
    //    
    // [JsonIgnore] private Vector2[] _defaultTextureCoords =
    // {
    //     new(1, 1),
    //     new(1, 0),
    //     new(0, 0),
    //     new(0, 1f)
    // };
    //
    // [JsonIgnore] private Vector2 _lastPosition = new Vector2();
    // [JsonIgnore] private Vector2 _currentPosition = new Vector2();
    //
    // // [JsonIgnore] [ShowUI(show = false)]private Matrix4x4 _lastTranslation { get; set; } = new();
    // // [JsonIgnore] [ShowUI(show = false)]private Matrix4x4 _currentTranslation { get; set; } = new();
    // [JsonIgnore]  [ShowUI(show = false)] private Vector4 _lastColor { get; set; } = new();
    // [JsonIgnore]private int _lastSpriteSheetIndex = -1;
    // //[JsonIgnore] private Renderer? _renderer = null;
    //
    // [JsonProperty][ShowUI (show = false)] internal bool    HasSpriteSheet         = false;
    // [JsonProperty][ShowUI (show = false)] internal string? SpriteSheetPath        = "";
    //
    // [JsonProperty] internal                       int      ZIndex                 = 0;
    // [JsonProperty] internal                       Vector4  Color                  = new(255,255,255,255);
    // [JsonProperty] internal                       int      SpriteSheetSpriteIndex = 0;
    // [JsonProperty] internal bool FlipX                                            = false;
    //
    // public ENTTSpriteRenderer(Entity parent)
    // {
    //     Parent = parent;
    //     
    //     Sprite = null;
    //     _spriteSheet = null;
    // }
    //
    // public void SetSprite(int spriteSheetIndex, string spriteSheet)
    // {
    //     Renderer.RemoveSprite(this);
    //     var sprs = ResourceManager.GetItem<SpriteSheet>(spriteSheet);
    //     
    //     if (sprs == null)
    //     {
    //         Log.Error("Couldn't find sprite sheet: " + spriteSheet);
    //         return;
    //     }
    //     
    //     HasSpriteSheet = true;
    //     
    //     _spriteSheet = ResourceManager.GetItem<SpriteSheet>(spriteSheet);
    //     
    //     
    //     Sprite = _spriteSheet.GetSprite(spriteSheetIndex);
    //     SpriteSheetPath = spriteSheet;
    //     SpriteSheetSpriteIndex = spriteSheetIndex;
    //     
    //     IsDirty = true;
    //     Renderer.AddSprite(this);
    // }
    //
    // public void OnGui()
    // {
    //     
    // }
    
    public ENTTSpriteRenderer(Entity parent)
    {
        Parent = parent;
    }
    
    public void OnGui()
    {
        
    }
}