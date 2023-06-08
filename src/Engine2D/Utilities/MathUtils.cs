using System.Numerics;
using System.Runtime.CompilerServices;
using OpenTK.Mathematics;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace Engine2D.Utilities;

public static class MathUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
    {
        if (val.CompareTo(min) < 0) return min;
        if (val.CompareTo(max) > 0) return max;
        return val;
    }

    #region Matrices

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

    public static System.Numerics.Vector4 Multiply(Matrix4x4 matrix4, System.Numerics.Vector4 vector4)
    {
        Vector4 temp = new Vector4(vector4.X, vector4.Y, vector4.Z, vector4.W);
        Vector4 resultOpenTK = Multiply(matrix4, temp);
        System.Numerics.Vector4 result = new(resultOpenTK.X, resultOpenTK.Y, resultOpenTK.Z, resultOpenTK.W);
        return result;
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

    #endregion

    #region Rotations

    public static System.Numerics.Vector3 Multiply(System.Numerics.Quaternion rotation, System.Numerics.Vector3 point)
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
        System.Numerics.Vector3 vector3;
        vector3.X = (float)((1.0 - ((double)num5 + (double)num6)) * (double)point.X +
                            ((double)num7 - (double)num12) * (double)point.Y +
                            ((double)num8 + (double)num11) * (double)point.Z);
        vector3.Y = (float)(((double)num7 + (double)num12) * (double)point.X +
                            (1.0 - ((double)num4 + (double)num6)) * (double)point.Y +
                            ((double)num9 - (double)num10) * (double)point.Z);
        vector3.Z = (float)(((double)num8 - (double)num11) * (double)point.X +
                            ((double)num9 + (double)num10) * (double)point.Y +
                            (1.0 - ((double)num4 + (double)num5)) * (double)point.Z);
        return vector3;
    }

    public static System.Numerics.Vector3 GetFront(System.Numerics.Quaternion quaternion)
    {
        System.Numerics.Vector3 front = new System.Numerics.Vector3(0.0f, 0.0f, 1f);
        return Multiply(quaternion, front);
    }

    public static System.Numerics.Vector3 GetUp(System.Numerics.Quaternion quaternion)
    {
        System.Numerics.Vector3 upVector = new System.Numerics.Vector3(0.0f, 1f, 0.0f);
        return Multiply(quaternion, upVector);
    }

    public static System.Numerics.Vector3 GetRight(System.Numerics.Quaternion quaternion)
    {
        System.Numerics.Vector3 rightVector = new System.Numerics.Vector3(1f, 0.0f, 0.0f);
        return Multiply(quaternion, rightVector);
    }
    
    #endregion


    public static byte[] NormalizedVector4ToBytes(System.Numerics.Vector4 v)
    {

        return new byte[]
        {
            (byte)(v.X * byte.MaxValue),
            (byte)(v.Y * byte.MaxValue),
            (byte)(v.Z * byte.MaxValue),
            (byte)(v.W * byte.MaxValue),
        };
    }
    
    public static Matrix4 NumericsToTK(Matrix4x4 original)
    {
        Matrix4 opentk = new Matrix4();
        
        opentk.M11 = original.M11;
        opentk.M12 = original.M12;
        opentk.M13 = original.M13;
        opentk.M14 = original.M14;
        opentk.M21 = original.M21;
        opentk.M22 = original.M22;
        opentk.M23 = original.M23;
        opentk.M24 = original.M24;
        opentk.M31 = original.M31;
        opentk.M32 = original.M32;
        opentk.M33 = original.M33;
        opentk.M34 = original.M34;
        opentk.M41 = original.M41;
        opentk.M42 = original.M42;
        opentk.M43 = original.M43;
        opentk.M44 = original.M44;

        return opentk;
    }

    public static System.Numerics.Vector4 MakeVec(float _x, float _y, float _z = 0f, float _w = 0f)
    {
        System.Numerics.Vector4 res;
        res.X = _x;
        res.Y = _y;
        res.Z = _z;
        res.W = _w;
        return res;
    }
}