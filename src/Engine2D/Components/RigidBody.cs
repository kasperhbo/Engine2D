using Box2DSharp.Dynamics;
using Newtonsoft.Json;

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
public class RigidBody : Component
{
    public bool FixedRotation = false;

    [JsonIgnore] public Body RuntimeBody = null;

    public RigidBody()
    {
    }

    public RigidBody(BodyType bodyType)
    {
        BodyType = bodyType;
    }

    public BodyType BodyType { get; set; }


    public override void GameUpdate(double dt)
    {
        throw new NotImplementedException();
        //Parent.transform.position = RuntimeBody.GetPosition();
        base.GameUpdate(dt);
    }

    public override string GetItemType()
    {
        return "Rigidbody";
    }
}