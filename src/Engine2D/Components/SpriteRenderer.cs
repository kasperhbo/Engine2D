using Engine2D.Components;
using Engine2D.Rendering;
using Engine2D.UI;
using ImGuiNET;
using System.Numerics;

namespace Engine2D.GameObjects
{
    internal class SpriteRenderer : Component
    {
        //public Vector2 Position = new();
        //public float rot = 0;

        //public Vector2 Size     = new(32,32);
        //public Vector4 Color { public get; public set { Console.WriteLine(); }       } = new(255,255,255, 255);        

        public Vector4 Color
        {
            get => _color;
            set
            {
                _color = value;
                IsDirty = true;
            }
        }
        public Texture Texture { get; private set; } = null;
        public Vector2 SpriteSize { get; private set; } = new Vector2(32, 32);

        private Transform _lastTransform = new();
        private Vector4 _color = new(255,255,255,255);
        

        internal bool IsDirty = true;



        internal override void Init(Gameobject parent)
        {
            base.Init(parent);
            GameRenderer.AddSpriteRenderer(this);
            parent.transform.Copy(_lastTransform);
        }

        internal void SetTexture(Texture texture)
        {
            Texture = texture;
            IsDirty = true;
        }

      
        internal override  void Start()
        {
        }

        internal override void EditorUpdate(double dt)
        {
            if(!_lastTransform.Equals(Parent.transform))
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
                //if(UIHelper.ColorPicker4("Color: ", ref _color))
                //{
                //    IsDirty = true;
                //}

                //if (Texture != null)
                //    UIHelper.ImageButton("Sprite: ", (IntPtr)Texture.TexID);
                ////ImGui.ImageButton("##sprite", (IntPtr)Texture.TexID, new Vector2(56, 56));
                //else
                //    UIHelper.ImageButton("Sprite: ", IntPtr.Zero);
                //ImGui.ImageButton("##sprite", IntPtr.Zero, new Vector2(56, 56));
            }
        }
    }
}
