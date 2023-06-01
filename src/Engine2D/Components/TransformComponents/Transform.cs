using System.Numerics;
using Engine2D.UI;
using ImGuiNET;

namespace Engine2D.Components.TransformComponents;

public class Transform : Component
{
    public Vector2 Position;
    public Vector2 Size = new(1, 1);

    public RotationTransform Rotation = new();
    
    public Transform() 
    {
    }

    public override void ImGuiFields()
    {
        
        OpenTKUIHelper.DrawProperty("Position", ref Position);
        OpenTKUIHelper.DrawProperty("Size", ref Size);

        Rotation.ImGuiFields();
    }

    public void Copy(Transform to)
    {
        to.Position = this.Position;
        to.Size = this.Size;
        Rotation.Copy(to.Rotation);
    }

    public bool Equals(Transform other)
    {
        return (this.Position == other.Position &&
                this.Size == other.Size &&
                this.Rotation.Equals(other.Rotation));
    }

    public Matrix4x4 GetTranslation()
    {
        Matrix4x4 result = Matrix4x4.Identity;
        result *= Matrix4x4.CreateScale(new Vector3(Size.X, Size.Y, 1));
        result *= Matrix4x4.CreateFromQuaternion(Rotation.Quaternion);
        result *= Matrix4x4.CreateTranslation(Position.X, Position.Y, 0);
        return result;
    }
    
    public Matrix4x4 GetTranslation(float width, float height)
    {
        Matrix4x4 result = Matrix4x4.Identity;
        result *= Matrix4x4.CreateScale(new Vector3(Size.X+width, Size.Y+height, 1));
        result *= Matrix4x4.CreateFromQuaternion(Rotation.Quaternion);
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