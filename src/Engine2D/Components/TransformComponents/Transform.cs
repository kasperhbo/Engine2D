#region

using System.Numerics;
using Engine2D.GameObjects;
using Engine2D.Utilities;
using Newtonsoft.Json;
using OpenTK.Windowing.Common;

#endregion

namespace Engine2D.Components.TransformComponents;

public class Transform : Component
{
    [JsonProperty]public Vector2 Position;
    [JsonIgnore]  public Vector3 EulerDegrees;
    [JsonIgnore]  public Vector3 EulerRadians;
    [JsonIgnore]  public Vector2 DraggingPos = new();
    [JsonProperty]public Quaternion Rotation;
    
    [JsonProperty]private Vector2 _size;

    [JsonIgnore]
    public Vector2 Size
    {
        get
        {
            if (Parent == null) return _size;
            var spr = Parent.GetComponent<SpriteRenderer>();
            if (spr == null) return _size;
            if (spr.Sprite == null) return _size;
            else
            {
                Vector2 size = new(_size.X * spr.Sprite.Height, _size.Y * spr.Sprite.Height);
                return size;
            }
        }
        set
        {
            _size = value;
        }
    }

    internal Transform()
    {
        Position = new Vector2();
        _size = new Vector2(1, 1);
        Rotation = new Quaternion();
    }

    [JsonConstructor]
    internal Transform(Vector2 position, Vector2 size, Quaternion rotation)
    {
        Position = position;
        _size = size;
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
        to._size = from._size;
        to.SetRotation(from.Rotation);
    }

    internal bool Equals(Transform other)
    {
        return Position != other.Position ||
               _size != other._size ||
               Rotation != other.Rotation;
    }

    internal Matrix4x4 GetTranslation()
    {
        var result = Matrix4x4.Identity;
        result *= Matrix4x4.CreateScale(new Vector3(_size.X, _size.Y, 1));
        result *= Matrix4x4.CreateFromQuaternion(Rotation);
        result *= Matrix4x4.CreateTranslation(Position.X, Position.Y, 0);
        return result;
    }

    internal Matrix4x4 GetTranslation(float width, float height)
    {
        var result = Matrix4x4.Identity;
        result *= Matrix4x4.CreateScale(new Vector3(_size.X * width, _size.Y * height, 1));
        result *= Matrix4x4.CreateFromQuaternion(Rotation);
        result *= Matrix4x4.CreateTranslation(Position.X, Position.Y, 0);
        return result;
    }

    internal override float GetFieldSize()
    {
        return 120;
    }

    public override Transform Clone()
    {
        Transform transform = new Transform();
        Copy(transform, this);
        return transform;
    }
}