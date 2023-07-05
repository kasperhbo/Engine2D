#region

using System.Numerics;
using Box2D.NetStandard.Dynamics.Bodies;
using Engine2D.Core;
using Engine2D.UI.ImGuiExtension;
using Newtonsoft.Json;
using OpenTK.Windowing.Common;

#endregion

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
public class RigidBody : Component
{
    [JsonProperty]public BodyType BodyType          ;      
    [JsonProperty]public float    GravityScale   = 1;   
    
    [JsonProperty]public float    LinearDamping  = 0;  
    [JsonProperty]public float    AngularDamping = 0; 
    [JsonProperty]public float    Mass           = 1;
    
    [JsonProperty]public bool     FixedRotation  = false;
    [JsonProperty]public bool     Continous      = false;
    

    [JsonIgnore] public Body? RuntimeBody = null;

    public RigidBody() : base()
    {
        
    }

    public override void Init()
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
            if(Parent.Name == "Player")
            {
                // Console.WriteLine(RuntimeBody.get());
            }
            Parent.Transform.Position = RuntimeBody.GetPosition();
        }
    }

    public override void Destroy()
    {
        Engine.Get().CurrentScene._physics2DWorld?.RemoveRigidBody(this);

        base.Destroy();
    }

    public override void StartPlay()
    {
        
    }

    public override void ImGuiFields()
    {
        Gui.DrawProperty("Bodytype: ", ref BodyType);
        base.ImGuiFields();
    }

    public override string GetItemType()
    {
        return this.GetType().FullName;
    }

    public override RigidBody Clone()
    {
        RigidBody clone = new RigidBody();
        
        clone.BodyType      = BodyType      ; 
        clone.FixedRotation = FixedRotation ; 
        clone.GravityScale  = GravityScale  ; 
        
        clone.Mass          = Mass          ;
        
        clone.LinearDamping = LinearDamping ; 
        clone.AngularDamping= AngularDamping; 
        clone.Continous     = Continous     ;
        
        return clone;            
    }
}
