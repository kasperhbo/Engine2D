using System.Numerics;
using Box2DSharp.Common;
using Engine2D.UI;
using ImGuiNET;
using ImTool;
using OpenTK.Graphics.ES20;

namespace Engine2D.Components.TransformComponents;

public class RotationTransform
{
    private Vector3 _front = -Vector3.UnitZ;
    private Vector3 _up = Vector3.UnitY;
    private Vector3 _right = Vector3.UnitX;

    public Quaternion Quaternion;//{get;  set;}
    public EulerDegrees EulerDegrees;//{get;  set;}
    public EulerRadians EulerRadians;//{get;  set;}
    
    public RotationTransform()
    {
        Quaternion = System.Numerics.Quaternion.Identity;
        EulerDegrees = new(0,0,0);
        EulerRadians = new(0,0,0);
    }
    
    public void SetRotation(System.Numerics.Quaternion quaternion)
    {
        Quaternion = quaternion;

        EulerRadians = new(quaternion);
        EulerDegrees = new(EulerRadians);
    }
    
    public void SetRotation(EulerRadians radians)
    {
        EulerRadians = radians;
        EulerDegrees = EulerRadians.ToDegrees();
        Quaternion = EulerRadians.ToQuaternion();
    }
    
    public virtual void SetRotation(EulerDegrees degrees)
    {
        EulerDegrees = degrees;

        EulerRadians = EulerDegrees.ToRadians();
        Quaternion = EulerRadians.ToQuaternion();                                               
    }

    public void Copy(RotationTransform to)
    {
        to.SetRotation(this.Quaternion);
    }

    public bool Equals(RotationTransform other)
    {
        if (
            other.EulerRadians.Pitch != this.EulerRadians.Pitch ||
            other.EulerRadians.Roll != this.EulerRadians.Roll ||
            other.EulerRadians.Yaw != this.EulerRadians.Yaw
            )
        {
            return false;
        }

        return true;
    }


    private static ROTATION_OPTIONS _currentSelectedRotationOption = ROTATION_OPTIONS.DEGREES;
    
    private static ROTATION_OPTIONS[] _rotationOptionArray = new[]
    {
        ROTATION_OPTIONS.QUATERNION,
        ROTATION_OPTIONS.EULER,
        ROTATION_OPTIONS.DEGREES
    };
    
    public void ImGuiFields()
    {
        ImGui.Separator();
        ImGui.Text("Rotation");

        OpenTKUIHelper.PrepareProperty("Rotation Type");
        if(ImGui.BeginCombo("##rotationoptions", 
               _currentSelectedRotationOption.ToString()))
        {
            for (int n = 0; n < _rotationOptionArray.Length; n++)
            {
                bool is_selected = (_currentSelectedRotationOption == _rotationOptionArray[n]);
                if (ImGui.Selectable(_rotationOptionArray[n].ToString(), is_selected))
                    _currentSelectedRotationOption = _rotationOptionArray[n];
                if (is_selected)
                    ImGui.SetItemDefaultFocus();   // Set the initial focus when opening the combo (scrolling + for keyboard navigation support in the upcoming navigation branch)
            }
            ImGui.EndCombo();
        }
        
        if(_currentSelectedRotationOption == ROTATION_OPTIONS.QUATERNION)
        {
            if (OpenTKUIHelper.DrawProperty("Quaternion", ref Quaternion))
            {
                SetRotation(Quaternion);
            }
        }
        
        if(_currentSelectedRotationOption == ROTATION_OPTIONS.EULER)
        {
            if (OpenTKUIHelper.DrawProperty("Radians", ref EulerRadians))
            {
                SetRotation(EulerRadians);
            }
        }

        if(_currentSelectedRotationOption == ROTATION_OPTIONS.DEGREES)
        {
            if (OpenTKUIHelper.DrawProperty("Degrees", ref EulerDegrees))
            {
                SetRotation(EulerDegrees);
            }
        }
    }

    public void SetRotationInvertPitch(EulerDegrees rotationEulerDegrees)
    {
        rotationEulerDegrees.Pitch = -rotationEulerDegrees.Yaw;
        SetRotation(rotationEulerDegrees);
    }
}

