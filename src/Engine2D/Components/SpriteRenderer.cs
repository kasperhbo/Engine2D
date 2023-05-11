using Engine2D.Components;
using Engine2D.Core;
using Engine2D.Rendering;
using Engine2D.UI;
using ImGuiNET;
using Newtonsoft.Json;
using System.Numerics;

namespace Engine2D.GameObjects
{
    internal class SpriteRenderer : Component
    {
        public Vector4 Color
        {
            get => _color;
            set
            {
                _color = value;
                IsDirty = true;
            }
        }

//        public Sprite? sprite = null;
        public Vector2 SpriteSize { get; private set; } = new Vector2(32, 32);

        private Transform _lastTransform = new();
        private Vector4 _color = new(1,1,1,1);
        
        internal bool IsDirty = true;

        [JsonIgnore]internal Texture texture;
        
        public TextureData? textureData;
        internal Vector2[] TextureCoords =
        {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(0, 1)
        };

        internal override void Init(Gameobject parent)
        {
            base.Init(parent);
            if(this.textureData != null)
            {
                Console.WriteLine("has texture data, loading texture...." + textureData.texturePath);
                texture = ResourceManager.GetTexture(textureData);
            }
            GameRenderer.AddSpriteRenderer(this);
        }

        internal override  void Start()
        {
        }

        internal override void EditorUpdate(double dt)
        {
            //Console.WriteLine(this.texture?.TexID);
            if (!_lastTransform.Equals(Parent.transform))
            {
                IsDirty = true;
                Parent.transform.Copy(_lastTransform);
            }
        }

        internal override void GameUpdate(double dt)
        {
            if (!_lastTransform.Equals(Parent.transform))
            {
                IsDirty = true;
                Parent.transform.Copy(_lastTransform);
            }
        }

        internal override void SetType()
        {
            Type = "SpriteRenderer";
        }

        internal override void ImGuiFields()
        {
            if(ImGui.CollapsingHeader("Sprite Renderer"))
            {
                if (ImGui.ColorPicker4("Color: ", ref _color))
                {
                    IsDirty = true;
                }

                if (this.texture != null)
                    ImGui.ImageButton("Sprite: ", (IntPtr)texture.TexID, new Vector2(128, 128));
                //ImGui.ImageButton("##sprite", (IntPtr)Texture.TexID, new Vector2(56, 56));
                else
                    ImGui.ImageButton("Sprite: ", IntPtr.Zero, new Vector2(128,128));
                //ImGui.ImageButton("##sprite", IntPtr.Zero, new Vector2(56, 56));
            }
        }
    }
}
