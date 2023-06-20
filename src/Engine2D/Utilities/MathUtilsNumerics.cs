using OpenTK.Mathematics;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace Engine2D.Utilities;

public static class MathUtilsNumerics
{
    #region Rotation
    
    public static Vector3 QuaternionToRadians(Quaternion quaternion)
    {
        float num1 = quaternion.W * quaternion.W;
        float num2 = quaternion.X * quaternion.X;
        float num3 = quaternion.Y * quaternion.Y;
        float num4 = quaternion.Z * quaternion.Z;
        float num5 = num2 + num3 + num4 + num1;
        float num6 = (float) (quaternion.X * (double) quaternion.Z + quaternion.W * (double) quaternion.Y);
        Vector3 eulerAngles;
        if (num6 > 0.4999994933605194 * num5)
        {
            eulerAngles.Z = 2f * MathF.Atan2(quaternion.X, quaternion.W);
            eulerAngles.Y = 1.5707964f;
            eulerAngles.X = 0.0f;
        }
        else if (num6 < -0.4999994933605194 * num5)
        {
            eulerAngles.Z = -2f * MathF.Atan2(quaternion.X, quaternion.W);
            eulerAngles.Y = -1.5707964f;
            eulerAngles.X = 0.0f;
        }
        else
        {
            eulerAngles.Z = MathF.Atan2((float) (2.0 * (quaternion.W * (double) quaternion.Z - quaternion.X * (double) quaternion.Y)), num1 + num2 - num3 - num4);
            eulerAngles.Y = MathF.Asin(2f * num6 / num5);
            eulerAngles.X = MathF.Atan2((float) (2.0 * (quaternion.W * (double) quaternion.X - quaternion.Y * (double) quaternion.Z)), num1 - num2 - num3 + num4);
        }
        return eulerAngles;
    }

    public static Quaternion RadiansToQuaternion(float pitch, float yaw, float roll)
    {
        float qx = MathF.Sin(roll/2) * MathF.Cos(pitch/2) * MathF.Cos(yaw/2) - MathF.Cos(roll/2) * MathF.Sin(pitch/2) * MathF.Sin(yaw/2);
        float qy = MathF.Cos(roll/2) * MathF.Sin(pitch/2) * MathF.Cos(yaw/2) + MathF.Sin(roll/2) * MathF.Cos(pitch/2) * MathF.Sin(yaw/2);
        float qz = MathF.Cos(roll/2) * MathF.Cos(pitch/2) * MathF.Sin(yaw/2) - MathF.Sin(roll/2) * MathF.Sin(pitch/2) * MathF.Cos(yaw/2);
        float qw = MathF.Cos(roll/2) * MathF.Cos(pitch/2) * MathF.Cos(yaw/2) + MathF.Sin(roll/2) * MathF.Sin(pitch/2) * MathF.Sin(yaw/2);
        Quaternion q = new Quaternion();
        
        q.X = qx;
        q.Y = qy;
        q.Z = qz;
        q.W = qw;

        return q;
    }
    
    public static Vector3 RadiansToDegrees(Vector3 eulerRadians)
    {
        float pitch = eulerRadians.X;
        float yaw = eulerRadians.Y;
        float roll = eulerRadians.Z;
        
        // ° × π/180
        float p = MathHelper.RadiansToDegrees(pitch);
        float y = MathHelper.RadiansToDegrees(yaw);
        float r = MathHelper.RadiansToDegrees(roll);
        
        return new(p, y, r);
    }

    public static Vector3 DegreesToRadians(Vector3 eulerDegrees)
    {
        float pitch = eulerDegrees.X;
        float yaw = eulerDegrees.Y;
        float roll = eulerDegrees.Z;
        
        // ° × π/180
        float p = MathHelper.DegreesToRadians(pitch);
        float y = MathHelper.DegreesToRadians(yaw);
        float r = MathHelper.DegreesToRadians(roll);
        
        return new(p, y, r);
    }

    public static Vector3 GetFrontAxis(Quaternion quaternion)
    {
        Vector3 front = new Vector3(0.0f, 0.0f, 1f);
        return Multiply(quaternion, front);
    }
    
    public static Vector3 GetUpAxis(Quaternion quaternion)
    {
        Vector3 upVector = new Vector3(0.0f, 1f, 0.0f);
        return Multiply(quaternion, upVector);
    }
    
    public static Vector3 GetRightAxis(Quaternion quaternion)
    {
        Vector3 rightVector = new Vector3(1f, 0.0f, 0.0f);
        return Multiply(quaternion, rightVector);
    }

    public static Vector3 Multiply(Quaternion rotation, Vector3 point)
    {
        float num1 = rotation.X * 2f;
        float num2 = rotation.Y * 2f;
        float num3 = rotation.Z * 2f;
        float num4 = rotation.X * num1;
        float num5 = rotation.Y * num2;
        float num6 = rotation.Z * num3;
        float num7 = rotation.X * num2;
        float num8 = rotation.X * num3;
        float num9 = rotation.Y * num3;
        float num10 = rotation.W * num1;
        float num11 = rotation.W * num2;
        float num12 = rotation.W * num3;
        Vector3 vector3;
        
        vector3.X = (float)((1.0 - (num5 + (double)num6)) * point.X +
                            (num7 - (double)num12) * point.Y +
                            (num8 + (double)num11) * point.Z);
        vector3.Y = (float)((num7 + (double)num12) * point.X +
                            (1.0 - (num4 + (double)num6)) * point.Y +
                            (num9 - (double)num10) * point.Z);
        vector3.Z = (float)((num8 - (double)num11) * point.X +
                            (num9 + (double)num10) * point.Y +
                            (1.0 - (num4 + (double)num5)) * point.Z);
        return vector3;
    }

    
    #endregion
}