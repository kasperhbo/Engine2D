using System.Diagnostics;
using System.Numerics;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Engine2D.Components;
using Vortice.Mathematics;


namespace Engine2D.Physics;

internal class Physics2DWorld
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
            bodyDef.GravityScale = 1;
            bodyDef.AngularVelocity = 10;
            

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
            Console.WriteLine("updating world");
            _physicsTime -= _physicsTimeStep;
            _world.Step(_physicsTimeStep, _velocityIterations, _positionIterations);
        }
    }

    private void AddBoxCollider(RigidBody rb, BoxCollider2D boxCollider2D)
    {
        Body body = rb.RuntimeBody;

        PolygonShape shape = new();
        Vector2 halfsize = boxCollider2D.HalfSize;
        Vector2 offset = boxCollider2D.Offset;

        shape.SetAsBox(halfsize.X, halfsize.Y, new Vector2(offset.X, offset.Y), 0);
        FixtureDef fdef = new FixtureDef();
        fdef.Shape = shape;
        fdef.Density = 1.0f;
        fdef.Friction = boxCollider2D.Friction;
        fdef.UserData = boxCollider2D.Parent;
        body.CreateFixture(fdef);
    }
}