using System.Numerics;
using Engine2D.Components;
using Engine2D.Components.TransformComponents;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.UI;
using ImGuiNET;
using Newtonsoft.Json;
using Transform = Engine2D.Components.TransformComponents.Transform;

namespace Engine2D.Cameras;

public class Camera : Component
{
    public CameraTypes CameraType = CameraTypes.ORTHO;
    
    public float Size = 1f;
    public float Near = 0.1f;
    public float Far = 1000f;
   
    private Vector2 _projectionSize = new(1920,1080);
    public Vector2 ProjectionSize => _projectionSize;
    
    public KDBColor ClearColor { get; set; } = new();
    
    public Camera()
    {
        
    }

    [JsonConstructor]
    public Camera(CameraTypes cameraType, float size, KDBColor clearColor)
    {
        this.CameraType = cameraType;
        this.Size = size;
        this.ClearColor = clearColor;
    }
    
    public Matrix4x4 GetViewMatrix()
    {
        Transform transform = Parent.GetComponent<Transform>();
        
        if(transform == null)
        {
            Log.Error(Parent.Name + " Has no transform component!");
            return Matrix4x4.Identity;
        }
        
        Vector2 pos = transform.Position;

        Vector3 frontTemp = MathUtilsNumerics.GetFrontAxis(transform.Rotation);
        Vector3 upTemp = MathUtilsNumerics.GetUpAxis(transform.Rotation);

        return Matrix4x4.CreateLookAt(new Vector3(
                pos.X, pos.Y, 20.0f),
            frontTemp + new Vector3(
                pos.X, pos.Y, 0.0f),
            upTemp
        );
    }
    
    public Matrix4x4 GetProjectionMatrix()
    {
        Matrix4x4 projectionMatrix = Matrix4x4.Identity;
        
        int zoom = 1;
        if(CameraType == CameraTypes.ORTHO)
        {
             projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(
                    0.0f, ProjectionSize.X * zoom,
                 0.0f, ProjectionSize.Y * zoom, 0.0f, 100.0f
            );
        }
        
        if (CameraType == CameraTypes.PERSPECTIVE)
        {
            throw new NotImplementedException();
        }
        

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



    public override float GetFieldSize()
    {
        return 100;
    }

    public override void ImGuiFields()
    {
        if (OpenTKUIHelper.DrawProperty("Size: ", ref Size, label: false))
        {
        }
        
        ImGui.Separator();
        ImGui.Text("Clipping Planes");

        if (OpenTKUIHelper.DrawProperty("Near: ", ref Near, label: false) ||
            OpenTKUIHelper.DrawProperty("Far: ", ref Far, label: false))
        {
        }   
    }

    public Matrix4x4 getInverseView()
    {
        Matrix4x4.Invert(GetViewMatrix(), out var matrix4X4);
        return matrix4X4;
    }
    
    public Matrix4x4 getInverseProjection()
    {
        Matrix4x4.Invert(GetProjectionMatrix(), out var matrix4X4);
        return matrix4X4;
    }
}

public enum CameraTypes
{
    ORTHO,
    PERSPECTIVE
}