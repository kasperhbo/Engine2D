#region

using Box2DSharp.Dynamics;
using Newtonsoft.Json;

#endregion

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
internal class RigidBody : Component
{
    [JsonProperty]internal bool FixedRotation = false;

    [JsonIgnore] internal Body RuntimeBody = null;

    internal RigidBody()
    {
    }

    internal RigidBody(BodyType bodyType)
    {
        BodyType = bodyType;
    }

    internal BodyType BodyType { get; set; }


    public override void GameUpdate(double dt)
    {
        throw new NotImplementedException();
        //Parent.transform.position = RuntimeBody.GetPosition();
        base.GameUpdate(dt);
    }

    public override string GetItemType()
    {
        return this.GetType().FullName;
    }
}