#region

using System.Numerics;
using Engine2D.Components.ENTT;
using Newtonsoft.Json;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

#endregion

namespace Engine2D.Components.Cameras;

//TODO: make this a component struct
public class Camera
{
    [JsonProperty] internal ENTTTransformComponent TransformComponent = new();
    
    [JsonProperty] private CameraTypes CameraType = CameraTypes.Ortho;
    
    [JsonProperty] private float _far = 1000f;
    [JsonProperty] private float _near = 0.1f;
    
    // private Vector2 _projectionSize = new(1920,1080);
    [JsonProperty] private Vector2 _projectionSize = new(1920, 1080);
    [JsonProperty] private Vector4 _clearColor = new(100, 149, 237, 255);
    [JsonProperty] private float _size = 0.001f;
    [JsonProperty] private bool _isMainCamera = false;
    [JsonProperty] private bool _isEditorCamera = false;

    [JsonIgnore] private Matrix4x4 _projectionMatrix = Matrix4x4.Identity;
    [JsonIgnore] private Matrix4x4 _viewMatrix = Matrix4x4.Identity;
    [JsonIgnore] private Matrix4x4 _viewProjectionMatrix = Matrix4x4.Identity;
    
    public float FadeRange = 1;
    
    internal Camera()
    {
    }

    internal Matrix4x4 GetViewMatrix()
    {
        var position = TransformComponent.Position;

        var cameraFront = new Vector3(0.0f, 0.0f, -1.0f);
        var cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
        var viewMatrix = Matrix4x4.Identity;
        
        viewMatrix = Matrix4x4.CreateLookAt(new Vector3(position.X, position.Y, 20.0f),
            cameraFront + new Vector3(position.X, position.Y, 0.0f),
            cameraUp);

        return viewMatrix;
    }

    internal Matrix4x4 GetProjectionMatrix()
    {
        //return _projectionMatrix;
        
        var projectionMatrix = Matrix4x4.Identity;

        if (CameraType == CameraTypes.Ortho)
            projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(
                -(_projectionSize.X) * (_size) / 2, (_projectionSize.X * (_size) / 2),
                -(_projectionSize.Y) * (_size) / 2, (_projectionSize.Y * (_size) / 2), 0.0f, 100.0f
            );
        if (CameraType == CameraTypes.Perspective) throw new NotImplementedException();

        return projectionMatrix;
    }
}


internal enum CameraTypes
{
    Ortho,
    Perspective
}