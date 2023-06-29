using System.Diagnostics;
using System.Numerics;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Engine2D.Components;
using Engine2D.GameObjects;
using Vortice.Mathematics;


namespace Engine2D.Physics;

public class Physics2DWorld
{
    private World _world = new World(new Vector2(0,-10.0f));
    private float _physicsTime = 0.0f;
    private float _physicsTimeStep = 1.0f / 60.0f;
    private int _velocityIterations = 8;
    private int _positionIterations = 3;

    internal Physics2DWorld()
    {
        _world = new World(new Vector2(0,-10.0f));
    }

    internal void AddRigidbody(RigidBody rb)
    {
        if (rb.RuntimeBody == null && rb.Parent != null)
        {
            BodyDef bodyDef = new BodyDef();
            bodyDef.BodyType = rb.BodyType;
            bodyDef.FixedRotation = rb.FixedRotation;
            bodyDef.Position = rb.Parent.Transform.Position;
            bodyDef.GravityScale = rb.GravityScale;
            bodyDef.AngularVelocity = rb.AngleVelocity;
            
            bodyDef.LinearVelocity = rb.Velocity;
            
            bodyDef.LinearDamping = rb.LinearDamping;
            bodyDef.AngularDamping = rb.AngularDamping;

            rb.RuntimeBody = _world.CreateBody(bodyDef);
            
            bodyDef.UserData = rb.Parent;
            
            switch (rb.BodyType)
            {
                case BodyType.DynamicBody:
                    bodyDef.BodyType = BodyType.DynamicBody;
                    break;
                case BodyType.KinematicBody:
                    bodyDef.BodyType = BodyType.KinematicBody;
                    break;
                case BodyType.StaticBody:
                    bodyDef.BodyType = BodyType.StaticBody;
                    break;
            }
            
            Body body = _world.CreateBody(bodyDef);
            
            BoxCollider2D boxCollider2D = rb.Parent.GetComponent<BoxCollider2D>();
            if (boxCollider2D != null)
            {
                AddBoxCollider(rb, boxCollider2D);
            }
            
        }
    }

    internal void GameUpdate(float dt)
    {
        _physicsTime += dt;
        if (_physicsTime >= 0.0f) {
            _physicsTime -= _physicsTimeStep;
            _world.Step(_physicsTimeStep, _velocityIterations, _positionIterations);
        }
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
        fdef.Shape = shape;
        fdef.Density = 1.0f;
        fdef.Restitution = boxCollider2D.Restitution;
        fdef.Friction = boxCollider2D.Friction;
        fdef.UserData = boxCollider2D.Parent;
        body.CreateFixture(fdef);
    }
    
    // Create a raycaster
    public bool Raycast(Vector2 startPoint, Vector2 endPoint)
    {
        GroundedRaycastCallback callback = new GroundedRaycastCallback();
        // Perform the raycast
        _world.RayCast(callback, startPoint, endPoint);
        return callback.IsGrounded;
    }
}

class GroundedRaycastCallback : IRayCastCallback
{
    public bool IsGrounded { get; private set; }

    public GroundedRaycastCallback()
    {
        IsGrounded = false;
    }

    public float RayCastCallback(Fixture fixture, in Vector2 point, in Vector2 normal, float fraction)
    {
        // Check if the fixture belongs to the ground or platform
        if (fixture.Body.BodyType == BodyType.StaticBody && fixture.UserData != null)
        {
            // Set the grounded flag to true
            IsGrounded = true;

            // Stop the raycast since we found a ground or platform
            return 0;
        }

        // Continue raycasting to find any ground or platform fixtures (return 1 to continue, 0 to stop)
        return 1;
    }
}