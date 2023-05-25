using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using OpenTK.Mathematics;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace Engine2D;

public static class MathUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
    {
        if (val.CompareTo(min) < 0) return min;
        if (val.CompareTo(max) > 0) return max;
        return val;
    }
    
    public static Vector3 V3DegreToRadian(in this Vector3 from)
    {
        return new Vector3(MathHelper.DegreesToRadians(from.X),
            MathHelper.DegreesToRadians(from.Y),
            MathHelper.DegreesToRadians(from.Z));
    }

    public static Vector4 Multiply(Matrix4x4 matrix4, Vector4 vector4)
    {
        double xN;
        double yN;
        double zN;

        var x = vector4.X;
        var y = vector4.Y;
        var z = vector4.Z;
        var w = vector4.W;

        xN = Math.FusedMultiplyAdd(matrix4.M11, x,
            Math.FusedMultiplyAdd(matrix4.M21, y,
                Math.FusedMultiplyAdd(matrix4.M31, z,
                    matrix4.M41 * w)));

        yN = Math.FusedMultiplyAdd(matrix4.M12, x,
            Math.FusedMultiplyAdd(matrix4.M22, y,
                Math.FusedMultiplyAdd(matrix4.M32, z,
                    matrix4.M42 * w)));

        zN = Math.FusedMultiplyAdd(matrix4.M13, x,
            Math.FusedMultiplyAdd(matrix4.M23, y,
                Math.FusedMultiplyAdd(matrix4.M33, z,
                    matrix4.M43 * w)));


        Vector4 dest = new((float)xN, (float)yN, (float)zN, w);
        return dest;
    }

    public static Vector4 Multiply(Matrix4 matrix4, Vector4 vector4)
    {
        double xN;
        double yN;
        double zN;

        var x = vector4.X;
        var y = vector4.Y;
        var z = vector4.Z;
        var w = vector4.W;


        xN = Math.FusedMultiplyAdd(matrix4.M11, x,
            Math.FusedMultiplyAdd(matrix4.M21, y,
                Math.FusedMultiplyAdd(matrix4.M31, z,
                    matrix4.M41 * w)));

        yN = Math.FusedMultiplyAdd(matrix4.M12, x,
            Math.FusedMultiplyAdd(matrix4.M22, y,
                Math.FusedMultiplyAdd(matrix4.M32, z,
                    matrix4.M42 * w)));

        zN = Math.FusedMultiplyAdd(matrix4.M13, x,
            Math.FusedMultiplyAdd(matrix4.M23, y,
                Math.FusedMultiplyAdd(matrix4.M33, z,
                    matrix4.M43 * w)));

        Vector4 dest = new((float)xN, (float)yN, (float)zN, w);

        return dest;
    }
    
    public static Vector4 FromMatToVec4(Matrix4 matrix4)
    {
        double xN;
        double yN;
        double zN;

        float x = 1;
        float y = 1;
        float z = 1;
        float w = 1;

        xN = Math.FusedMultiplyAdd(matrix4.M11, x,
            Math.FusedMultiplyAdd(matrix4.M21, y,
                Math.FusedMultiplyAdd(matrix4.M31, z,
                    matrix4.M41 * w)));

        yN = Math.FusedMultiplyAdd(matrix4.M12, x,
            Math.FusedMultiplyAdd(matrix4.M22, y,
                Math.FusedMultiplyAdd(matrix4.M32, z,
                    matrix4.M42 * w)));

        zN = Math.FusedMultiplyAdd(matrix4.M13, x,
            Math.FusedMultiplyAdd(matrix4.M23, y,
                Math.FusedMultiplyAdd(matrix4.M33, z,
                    matrix4.M43 * w)));

        Vector4 dest = new((float)xN, (float)yN, (float)zN, w);

        return dest;
    }

    public static Vector3 QuaternionToAngleAxis(Quaternion q)
    {
        float Θ = (float)Math.Acos(q.W)*2;
        
        float ax = (float)(q.X / Math.Sin(Math.Acos(Θ)));
        float ay = (float)(q.Y / Math.Sin(Math.Acos(Θ)));
        float az = (float)(q.Z / Math.Sin(Math.Acos(Θ)));

        return new Vector3(ax, ay, az);
    } 
    
    public static double ConvertRadiansToDegrees(double radians)
    {
        double degrees = (180f / Math.PI) * radians;
        return (degrees);
    }
    
    public static double ConvertDegreesToRadians (double degrees)
    {
        double radians = (Math.PI / 180) * degrees;
        return (radians);
    }
}