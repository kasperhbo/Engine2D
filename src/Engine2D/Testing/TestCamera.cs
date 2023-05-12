using Engine2D.UI;
using ImGuiNET;
using KDBEngine.Core;
using OpenTK.Mathematics;

namespace Engine2D.Testing;

public class TestCamera
{
    // The inverse projection matrix
    private Matrix4 inverseProjectionMatrix;

    // The inverse view matrix
    private Matrix4 inverseViewMatrix;

    private Vector2 position;


    // Projection matrix say how big the screen is going to be.
    private Matrix4 projectionMatrix;
    private readonly Vector2 projectionSize = new(Engine.Get().getWidth(), Engine.Get().getHeight());

    // View Matrix says where the camera is in relation to our world.
    private Matrix4 viewMatrix;
    public float zoom = 1.0f;


    public TestCamera()
    {
        init(new Vector2());
    }

    public TestCamera(Vector2 position)
    {
        init(position);
    }

    private void init(Vector2 position)
    {
        this.position = position;
        projectionMatrix = new Matrix4();
        inverseProjectionMatrix = new Matrix4();
        viewMatrix = new Matrix4();
        inverseViewMatrix = new Matrix4();
        adjustProjection();
    }

    public void adjustProjection()
    {
        projectionMatrix = Matrix4.Identity;

        // Somehow this defines how many tiles are visible on the screen (40 * 21).
        //projectionMatrix.ortho(0.0f, 32.0f * 40.0f, 0, 32.0f * 21.0f, 0, 100);
        projectionMatrix = Matrix4.CreateOrthographicOffCenter(
            -(projectionSize.X * zoom / 2),
            projectionSize.X * zoom / 2,
            -(projectionSize.Y * zoom / 2),
            projectionSize.Y * zoom / 2,
            0,
            100
        );
        //projectionMatrix = this.createProjectionMatrix();
        // Save the inverted matrix
        inverseProjectionMatrix = Matrix4.Invert(projectionMatrix);
    }

    public Matrix4 getProjectionMatrix()
    {
        return projectionMatrix;
    }

    public Matrix4 getViewMatrix()
    {
        var cameraFront = new Vector3(0.0f, 0.0f, -1.0f);
        var cameraUp = new Vector3(0.0f, 1.0f, 0.0f);

        viewMatrix = Matrix4.Identity;
        viewMatrix = Matrix4.LookAt(
            new Vector3(position.X, position.Y, 20.0f), // Is the 20.0f something i want to scroll to zoom in and out?
            cameraFront + (position.X, position.Y, 0.0f),
            cameraUp
        );
        inverseViewMatrix = Matrix4.Invert(viewMatrix);
        return viewMatrix;
    }

    public Matrix4 getInverseProjection()
    {
        return inverseProjectionMatrix;
    }

    public Matrix4 getInverseViewMatrix()
    {
        return inverseViewMatrix;
    }

    public Vector2 getProjectionSize()
    {
        return projectionSize;
    }

    public Vector2 getPosition()
    {
        return position;
    }

    public float getZoom()
    {
        return zoom;
    }


    public void setZoom(float zoom)
    {
        this.zoom = zoom;
        adjustProjection();
    }

    public void addZoom(float value)
    {
        zoom += value;
    }

    public void CameraSettingsGUI()
    {
        ImGui.Begin("Camera Settings");
        OpenTKUIHelper.DrawComponentWindow("camera_transform", "Camera Transform", () =>
        {
            var tempPos =
                new System.Numerics.Vector2(position.X, position.Y);
            OpenTKUIHelper.DrawProperty("Position: ", ref tempPos);
            position = new Vector2(tempPos.X, tempPos.Y);

            if (OpenTKUIHelper.DrawProperty("Zoom", ref zoom)) adjustProjection();
        });
        ImGui.End();
    }
}