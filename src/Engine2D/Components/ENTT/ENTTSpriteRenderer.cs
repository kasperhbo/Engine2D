using System.Numerics;
using Engine2D.Rendering.NewRenderer;
using Newtonsoft.Json;
using Serilog;

namespace Engine2D.Components.ENTT;

public struct ENTTSpriteRenderer : IENTTComponent
{
   [JsonProperty] public Vector4 Color = new Vector4(255, 255, 255, 255);
   [JsonProperty]public Vector2[] TextureCoords = new Vector2[4];
   [JsonProperty]public int ParentUUID = 0;
   
   [JsonIgnore]public Entity? Parent = null;

   [JsonConstructor]
   public ENTTSpriteRenderer(Vector4 color, Vector2[] textureCoords, int parentUuid)
   {
      Init(color, textureCoords, parentUuid);
   }
   
   public ENTTSpriteRenderer(int parentUuid)
   {
      var textureCoords = new Vector2[4]
      {
         new Vector2(0f, 0f),
         new Vector2(1f, 0f),
         new Vector2(1f, 1f),
         new Vector2(0f, 1f),
      };
      
      Init(new Vector4(255,255,255,255), textureCoords, parentUuid);
   }

   private void Init(Vector4 color, Vector2[] textureCoords, int parentUuid)
   {
      this.Color = color;
      this.TextureCoords = textureCoords;
      this.ParentUUID = parentUuid;
      Parent = Engine2D.Core.Engine.Get().CurrentScene.FindEntityByUUID(ParentUUID);
      
      if(Parent != null)
      {
         Parent.SetComponent(this);
         Renderer.AddSprite(Parent);
      }
      else
      {
         Log.Error("Parent not found: " + ParentUUID);
      }
   }
}