using System.Numerics;
using Engine2D.Components;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.UI;
using ImGuiNET;

namespace Engine2D.Testing;

public class TestCamera : Component
{
    public TestCamera(Vector2 projectionSize)
    {
        _projectionSize = projectionSize;
    }
    
    private float _size = 10f;
    private float _near = 0.3f;
    private float _far = 100f;
    public Vector2 ProjectionSize => _projectionSize;
    public float Size  => _size;
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
        float aspect = (float)_projectionSize.X / (float)_projectionSize.Y;

        float width = _size * aspect;
        float height = _size;
        
        Matrix4x4 projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(
            -width * 0.5f,
            width * 0.5f,
            -height * 0.5f,
            height * 0.5f,
            _near,
            _far
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
    }

    public void AdjustProjection(float X, float Y)
    {
        _projectionSize.X = X;
        _projectionSize.Y = Y;
    }

    public override float GetFieldSize()
    {
        return 100;
    }

    public override void ImGuiFields()
    {
        OpenTKUIHelper.DrawProperty("Size: ", ref _size, label: false);
        ImGui.Separator();
        ImGui.Text("Clipping Planes");
        
        OpenTKUIHelper.DrawProperty("Near: ", ref _near, label: false);
        OpenTKUIHelper.DrawProperty("Far: ", ref _far, label: false);
    }
}