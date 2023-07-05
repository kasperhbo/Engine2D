using System.Numerics;

using Box2D.NetStandard.Collision.Shapes;
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Dynamics.World;

using Engine2D.Components;


namespace Engine2D.Physics;

public class Physics2DWorld
{
    private World _world = new World();

    private int _velocityIterations = 8;
    private int _positionIterations = 5;

    internal Physics2DWorld()
    {
        _world = new World(new Vector2(0, -9.81F));
    }

    internal void AddRigidbody(RigidBody rb)
    {
        if (rb.RuntimeBody == null && rb.Parent != null)
        {
            BodyDef bodyDef = new BodyDef();
            
            bodyDef.fixedRotation  = rb.FixedRotation  ;
            bodyDef.gravityScale   = rb.GravityScale   ;
              
            bodyDef.linearDamping  = rb.LinearDamping  ;
            bodyDef.angularDamping = rb.AngularDamping ;
            bodyDef.bullet         = rb.Continous      ;
            bodyDef.position       = rb.Parent.Transform.Position;

            bodyDef.userData = rb.Parent;
            
            switch (rb.BodyType)
            {
                case BodyType.Dynamic:
                    bodyDef.type = BodyType.Dynamic;
                    break;
                case BodyType.Kinematic:
                    bodyDef.type = BodyType.Kinematic;
                    break;
                case BodyType.Static:
                    bodyDef.type = BodyType.Static;
                    break;
            }

            rb.RuntimeBody = _world.CreateBody(bodyDef);
            rb.RuntimeBody.SetMassData(new MassData()
            {   
                mass = rb.Mass,
                center = rb.RuntimeBody.GetPosition(),
            });
            
            
            BoxCollider2D boxCollider2D = rb.Parent.GetComponent<BoxCollider2D>();
            if (boxCollider2D != null)
            {
                AddBoxCollider(rb, boxCollider2D);
            }
            
        }
    }

    internal void RemoveRigidBody(RigidBody rb)
    {
        if(rb.RuntimeBody != null)
            _world.DestroyBody(rb.RuntimeBody);
    }

    internal void GameUpdate(float dt)
    {
      
    }

    private void AddBoxCollider(RigidBody rb, BoxCollider2D boxCollider2D)
    {
        Body body = rb.RuntimeBody;

        PolygonShape shape = new();
        
        Vector2 offset = boxCollider2D.Offset;

        float x = (boxCollider2D.Parent.Transform.GetFullSize(true).X/2);
        float y = (boxCollider2D.Parent.Transform.GetFullSize(true).Y/2);
        
        Vector2 halfSize = new(x, y);
        
        shape.SetAsBox(halfSize.X, halfSize.Y, new Vector2(0,0), 0);
        
        FixtureDef fdef = new FixtureDef();
        
        fdef.shape = shape;
        fdef.density = boxCollider2D.Density;
        fdef.restitution = boxCollider2D.Restitution;
        fdef.friction = boxCollider2D.Friction;
        fdef.userData = boxCollider2D.Parent;

        body.CreateFixture(fdef);
    }

    public void FixedGameUpdate(float physicsTimeStep)
    {
        _world.Step(physicsTimeStep, _velocityIterations, _positionIterations);
    }
}