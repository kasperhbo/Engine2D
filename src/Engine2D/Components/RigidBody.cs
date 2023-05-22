using Box2DSharp.Dynamics;
using Newtonsoft.Json;

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
public class RigidBody : Component
{
    public bool FixedRotation = false;
    public BodyType BodyType { get; set; }

    [JsonIgnore] public Body runtimeBody = null;

    public RigidBody()
    {
        
    }

    public RigidBody(BodyType bodyType)
    {
        BodyType = bodyType;
    }


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