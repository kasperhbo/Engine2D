using System.Numerics;
using System.Text.Json.Serialization;
using Engine2D.Components;
using Engine2D.Flags;
using Engine2D.GameObjects;
using Engine2D.UI;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Engine2D.Components.TransformComponents;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Engine2D.Testing;

public class TestCamera : Component
{
    public Vector2 ProjectionSize => projectionSize;
    public double Zoom  => zoom;
    
    private Vector2 projectionSize = new();
    // The inverse projection matrix
    private Matrix4 projectionMatrix;
    private Matrix4 inverseProjectionMatrix;
    
    private Matrix4 viewMatrix;
    private Matrix4 inverseViewMatrix;
    
    // private Vector2 Position;
    [JsonIgnore][ShowUI (show = false)]public Transform Transform = new();
    public KDBColor ClearColor = new KDBColor();
    private float zoom = 1.0f;


    public TestCamera(Vector2 projectionSize){
        Transform.Position = new System.Numerics.Vector2(0, 0);
        this.projectionSize = new(projectionSize.X, projectionSize.Y);
        init();
    }

    public TestCamera(System.Numerics.Vector2 Position, Vector2 projectionSize)
    {
        Transform.Position = Position;
        this.projectionSize = projectionSize;
        init();
    }

    private void init()
    {
        init(Transform.Position, projectionSize);
    }

    private void init(System.Numerics.Vector2 Position, Vector2 projectionSize)
    {
        Console.WriteLine(projectionSize);
        this.projectionSize = projectionSize;
        this.Transform.Position = Position;
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
            new Vector3(Transform.Position.X, Transform.Position.Y, 20.0f), // Is the 20.0f something i want to scroll to zoom in and out?
            cameraFront + (Transform.Position.X, Transform.Position.Y, 0.0f),
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
        return Transform.Position;
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

    public override void ImGuiFields()
    {
        if (OpenTKUIHelper.DrawProperty("Clear Color: ", ref ClearColor))
        {
            GL.ClearColor(ClearColor.R, ClearColor.G, ClearColor.B, ClearColor.A);
        };
        
        if(OpenTKUIHelper.DrawProperty("zoom", ref zoom))
        {
            adjustProjection();
        }
        
        if(OpenTKUIHelper.DrawProperty("Projection Size", ref projectionSize))
        {
            adjustProjection();
        }
    }

    public override float GetFieldSize()
    {
        return 120;
    }


    public void CameraSettingsGUI()
    {
        ImGui.Begin("Camera Settings");
        OpenTKUIHelper.DrawComponentWindow("camera_transform", "Camera Transform", () =>
        {
            var tempPos =
                new System.Numerics.Vector2(Transform.Position.X, Transform.Position.Y);
            
            OpenTKUIHelper.DrawProperty("Position: ", ref tempPos);
            
            Transform.Position = new System.Numerics.Vector2(tempPos.X, tempPos.Y);

            float zTemp = zoom;

            if (OpenTKUIHelper.DrawProperty("Zoom", ref zTemp))
            {
                setZoom(zTemp);
            };
            
            
        });
        ImGui.End();
    }
    
    #region Numerics Overloads  
    
    public Matrix4x4 GetProjectionMatrixNumerics()
    {
        Matrix4x4 projection = Matrix4x4.CreateOrthographicOffCenter(
            -(projectionSize.X * zoom)/2,
            (projectionSize.X * zoom)/2,
            -(projectionSize.Y * zoom)/2,
            (projectionSize.Y * zoom)/2,
            0,
            100
        );
        return projection;
    }
    
    public Matrix4x4 GetViewMatrixNumerics()
    {
        var cameraFront = new System.Numerics.Vector3(0.0f, 0.0f, -1.0f);
        var cameraUp = new System.Numerics.Vector3(0.0f, 1.0f, 0.0f);

        Matrix4x4 view = Matrix4x4.Identity;
        view = Matrix4x4.CreateLookAt(new System.Numerics.Vector3(Transform.Position.X, Transform.Position.Y, 20.0f), // Is the 20.0f something i want to scroll to zoom in and out?
            cameraFront + new System.Numerics.Vector3(Transform.Position.X, Transform.Position.Y, 0.0f),
            cameraUp
        );
        
        return view;
    }
    
    #endregion


    public override string GetItemType()
    {
        return "Camera";
    }
}