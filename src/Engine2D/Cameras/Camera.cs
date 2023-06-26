#region

using System.Numerics;
using System.Security.Claims;
using Engine2D.Components;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.UI.ImGuiExtension;
using Engine2D.Utilities;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Windowing.Common;

#endregion

namespace Engine2D.Cameras;

internal class Camera : Component
{
    [JsonProperty]internal CameraTypes CameraType = CameraTypes.ORTHO;
    [JsonProperty]internal float Far = 1000f;
    [JsonProperty]internal float Near = 0.1f;

    // private Vector2 _projectionSize = new(1920,1080);
    [JsonProperty]internal Vector2 ProjectionSize = new(1920, 1080);
    [JsonProperty]internal Vector4 ClearColor = new(100, 149, 237, 255);
    [JsonProperty]internal float Size = 1f;
    [JsonProperty]private bool _isMainCamera;

    public bool IsMainCamera
    {
        get => _isMainCamera;
        set
        {
            Engine.Get().CurrentScene.CurrentMainGameCamera = this;
            _isMainCamera = value;
        }
    }

    internal Camera()
    {
    }


    internal Matrix4x4 GetViewMatrix()
    {
        if (Parent == null) return Matrix4x4.Identity;
        
        var transform = Parent.GetComponent<Transform>();

        if (transform == null)
        {
            Log.Error(Parent.Name + " Has no transform component!");
            return Matrix4x4.Identity;
        }

        var pos = transform.Position;

        var frontTemp = MathUtilsNumerics.GetFrontAxis(transform.Rotation);
        var upTemp = MathUtilsNumerics.GetUpAxis(transform.Rotation);

        return Matrix4x4.CreateLookAt(new Vector3(
                pos.X, pos.Y, 20.0f),
            frontTemp + new Vector3(
                pos.X, pos.Y, 0.0f),
            upTemp
        );
    }

    internal Matrix4x4 GetProjectionMatrix()
    {
        var projectionMatrix = Matrix4x4.Identity;

        var zoom = 1;
        if (CameraType == CameraTypes.ORTHO)
            projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(
                -(ProjectionSize.X * Size / 2), ProjectionSize.X * Size / 2,
                -(ProjectionSize.Y * Size / 2), ProjectionSize.Y * Size / 2, 0.0f, 100.0f
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
        if (OpenTkuiHelper.DrawProperty("Size: ", ref Size, false))
        {
        }

        Gui.DrawProperty("Clear Color: ", ref ClearColor);
        if (Gui.DrawProperty("Is Main Camera", ref _isMainCamera))
        {
            if (_isMainCamera) Engine.Get().CurrentScene.CurrentMainGameCamera = this;
        }



        ImGui.Separator();
        ImGui.Text("Clipping Planes");
        
        if (OpenTkuiHelper.DrawProperty("Near: ", ref Near, false) ||
            OpenTkuiHelper.DrawProperty("Far: ", ref Far, false))
        {
        }
    }

    internal Matrix4x4 getInverseView()
    {
        Matrix4x4.Invert(GetViewMatrix(), out var matrix4X4);
        return matrix4X4;
    }

    internal Matrix4x4 getInverseProjection()
    {
        Matrix4x4.Invert(GetProjectionMatrix(), out var matrix4X4);
        return matrix4X4;
    }
}

internal enum CameraTypes
{
    ORTHO,
    PERSPECTIVE
}