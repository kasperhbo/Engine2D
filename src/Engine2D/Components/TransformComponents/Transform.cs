#region

using System.Numerics;
using Engine2D.Logging;
using Engine2D.UI.ImGuiExtension;
using Engine2D.Utilities;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Windowing.Common;

#endregion

namespace Engine2D.Components.TransformComponents;

public class Transform : Component
{
    [JsonProperty]public Vector2 Position;
    [JsonIgnore]  public Vector3 EulerDegrees;
    [JsonIgnore]  public Vector3 EulerRadians;
    [JsonProperty]public Quaternion Rotation;
    [JsonProperty]public Vector2 Size;


    internal Transform()
    {
        Position = new Vector2();
        Size = new Vector2(1, 1);
        Rotation = new Quaternion();
    }

    [JsonConstructor]
    internal Transform(Vector2 position, Vector2 size, Quaternion rotation)
    {
        Position = position;
        Size = size;
        SetRotation(rotation);
    }

    internal void SetRotation(Quaternion q)
    {
        Rotation = q;
        EulerRadians = MathUtilsNumerics.QuaternionToRadians(q);
        EulerDegrees = MathUtilsNumerics.RadiansToDegrees(EulerRadians);
    }

    public override void StartPlay()
    {
        
    }

    public override void Update(FrameEventArgs args)
    {
        
    }

    public override void ImGuiFields()
    {
        base.ImGuiFields();
        // Gui.DrawProperty("Position", ref Position);
        // Gui.DrawProperty("Rotation", ref EulerDegrees);
        // Gui.DrawProperty("Size", ref Size);
    }

    internal static void Copy(Transform to, Transform from)
    {
        to.Position = from.Position;
        to.Size = from.Size;
        to.SetRotation(from.Rotation);
    }

    internal bool Equals(Transform other)
    {
        return Position != other.Position ||
               Size != other.Size ||
               Rotation != other.Rotation;
    }

    internal Matrix4x4 GetTranslation()
    {
        var result = Matrix4x4.Identity;
        result *= Matrix4x4.CreateScale(new Vector3(Size.X, Size.Y, 1));
        result *= Matrix4x4.CreateFromQuaternion(Rotation);
        result *= Matrix4x4.CreateTranslation(Position.X, Position.Y, 0);
        return result;
    }

    internal Matrix4x4 GetTranslation(float width, float height)
    {
        var result = Matrix4x4.Identity;
        result *= Matrix4x4.CreateScale(new Vector3(Size.X * width, Size.Y * height, 1));
        result *= Matrix4x4.CreateFromQuaternion(Rotation);
        result *= Matrix4x4.CreateTranslation(Position.X, Position.Y, 0);
        return result;
    }

    internal override float GetFieldSize()
    {
        return 120;
    }
}