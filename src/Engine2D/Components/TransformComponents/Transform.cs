using System.Numerics;
using Engine2D.Logging;
using Engine2D.UI;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using Quaternion = System.Numerics.Quaternion;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace Engine2D.Components.TransformComponents;

public class Transform : Component
{
    public Vector2 Position;
    public Vector2 Size;
    
    //Rotation
    public Quaternion Rotation;
    
    [JsonIgnore] public Vector3 EulerRadians;
    [JsonIgnore] public Vector3 EulerDegrees;


    public Transform()
    {
        Position = new();
        Size = new(1,1);
        Rotation = new Quaternion();
    }
    
    [JsonConstructor]
    public Transform(Vector2 position, Vector2 size, Quaternion rotation)
    {
        this.Position = position;
        this.Size = size;
        SetRotation(rotation);
    }

    public void SetRotation(Quaternion q)
    {
        Rotation = q;
        EulerRadians = MathUtilsNumerics.QuaternionToRadians(q);
        EulerDegrees = MathUtilsNumerics.RadiansToDegrees(EulerRadians);
    }

    public override void ImGuiFields()
    {               
        OpenTKUIHelper.DrawProperty("Position", ref Position);
        OpenTKUIHelper.DrawProperty("Size", ref Size);
        ImGui.Separator();
        var Q = Rotation;
        if (OpenTKUIHelper.DrawProperty("Q Rotation", ref Q))
        {
            SetRotation(Q);
        }
        
        if (OpenTKUIHelper.DrawProperty("Euler Radians", ref EulerRadians))
        {
            Log.Warning("Euler changing not implemented");
        }
        
        if (OpenTKUIHelper.DrawProperty("Euler Degrees", ref EulerDegrees))
        {
            Log.Warning("Degrees changing not implemented");
        }
    }

    public void Copy(Transform to)
    {
        to.Position = this.Position;
        to.Size = this.Size;
        SetRotation(this.Rotation);
    }

    public bool Equals(Transform other)
    {
        return (this.Position == other.Position &&
                this.Size == other.Size &&
                this.Rotation == other.Rotation
                );
    }

    public Matrix4x4 GetTranslation()
    {
        Matrix4x4 result = Matrix4x4.Identity;
        result *= Matrix4x4.CreateScale(new Vector3(Size.X, Size.Y, 1));
        result *= Matrix4x4.CreateFromQuaternion(Rotation);
        result *= Matrix4x4.CreateTranslation(Position.X, Position.Y, 0);
        return result;
    }
    
    public Matrix4x4 GetTranslation(float width, float height)
    {
        Matrix4x4 result = Matrix4x4.Identity;
        result *= Matrix4x4.CreateScale(new Vector3(Size.X*width, Size.Y*height, 1));
        result *= Matrix4x4.CreateFromQuaternion(Rotation);
        result *= Matrix4x4.CreateTranslation(Position.X, Position.Y, 0);
        return result;
    }

    public override float GetFieldSize()
    {
        return 120;
    }

    public override string GetItemType()
    {
        return "Transform";
    }

}