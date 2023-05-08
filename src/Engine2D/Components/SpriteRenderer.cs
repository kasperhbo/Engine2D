using Engine2D.Components;
using Engine2D.Rendering;
using ImGuiNET;
using System.Numerics;

namespace Engine2D.GameObjects
{
    internal class SpriteRenderer : Component
    {
        //public Vector2 Position = new();
        //public float rot = 0;

        //public Vector2 Size     = new(32,32);
        public Vector4 Color    = new(255,0,0, 255);

        internal bool IsDirty = true;


        internal override void Init(Gameobject parent)
        {
            base.Init(parent);
            GameRenderer.AddSpriteRenderer(this);
        }

        internal override  void Start()
        {
        }

        internal override  void Update(double dt)
        {
        }

        internal override  void Destroy()
        {
        }

        internal override void SetType()
        {
            Type = "SpriteRenderer";
        }

        internal override void ImGuiFields()
        {
            ImGui.ColorEdit4("Color: ", ref Color);
        }
    }
}
