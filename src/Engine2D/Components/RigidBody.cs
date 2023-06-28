#region

using Box2DSharp.Dynamics;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Windowing.Common;

#endregion

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
internal class RigidBody : Component
{
    [JsonProperty]internal bool FixedRotation = false;
    [JsonProperty] internal BodyType BodyType;
    

    [JsonIgnore] internal Body? RuntimeBody = null;

    internal RigidBody() : base()
    {
    }

    public override void Update(FrameEventArgs args)
    {
        
    }

    public override void GameUpdate(double dt)
    {
        base.GameUpdate(dt);
        if (RuntimeBody != null)
        {
            Parent.Transform.Position = RuntimeBody.GetPosition();
        }
    }

    public override void StartPlay()
    {
        
    }

    public override void ImGuiFields()
    {
        Gui.DrawProperty("Bodytype: ", ref BodyType);
    }

    public override string GetItemType()
    {
        return this.GetType().FullName;
    }

    public override RigidBody Clone()
    {
        RigidBody rb = new RigidBody();
        rb.FixedRotation = FixedRotation;
        rb.BodyType = BodyType;
        return rb;
    }
}
