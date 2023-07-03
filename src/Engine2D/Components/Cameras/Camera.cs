#region

using System.Numerics;
using Engine2D.Components;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Windowing.Common;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

#endregion

namespace Engine2D.Cameras;

internal class Camera : Component
{
    [JsonProperty] internal CameraTypes CameraType = CameraTypes.ORTHO;
    [JsonProperty] internal float Far = 1000f;
    [JsonProperty] internal float Near = 0.1f;

    // private Vector2 _projectionSize = new(1920,1080);
    [JsonProperty] internal Vector2 ProjectionSize = new(1920, 1080);
    [JsonProperty] internal Vector4 ClearColor = new(100, 149, 237, 255);
    [JsonProperty] internal float Size = 1f;
    [JsonProperty] internal bool _isMainCamera = false;
    [JsonProperty] internal bool _isEditorCamera = false;

    public float FadeRange = 1;
    
    internal Camera()
    {
    }
    

    public override void Init()
    {
    }


    internal Matrix4x4 GetViewMatrix()
    {
        var transform = Parent.GetComponent<Transform>();

        if (transform == null)
        {
            Log.Error(Parent.Name + " Has no transform component!");
            return Matrix4x4.Identity;
        }

        var position = transform.Position;

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
        var projectionMatrix = Matrix4x4.Identity;

        var zoom = 1;
        if (CameraType == CameraTypes.ORTHO)
            projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(
                -(ProjectionSize.X) * Size / 2, (ProjectionSize.X * Size / 2),
                -(ProjectionSize.Y) * Size / 2, (ProjectionSize.Y * Size / 2), 0.0f, 100.0f
            );
        if (CameraType == CameraTypes.PERSPECTIVE) throw new NotImplementedException();

        return projectionMatrix;
    }

    public override void StartPlay()
    {

    }

    public override string GetItemType()
    {
        return this.GetType().FullName;
    }

    public override void Update(FrameEventArgs args)
    {

    }

    public override void Destroy()
    {
        base.Destroy();
    }

    public override void EditorUpdate(double dt)
    {
        base.EditorUpdate(dt);
    }


    internal override float GetFieldSize()
    {
        return 100;
    }

    public override void ImGuiFields()
    {
        Gui.DrawProperty("Projection Size", ref ProjectionSize);
        Gui.DrawProperty("Fade range", ref FadeRange);
        if (OpenTkuiHelper.DrawProperty("Size: ", ref Size, false))
        {
        }

        Gui.DrawProperty("Clear Color: ", ref ClearColor);
        if (Gui.DrawProperty("Is Main Camera", ref _isMainCamera))
        {
            
        }

        if (Gui.DrawProperty("Is Editor Camera", ref _isEditorCamera))
        {
            
        }


        ImGui.Separator();
        ImGui.Text("Clipping Planes");

        if (OpenTkuiHelper.DrawProperty("Near: ", ref Near, false) ||
            OpenTkuiHelper.DrawProperty("Far: ", ref Far, false))
        {
        }
    }

    public override Camera Clone()
    {
        var camera = new Camera();
        
        camera.ProjectionSize = ProjectionSize;
        camera.FadeRange = FadeRange;
        camera.Size = Size;
        camera.ClearColor = ClearColor;
        camera.Near = Near;
        camera.Far = Far;
        camera.CameraType = CameraType;
        camera._isMainCamera = _isMainCamera;
        camera._isEditorCamera = _isEditorCamera;
        
        return camera;
    }
}


internal enum CameraTypes
{
    ORTHO,
    PERSPECTIVE
}