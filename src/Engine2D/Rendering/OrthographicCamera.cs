using Engine2D.GameObjects;
using OpenTK.Mathematics;
using Vector2 = System.Numerics.Vector2;

namespace Engine2D.Rendering;

public class OrthographicCamera : GameObject
{
    private readonly Vector3 _front = -Vector3.UnitZ;
    private readonly Vector3 _up = Vector3.UnitY;

    private float _size;
    private Matrix4 _viewMatrix = Matrix4.Identity;

    internal bool IsMainCamera = true;

    internal Vector2 Position = new(0, 0);


    public OrthographicCamera(float aspectRatio, float size)
    {
        _size = size;
        UpdateProjectionMatrix();
    }

    internal Matrix4 ProjectionMatrix { get; private set; }
    internal Color4 ClearColor { get; } = new(56, 61, 58, 255);

    internal Matrix4 ViewMatrix
    {
        get
        {
            CalculateViewMatrix();
            return _viewMatrix;
        }
    }

    public Matrix4 InverseView { get; private set; }
    public Matrix4 InverseProjection { get; private set; }

    private void CalculateViewMatrix()
    {
        _viewMatrix = Matrix4.LookAt(new Vector3(Position.X, Position.Y, 0),
            new Vector3(Position.X, Position.Y, 0) + _front, _up);
        InverseView = Matrix4.Invert(_viewMatrix);
        UpdateProjectionMatrix();
    }

    public void UpdateProjectionMatrix()
    {
        ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, 1920 * _size, 0, 1080 * _size, -1f, 100f);
        InverseProjection = Matrix4.Invert(ProjectionMatrix);
    }

    public void SetAspectRatio(float aspectRatio)
    {
        UpdateProjectionMatrix();
    }

    public void SetSize(float size)
    {
        _size = size;
        UpdateProjectionMatrix();
    }
}