public enum ROTATION_OPTIONS
{
    QUATERNION = 0,
    EULER = 1,
    DEGREES = 2
}

public abstract class RotationBase
{
    public float Pitch;
    public float Yaw;
    public float Roll;

    public RotationBase(float roll, float pitch, float yaw)
    {
        Pitch = pitch;
        Yaw = yaw;
        Roll = roll;
    }

    public RotationBase()
    {
        Pitch = 0;
        Yaw = 0;
        Roll = 0;
    }
}

public class EulerRadians : RotationBase
{
    public EulerRadians(System.Numerics.Quaternion quaternion)
    {
        EulerRadians res = FromQuaternion(quaternion);
        this.Pitch = res.Pitch;
        this.Roll = res.Roll;
        this.Yaw = res.Yaw;
    }

    public EulerRadians(float roll, float pitch, float yaw) : base(roll, pitch, yaw)
    {
    }
    
    public EulerDegrees ToDegrees()
    {
        float x = (float)(Roll / Math.PI * 180);
        float y = (float)(Pitch / Math.PI * 180);
        float z = (float)(Yaw / Math.PI * 180);
        return new(x, y, z);
    }
    
    public static EulerRadians FromQuaternion(System.Numerics.Quaternion q)
    {
        EulerRadians angles = new(0,0,0);

        // roll / x
        double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
        double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
        angles.Roll = (float)Math.Atan2(sinr_cosp, cosr_cosp);

        // pitch / y
        double sinp = 2 * (q.W * q.Y - q.Z * q.X);
        if (Math.Abs(sinp) >= 1)
        {
            angles.Pitch = (float)Math.CopySign(Math.PI / 2, sinp);
        }
        else
        {
            angles.Pitch = (float)Math.Asin(sinp);
        }

        // yaw / z
        double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
        double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
        angles.Yaw = (float)Math.Atan2(siny_cosp, cosy_cosp);

        return angles;
    }
    
    public System.Numerics.Quaternion ToQuaternion()
    {
        var v = this;
        float cy = (float)Math.Cos(v.Yaw * 0.5);
        float sy = (float)Math.Sin(v.Yaw * 0.5);
        float cp = (float)Math.Cos(v.Pitch * 0.5);
        float sp = (float)Math.Sin(v.Pitch * 0.5);
        float cr = (float)Math.Cos(v.Roll * 0.5);
        float sr = (float)Math.Sin(v.Roll * 0.5);

        return new System.Numerics.Quaternion
        {
            W = (cr * cp * cy + sr * sp * sy),
            X = (sr * cp * cy - cr * sp * sy),
            Y = (cr * sp * cy + sr * cp * sy),
            Z = (cr * cp * sy - sr * sp * cy)
        };
    }
}

public class EulerDegrees : RotationBase
{
    public EulerDegrees(float roll, float pitch, float yaw) : base(roll, pitch, yaw)
    {
    }
    
    public EulerDegrees(EulerRadians degree)
    {
        Roll = (float)(degree.Roll * Math.PI / 180);
        Pitch = (float)(degree.Pitch * Math.PI / 180);
        Yaw = (float)(degree.Yaw * Math.PI / 180);
    }

    public EulerDegrees(Quaternion outQuaternion)
    {
        EulerRadians rad = new EulerRadians(outQuaternion);
        Roll = rad.ToDegrees().Roll;
        Yaw = rad.ToDegrees().Yaw;
        Pitch = rad.ToDegrees().Pitch;
    }

    public EulerRadians ToRadians()
    {
        float x = (float)(this.Roll * Math.PI / 180);
        float y = (float)(this.Pitch * Math.PI / 180);
        float z = (float)(this.Yaw * Math.PI / 180);
        return new(x, y, z);
    }
    
    public EulerDegrees FromRadians()
    {
        float x = (float)(this.Roll / Math.PI * 180);
        float y = (float)(this.Pitch / Math.PI * 180);
        float z = (float)(this.Yaw / Math.PI * 180);
        return new(x, y, z);
    }
}