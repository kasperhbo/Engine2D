using System.Numerics;
using Engine2D.Components;
using Engine2D.Components.TransformComponents;
using System.Numerics;
using Engine2D.GameObjects;
using Engine2D.Logging;
using KDBEngine.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace Engine2D.Testing;

public class TestCamera : Component
{
    public TestCamera(Vector2 projectionSize)
    {
        _projectionSize = projectionSize;
    }
    
    private float _zoom = 1;
    
    public Vector2 ProjectionSize => _projectionSize;
    public float Zoom  => _zoom;
    public KDBColor ClearColor { get; set; } = new();

    private Vector2 _projectionSize;

    private Vector3 _front = -Vector3.UnitZ;
    private Vector3 _up = Vector3.UnitY;
    private Vector3 _right = Vector3.UnitX;

    public Matrix4x4 GetViewMatrix()
    {
        return Matrix4x4.CreateLookAt(new System.Numerics.Vector3(
                Parent.Transform.Position.X, Parent.Transform.Position.Y, 20.0f), // Is the 20.0f something i want to scroll to zoom in and out?
            _front + new System.Numerics.Vector3(
                Parent.Transform.Position.X, Parent.Transform.Position.Y, 0.0f),
            _up
        );
    }
    
    
    
    public Matrix4x4 GetProjectionMatrix()
    {
        Matrix4x4 projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(
            -(_projectionSize.X * _zoom)/2,
            (_projectionSize.X * _zoom)/2,
            -(_projectionSize.Y * _zoom)/2,
            (_projectionSize.Y * _zoom)/2,
            0,
            100
        );

        return projectionMatrix;
    }

    public override string GetItemType()
    {
        return "Camera";
    }

    public override void EditorUpdate(double dt)
    {
        base.EditorUpdate(dt);

        Log.Message("Updating Vectors Camera");
        var rot = Parent.Transform.Rotation;
        MathUtils.UpdateVectors(rot.Quaternion, _front: out _front, _right: out _right, _up: out _up);
        //
        // if (rot.EulerDegrees.Yaw <= 90)
        // {
        //     _up = ;
        // }
    }



    public void AdjustProjection(float X, float Y)
    {
        _projectionSize.X = X;
        _projectionSize.Y = Y;
    }
}