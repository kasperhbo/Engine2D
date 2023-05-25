using OpenTK.Mathematics;
using Vector2 = System.Numerics.Vector2;

namespace Engine2D.Components;

public class Transform : Component
{
    public Vector2 position;
    public float rotation;
    public Vector2 size = new(1, 1);

    public Matrix4 GetTranslation()
    {
        var t = Matrix4.Identity;
        t = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation));
        t *= Matrix4.CreateScale(new OpenTK.Mathematics.Vector3(size.X, size.Y, 1));
        t *= Matrix4.CreateTranslation(position.X, position.Y, 0);
        return t;
    }
    
    public Matrix4 GetTranslation(Vector2 positionManipulations)
    {
        var t = Matrix4.Identity;
        t = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation));
        t *= Matrix4.CreateScale(new OpenTK.Mathematics.Vector3(size.X, size.Y, 1));
        t *= Matrix4.CreateTranslation(position.X + positionManipulations.X, position.Y + positionManipulations.Y, 0);
        return t;
    }

    /// <summary>
    ///     Custom copy method, if you dont do this the project will make an reference to the other transform object instead of
    ///     copying the properties
    /// </summary>
    /// <param name="to">The transform where it needs to copy to</param>
    public void Copy(Transform to)
    {
        to.position = position;
        to.size = size;
        to.rotation = rotation;
    }

    public override bool Equals(object o)
    {
        if (o == null) return false;
        if (!(o is Transform)) return false;

        var t = (Transform)o;

        return t.position.Equals(position) && t.size.Equals(size) && t.rotation.Equals(rotation);
    }

    public void SetPosition(OpenTK.Mathematics.Vector2 position)
    {
        this.position = new Vector2(position.X, position.Y);
    }

    public override string GetItemType()
    {
        return "Transform";
    }
}