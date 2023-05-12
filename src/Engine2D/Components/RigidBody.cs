using Box2DSharp.Dynamics;
using Engine2D.Flags;
using Engine2D.Rendering;
using Engine2D.UI;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Components
{
    [JsonConverter(typeof(ComponentSerializer))]
    public class RigidBody : Component
    {
        public BodyType BodyType { get; set; }
        public bool FixedRotation = false;

        [JsonIgnore] public Body runtimeBody = null;

        public RigidBody()
        {
            
        }

        public RigidBody(BodyType bodyType) {
            BodyType = bodyType;
        }

        //public override void ImGuiFields()
        //{
        //    if(ImGui.BeginCombo("##combo", BodyType.ToString()))
        //    {
        //        if (ImGui.Selectable("Dynamic"))
        //        {
        //            BodyType = BodyType.DynamicBody;
        //        }
        //        if (ImGui.Selectable("Static"))
        //        {
        //            BodyType = BodyType.StaticBody;
        //        }
        //        if (ImGui.Selectable("Kinematic"))
        //        {
        //            BodyType = BodyType.KinematicBody;
        //        }
        //        ImGui.EndCombo();
        //    }
        //}

        public override void GameUpdate(double dt)
        {
            Parent.transform.position = runtimeBody.GetPosition();
            base.GameUpdate(dt);
        }

        public override string GetItemType()
        {
           return "Rigidbody";
        }       
    }
}
