#region

using System.Numerics;
using Box2DSharp.Dynamics;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Windowing.Common;

#endregion

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
public class RigidBody : Component
{
    [JsonProperty]public bool     FixedRotation  = false;
    [JsonProperty]public BodyType BodyType          ;      
    [JsonProperty]public float    GravityScale   = 1;   
    [JsonProperty]public float    AngleVelocity  = 0;  
    [JsonProperty]public Vector2  Velocity       = new(0,0);     
    [JsonProperty]public float    LinearDamping  = 0;  
    [JsonProperty]public float    AngularDamping = 0; 

    [JsonIgnore] public Body? RuntimeBody = null;

    public RigidBody() : base()
    {
    }

    public override void Update(FrameEventArgs args)
    {
        
    }

    public void SetVelocity(Vector2 vel)
    {
        RuntimeBody?.ApplyForceToCenter(vel, true);
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
        RigidBody clone = new RigidBody();
        clone.FixedRotation = FixedRotation ; 
        clone.BodyType      = BodyType      ; 
        clone.GravityScale  = GravityScale  ; 
        clone.AngleVelocity = AngleVelocity ; 
        clone.Velocity      = Velocity      ; 
        clone.LinearDamping = LinearDamping ; 
        clone.AngularDamping= AngularDamping; 
        return clone;            
    }
}
