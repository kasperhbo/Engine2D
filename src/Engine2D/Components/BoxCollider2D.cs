using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Components
{
    [JsonConverter(typeof(ComponentSerializer))]
    public class BoxCollider2D : Component
    {
        public Vector2 Offset = new Vector2();
        public Vector2 Size = new Vector2(0.5f, 0.5f);

        public float Density = 1.0f;
        public float Friction = 0.5f;
        public float Restitution = 0.0f;
        public float RestitutionThreshold = 0.5f;

        public override void ImGuiFields()
        {
        }

        public override string GetItemType()
        {
            Logging.Log.Warning("Getting type");
            return "BoxCollider2D";
        }

        public override System.Numerics.Vector2 WindowSize()
        {
            return new System.Numerics.Vector2(0, 100);
        }

    }
}
