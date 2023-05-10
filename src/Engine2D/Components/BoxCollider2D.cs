using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Components
{
    internal class BoxCollider2D : Component
    {
        public Vector2 Offset = new Vector2();
        public Vector2 Size = new Vector2(0.5f, 0.5f);

        public float Density = 1.0f;
        public float Friction = 0.5f;
        public float Restitution = 0.0f;
        public float RestitutionThreshold = 0.5f;

        internal override void ImGuiFields()
        {
        }

        internal override void SetType()
        {
            Type = "BoxCollider2D";
        }
    }
}
