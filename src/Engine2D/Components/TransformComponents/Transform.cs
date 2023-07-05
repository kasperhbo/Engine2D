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
    [JsonProperty]public Vector2 LocalPosition;
    [JsonProperty]public Quaternion Rotation;
    [JsonProperty]public Vector2 Size;
    
    [JsonIgnore]  public Vector3 EulerDegrees;
    [JsonIgnore]  public Vector3 EulerRadians;
    [JsonIgnore]  public Vector2 DraggingPos = new();

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

    public override void Init()
    {
        
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

    internal Matrix4x4 GetTranslation(bool includeSprite = true)
    {
        var result = Matrix4x4.Identity;
        result *= Matrix4x4.CreateScale(new Vector3(GetFullSize(includeSprite).X, GetFullSize(includeSprite).Y, 1));
        result *= Matrix4x4.CreateFromQuaternion(Rotation);
        
        if(Parent?.ParentUid != -1)
            result *= Matrix4x4.CreateTranslation(Position.X + LocalPosition.X, Position.Y + LocalPosition.Y, 0);
        else
            result *= Matrix4x4.CreateTranslation(Position.X , Position.Y , 0);
        
        return result;
    }

    internal Vector2 GetFullSize(bool includeSprite = true)
    {
        if (Parent == null) return new Vector2(0, 0);
        
        var spr = Parent.GetComponent<SpriteRenderer>();
        var dividedSize = Size;
        if (spr != null && includeSprite)
        {
            if (spr.Sprite != null)
            {
                return new Vector2(dividedSize.X * spr.Sprite.Width, dividedSize.Y * spr.Sprite.Height);
            }
        }

        return new Vector2(dividedSize.X, dividedSize.Y);
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