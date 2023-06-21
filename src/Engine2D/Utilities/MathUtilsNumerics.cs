#region

using OpenTK.Mathematics;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

#endregion

namespace Engine2D.Utilities;

internal static class MathUtilsNumerics
{
    #region Rotation

    internal static Vector3 QuaternionToRadians(Quaternion quaternion)
    {
        var num1 = quaternion.W * quaternion.W;
        var num2 = quaternion.X * quaternion.X;
        var num3 = quaternion.Y * quaternion.Y;
        var num4 = quaternion.Z * quaternion.Z;
        var num5 = num2 + num3 + num4 + num1;
        var num6 = (float)(quaternion.X * (double)quaternion.Z + quaternion.W * (double)quaternion.Y);
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
            eulerAngles.Z =
                MathF.Atan2((float)(2.0 * (quaternion.W * (double)quaternion.Z - quaternion.X * (double)quaternion.Y)),
                    num1 + num2 - num3 - num4);
            eulerAngles.Y = MathF.Asin(2f * num6 / num5);
            eulerAngles.X =
                MathF.Atan2((float)(2.0 * (quaternion.W * (double)quaternion.X - quaternion.Y * (double)quaternion.Z)),
                    num1 - num2 - num3 + num4);
        }

        return eulerAngles;
    }

    internal static Quaternion RadiansToQuaternion(float pitch, float yaw, float roll)
    {
        var qx = MathF.Sin(roll / 2) * MathF.Cos(pitch / 2) * MathF.Cos(yaw / 2) -
                 MathF.Cos(roll / 2) * MathF.Sin(pitch / 2) * MathF.Sin(yaw / 2);
        var qy = MathF.Cos(roll / 2) * MathF.Sin(pitch / 2) * MathF.Cos(yaw / 2) +
                 MathF.Sin(roll / 2) * MathF.Cos(pitch / 2) * MathF.Sin(yaw / 2);
        var qz = MathF.Cos(roll / 2) * MathF.Cos(pitch / 2) * MathF.Sin(yaw / 2) -
                 MathF.Sin(roll / 2) * MathF.Sin(pitch / 2) * MathF.Cos(yaw / 2);
        var qw = MathF.Cos(roll / 2) * MathF.Cos(pitch / 2) * MathF.Cos(yaw / 2) +
                 MathF.Sin(roll / 2) * MathF.Sin(pitch / 2) * MathF.Sin(yaw / 2);
        var q = new Quaternion();

        q.X = qx;
        q.Y = qy;
        q.Z = qz;
        q.W = qw;

        return q;
    }

    internal static Vector3 RadiansToDegrees(Vector3 eulerRadians)
    {
        var pitch = eulerRadians.X;
        var yaw = eulerRadians.Y;
        var roll = eulerRadians.Z;

        // ° × π/180
        var p = MathHelper.RadiansToDegrees(pitch);
        var y = MathHelper.RadiansToDegrees(yaw);
        var r = MathHelper.RadiansToDegrees(roll);

        return new Vector3(p, y, r);
    }

    internal static Vector3 DegreesToRadians(Vector3 eulerDegrees)
    {
        var pitch = eulerDegrees.X;
        var yaw = eulerDegrees.Y;
        var roll = eulerDegrees.Z;

        // ° × π/180
        var p = MathHelper.DegreesToRadians(pitch);
        var y = MathHelper.DegreesToRadians(yaw);
        var r = MathHelper.DegreesToRadians(roll);

        return new Vector3(p, y, r);
    }

    internal static Vector3 GetFrontAxis(Quaternion quaternion)
    {
        var front = new Vector3(0.0f, 0.0f, 1f);
        return Multiply(quaternion, front);
    }

    internal static Vector3 GetUpAxis(Quaternion quaternion)
    {
        var upVector = new Vector3(0.0f, 1f, 0.0f);
        return Multiply(quaternion, upVector);
    }

    internal static Vector3 GetRightAxis(Quaternion quaternion)
    {
        var rightVector = new Vector3(1f, 0.0f, 0.0f);
        return Multiply(quaternion, rightVector);
    }

    internal static Vector3 Multiply(Quaternion rotation, Vector3 point)
    {
        var num1 = rotation.X * 2f;
        var num2 = rotation.Y * 2f;
        var num3 = rotation.Z * 2f;
        var num4 = rotation.X * num1;
        var num5 = rotation.Y * num2;
        var num6 = rotation.Z * num3;
        var num7 = rotation.X * num2;
        var num8 = rotation.X * num3;
        var num9 = rotation.Y * num3;
        var num10 = rotation.W * num1;
        var num11 = rotation.W * num2;
        var num12 = rotation.W * num3;
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