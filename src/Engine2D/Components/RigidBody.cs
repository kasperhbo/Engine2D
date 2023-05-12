using Box2DSharp.Dynamics;
using Newtonsoft.Json;

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
public class RigidBody : Component
{
    public bool FixedRotation = false;

    [JsonIgnore] public Body runtimeBody = null;

    public RigidBody()
    {
    }

    public RigidBody(BodyType bodyType)
    {
        BodyType = bodyType;
    }

    public BodyType BodyType { get; set; }

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