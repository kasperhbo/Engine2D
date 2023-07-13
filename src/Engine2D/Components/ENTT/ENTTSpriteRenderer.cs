using System.Numerics;
using Engine2D.Components.Sprites;
using Engine2D.Core;
using Engine2D.Rendering;
using Engine2D.Rendering.NewRenderer;
using Newtonsoft.Json;
using Serilog;

namespace Engine2D.Components.ENTT;

/// <summary>
/// SPRITE RENDERER MEGA STRUCT <3 <3 <3 <3 <3 <3   
/// </summary>
public struct ENTTSpriteRenderer : IENTTComponent
{
   [JsonProperty] public Vector4 Color = new Vector4(1, 1, 1, 1);
   [JsonProperty] public int ParentUUID = 0;

   [JsonProperty]
   public Vector2[] TextureCoords
   {
      get
      {
         if (Sprite != null)
         {
            return Sprite.TextureCoords;
         }
         else
         {
            return new Vector2[4]
            {
               new Vector2(0f, 0f),
               new Vector2(1f, 0f),
               new Vector2(1f, 1f),
               new Vector2(0f, 1f)
            };
         }  
      }
   }
   
   [JsonProperty] internal string SpritePath = "";
   
   [JsonIgnore]private Sprite? _sprite = null;
   [JsonIgnore]internal Sprite? Sprite
   {
      get
      {
         return _sprite;
      }
      set
      {
         if(value == null)
         {
            _sprite = value;
            return;
         }
         _sprite = value;
         SpritePath = value.SavePath;
      }
   }

   public ENTTSpriteRenderer()
   {
      
   }
}