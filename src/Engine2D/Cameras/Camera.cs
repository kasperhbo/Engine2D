using System.Numerics;
using Engine2D.Components;
using Engine2D.GameObjects;
using Engine2D.UI;
using ImGuiNET;

namespace Engine2D.Cameras;

public class Camera : Component
{
    public CameraTypes CameraType = CameraTypes.ORTHO;
    
    private float _size = 10f;
    private float _near = 0.1f;
    private float _far = 1000f;
    
    private Vector2 _projectionSize = new();
    private Matrix4x4 _projectionMatrix = Matrix4x4.Identity;
    
    public Vector2 ProjectionSize => _projectionSize;
    public float Size  => _size;
    public KDBColor ClearColor { get; set; } = new();
    
    public Camera(Vector2 projectionSize)
    {
        _projectionSize = projectionSize;
    }
    
    public Camera(Vector2 projectionSize, float size)
    {
        _projectionSize = projectionSize;
        _size = size;
    }
    
    public Camera(Vector2 projectionSize, float size, float near, float far)
    {
        _projectionSize = projectionSize;
        _size = size;
        _near = near;
        _far = far;
    }

    public Matrix4x4 GetViewMatrix()
    {
        return Matrix4x4.CreateLookAt(new System.Numerics.Vector3(
                Parent.Transform.Position.X, Parent.Transform.Position.Y, 20.0f),
            Parent.Transform.Rotation.Front + new System.Numerics.Vector3(
                Parent.Transform.Position.X, Parent.Transform.Position.Y, 0.0f),
            Parent.Transform.Rotation.Up
        );
    }
    
    public Matrix4x4 GetProjectionMatrix()
    {
        float aspect = (float)_projectionSize.X / (float)_projectionSize.Y;

        float width = _size * aspect;
        float height = _size;
        
        if(CameraType == CameraTypes.ORTHO)
        {
            _projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(
                -width * 0.5f,
                width * 0.5f,
                -height * 0.5f,
                height * 0.5f,
                _near,
                _far
            );
        }

        if (CameraType == CameraTypes.PERSPECTIVE)
        {
            throw new NotImplementedException();
        }
        

        return _projectionMatrix;
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
        if (OpenTKUIHelper.DrawProperty("Size: ", ref _size, label: false))
        {
        }
        
        ImGui.Separator();
        ImGui.Text("Clipping Planes");

        if (OpenTKUIHelper.DrawProperty("Near: ", ref _near, label: false) ||
            OpenTKUIHelper.DrawProperty("Far: ", ref _far, label: false))
        {
        }   
    }
}

public enum CameraTypes
{
    ORTHO,
    PERSPECTIVE
}