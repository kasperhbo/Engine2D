#region

using System.Numerics;
using Engine2D.Logging;
using Engine2D.UI.ImGuiExtension;
using Engine2D.Utilities;
using ImGuiNET;
using Newtonsoft.Json;

#endregion

namespace Engine2D.Components.TransformComponents;

internal class Transform : Component
{
    [JsonIgnore] internal Vector3 EulerDegrees;

    [JsonIgnore] internal Vector3 EulerRadians;
    
    [JsonProperty]internal Vector2 Position;
    [JsonProperty]internal Quaternion Rotation;
    [JsonProperty]internal Vector2 Size;


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

    public override void ImGuiFields()
    {
        OpenTkuiHelper.DrawProperty("Position", ref Position);
        OpenTkuiHelper.DrawProperty("Size", ref Size);
        ImGui.Separator();
        var Q = Rotation;
        if (OpenTkuiHelper.DrawProperty("Q Rotation", ref Q)) SetRotation(Q);

        if (OpenTkuiHelper.DrawProperty("Euler Radians", ref EulerRadians))
            Log.Warning("Euler changing not implemented");

        if (OpenTkuiHelper.DrawProperty("Euler Degrees", ref EulerDegrees))
            Log.Warning("Degrees changing not implemented");
    }

    internal static void Copy(Transform to, Transform from)
    {
        to.Position = from.Position;
        to.Size = from.Size;
        to.SetRotation(from.Rotation);
    }

    internal bool Equals(Transform other)
    {
        return Position == other.Position &&
               Size == other.Size &&
               Rotation == other.Rotation;
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

    public override string GetItemType()
    {
        return this.GetType().FullName;
    }
}