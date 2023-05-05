using OpenTK.Mathematics;

namespace Engine2D.Rendering;

public class OrthographicCamera
{
    public Matrix4 ProjectionMatrix { get; private set; }
    public Matrix4 ViewMatrix { get;}

    public Color4 ClearColor { get;} = new(56, 61, 58, 255);
    
    private float _aspectRatio;
    private float _size;
    
    private float _near = -1f;
    private float _far  = 1f;


    public OrthographicCamera(float aspectRatio, float size)
    {
        this._aspectRatio = aspectRatio;
        this._size = size;
        
        ViewMatrix = Matrix4.Identity;
        
        UpdateProjectionMatrix();
    }

    public void UpdateProjectionMatrix()
    {
        ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(-_size * _aspectRatio, _size * _aspectRatio, -_size, _size, -1f, 1f);
    }

    public void SetAspectRatio(float aspectRatio)
    {
        this._aspectRatio = aspectRatio;
        UpdateProjectionMatrix();
    }

    public void SetSize(float size)
    {
        this._size = size;
        UpdateProjectionMatrix();
    }
}