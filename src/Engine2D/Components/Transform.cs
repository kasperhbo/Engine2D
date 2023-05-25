using System.Numerics;
using Engine2D.UI;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector2 = System.Numerics.Vector2;


namespace Engine2D.Components;

public class Transform : Component
{
    public Vector2 position;
    
    public Quaternion quaternion;
    public Vector3 eulerAngles;
    public Vector3 degrees;
    
    public Vector2 size = new(1, 1);

    public void SetRotation(Quaternion rotation)
    {
        quaternion = rotation;
        eulerAngles = quaternion.ToEulerAngles();
        degrees.X = (float)MathUtils.ConvertRadiansToDegrees((double)eulerAngles.X);
        degrees.Y = (float)MathUtils.ConvertRadiansToDegrees((double)eulerAngles.Y);
        degrees.Z = (float)MathUtils.ConvertRadiansToDegrees((double)eulerAngles.Z);
    }

    public void SetRotationByEuler()
    {
        Quaternion q = Quaternion.FromEulerAngles(eulerAngles);
        
        SetRotation(q);
    }
    
    public void SetRotationByDegrees()
    {
        eulerAngles.X = (float)MathUtils.ConvertDegreesToRadians((double)degrees.X);
        eulerAngles.Y = (float)MathUtils.ConvertDegreesToRadians((double)degrees.Y);
        eulerAngles.Z = (float)MathUtils.ConvertDegreesToRadians((double)degrees.Z);

        Quaternion q = Quaternion.FromEulerAngles(eulerAngles);
        
        SetRotation(q);
    }
    
       public void SetRotationByDegrees(Vector3 degrees)
       {
           SetRotationByDegrees();
       }
    
    public Matrix4 GetTranslation()
    {
        var t = Matrix4.Identity;
        t =  Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(
            eulerAngles
            ));
        t *= Matrix4.CreateScale(new OpenTK.Mathematics.Vector3(size.X, size.Y, 1));
        t *= Matrix4.CreateTranslation(position.X, position.Y, 0);
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
        to.eulerAngles = eulerAngles;
    }

    public override bool Equals(object o)
    {
        if (o == null) return false;
        if (!(o is Transform)) return false;

        var t = (Transform)o;

        return t.position.Equals(position) && t.size.Equals(size) && t.eulerAngles.Equals(eulerAngles);
    }

    public void SetPosition(OpenTK.Mathematics.Vector2 position)
    {
        this.position = new Vector2(position.X, position.Y);
    }

    public override string GetItemType()
    {
        return "Transform";
    }

    public void ImGui()
    {
        OpenTKUIHelper.DrawProperty("Position", ref position);
        
        ImGuiNET.ImGui.Separator();
        ImGuiNET.ImGui.Text("Rotation");
        if (OpenTKUIHelper.DrawProperty("Euler Angles", ref eulerAngles))
        {
            SetRotationByEuler();
        }

        if (OpenTKUIHelper.DrawProperty("Quaternion Angles", ref quaternion))
        {
            SetRotation(quaternion);
        }

        if (OpenTKUIHelper.DrawProperty("Radians Angles", ref degrees))
        {
            SetRotationByDegrees();
        }
    }
}