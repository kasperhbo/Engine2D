using Engine2D.Components;
using Engine2D.Core;
using Engine2D.Flags;
using Engine2D.Rendering;
using ImGuiNET;

using Newtonsoft.Json;
using System.Numerics;

namespace Engine2D.GameObjects
{
    public class SpriteColor
    {
        public Vector4 Color = new();

        public SpriteColor()
        {
            Color = new();
        }

        public SpriteColor(Vector4 color)
        {
            Color = color;
        }
    }

    [JsonConverter(typeof(ComponentSerializer))]
    public class SpriteRenderer : Component
    {
        public SpriteColor Color = new SpriteColor();

        [ShowUI(show = false)] private Transform _lastTransform = new();
        [ShowUI(show = false)] SpriteColor _lastColor = new SpriteColor();

        [ShowUI(show = false)]internal bool IsDirty = true;

        [JsonIgnore]internal Texture texture;
        
        public TextureData? textureData;

        internal Vector2[] TextureCoords =
        {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(0, 1)
        };

        public override void Init(Gameobject parent)
        {
            base.Init(parent);
            if(this.textureData != null)
            {
                Console.WriteLine("has texture data, loading texture...." + textureData.texturePath);
                texture = ResourceManager.GetTexture(textureData);
            }
            GameRenderer.AddSpriteRenderer(this);
        }

        public override  void Start()
        {
        }

        public override void EditorUpdate(double dt)
        {
            //Console.WriteLine(this.texture?.TexID);
            if (!_lastTransform.Equals(Parent.transform))
            {
                IsDirty = true;
                Parent.transform.Copy(_lastTransform);
            }

            if (!_lastColor.Color.Equals(Color.Color))
            {
                IsDirty = true;
                _lastColor = new(Color.Color);
            }
        }

        public override void GameUpdate(double dt)
        
        {
            if (!_lastTransform.Equals(Parent.transform))
            {
                IsDirty = true;
                Parent.transform.Copy(_lastTransform);
            }

            if (!_lastColor.Color.Equals(Color.Color))
            {
                IsDirty = true;
                _lastColor = new(Color.Color);
            }
        }

        public override string GetItemType()
        {
            return "SpriteRenderer";
        }
    }
}
