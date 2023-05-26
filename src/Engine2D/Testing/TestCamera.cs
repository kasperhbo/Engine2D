using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Engine2D.Components;
using Engine2D.Flags;
using Engine2D.GameObjects;
using Engine2D.UI;
using ImGuiNET;
using ImTool;
using KDBEngine.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Engine2D.Testing;

public class TestCamera : Component
{
    public Vector2 projectionSize { get; private set; } = new(Engine.Get().ClientSize.X, Engine.Get().ClientSize.Y);

    // The inverse projection matrix
    private Matrix4 projectionMatrix;
    private Matrix4 inverseProjectionMatrix;
    
    private Matrix4 viewMatrix;
    private Matrix4 inverseViewMatrix;
    
    // private Vector2 position;
    [JsonIgnore][ShowUI (show = false)]public Transform Transform = new();

    public SpriteColor ClearColor = new SpriteColor(); 

    // Projection matrix say how big the screen is going to be.

    // View Matrix says where the camera is in relation to our world.
    public float zoom { get; private set; }= 1.0f;


    public TestCamera(Vector2 projectionSize)
    {
        init(new Vector2(), projectionSize);
    }

    public TestCamera(Vector2 position, Vector2 projectionSize)
    {
        init(position, projectionSize);
    }

    private void init(OpenTK.Mathematics.Vector2 position, Vector2 projectionSize)
    {
        init(new System.Numerics.Vector2(position.X, position.Y), projectionSize);
    }

    private void init(System.Numerics.Vector2 position, Vector2 projectionSize)
    {
        this.projectionSize = projectionSize;
        this.Transform.position = position;
        projectionMatrix = new Matrix4();
        inverseProjectionMatrix = new Matrix4();
        viewMatrix = new Matrix4();
        inverseViewMatrix = new Matrix4();
        adjustProjection(this.projectionSize);
    }

    public void adjustProjection()
    {
        adjustProjection(this.projectionSize);
    }

    public void adjustProjection(Vector2 projectionSize)
    {
        this.projectionSize = projectionSize;
        projectionMatrix = Matrix4.Identity;

        // Somehow this defines how many tiles are visible on the screen (40 * 21).
        //projectionMatrix.ortho(0.0f, 32.0f * 40.0f, 0, 32.0f * 21.0f, 0, 100);
        projectionMatrix = Matrix4.CreateOrthographicOffCenter(
            -(projectionSize.X * zoom)/2,
            (projectionSize.X * zoom)/2,
            -(projectionSize.Y * zoom)/2,
            (projectionSize.Y * zoom)/2,
            0,
            100
        );       
        // projectionMatrix = Matrix4.CreateOrthographicOffCenter(
        //     -(projectionSize.X * zoom)/2,
        //     (projectionSize.X * zoom)/2,
        //     -(projectionSize.Y * zoom)/2,
        //     (projectionSize.Y * zoom)/2,
        //     0,
        //     100
        // );
        
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
            new Vector3(Transform.position.X, Transform.position.Y, 20.0f), // Is the 20.0f something i want to scroll to zoom in and out?
            cameraFront + (Transform.position.X, Transform.position.Y, 0.0f),
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

    public System.Numerics.Vector2 getPosition()
    {
        if (Parent != null)
        {
            Transform = Parent.Transform;
        };
        return Transform.position;
    }
    
    
    public void setZoom(float zoom)
    {
        this.zoom = zoom;
        adjustProjection(this.projectionSize);
    }
    
    public void addZoom(float value)
    {
        setZoom(zoom + value);
    }

    public override int ImGuiFields()
    {
        int count = 0;
        

        if (OpenTKUIHelper.DrawProperty("Clear Color: ", ref ClearColor))
        {
            count++;
            GL.ClearColor(ClearColor.Color.X, ClearColor.Color.Y, ClearColor.Color.Z, ClearColor.Color.W);
        };

        return count;
    }

    public void CameraSettingsGUI()
    {
        ImGui.Begin("Camera Settings");
        OpenTKUIHelper.DrawComponentWindow("camera_transform", "Camera Transform", () =>
        {
            var tempPos =
                new System.Numerics.Vector2(Transform.position.X, Transform.position.Y);
            
            OpenTKUIHelper.DrawProperty("Position: ", ref tempPos);
            
            Transform.position = new System.Numerics.Vector2(tempPos.X, tempPos.Y);

            float zTemp = zoom;

            if (OpenTKUIHelper.DrawProperty("Zoom", ref zTemp))
            {
                setZoom(zTemp);
            };
            
            
        });
        ImGui.End();
    }

    public override string GetItemType()
    {
        return "Camera";
    }
}