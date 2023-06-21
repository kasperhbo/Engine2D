#region

using System.Numerics;
using System.Runtime.CompilerServices;
using OpenTK.Mathematics;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

#endregion

namespace Engine2D.Utilities;

internal static class MathUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
    {
        if (val.CompareTo(min) < 0) return min;
        if (val.CompareTo(max) > 0) return max;
        return val;
    }


    internal static byte[] NormalizedVector4ToBytes(Vector4 v)
    {
        return new[]
        {
            (byte)(v.X * byte.MaxValue),
            (byte)(v.Y * byte.MaxValue),
            (byte)(v.Z * byte.MaxValue),
            (byte)(v.W * byte.MaxValue)
        };
    }

    internal static Matrix4 NumericsToTK(Matrix4x4 original)
    {
        var opentk = new Matrix4();

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

    internal static Vector4 MakeVec(float _x, float _y, float _z = 0f, float _w = 0f)
    {
        Vector4 res;
        res.X = _x;
        res.Y = _y;
        res.Z = _z;
        res.W = _w;
        return res;
    }

    #region Matrices

    internal static OpenTK.Mathematics.Vector4 Multiply(Matrix4 matrix4, OpenTK.Mathematics.Vector4 vector4)
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

        OpenTK.Mathematics.Vector4 dest = new((float)xN, (float)yN, (float)zN, w);

        return dest;
    }

    internal static Vector4 Multiply(Matrix4x4 matrix4, Vector4 vector4)
    {
        var temp = new OpenTK.Mathematics.Vector4(vector4.X, vector4.Y, vector4.Z, vector4.W);
        var resultOpenTK = Multiply(matrix4, temp);
        Vector4 result = new(resultOpenTK.X, resultOpenTK.Y, resultOpenTK.Z, resultOpenTK.W);
        return result;
    }

    internal static OpenTK.Mathematics.Vector4 Multiply(Matrix4x4 matrix4, OpenTK.Mathematics.Vector4 vector4)
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

        OpenTK.Mathematics.Vector4 dest = new((float)xN, (float)yN, (float)zN, w);

        return dest;
    }

    internal static OpenTK.Mathematics.Vector4 FromMatToVec4(Matrix4 matrix4)
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

        OpenTK.Mathematics.Vector4 dest = new((float)xN, (float)yN, (float)zN, w);

        return dest;
    }

    #endregion

    #region Rotations

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

    internal static Vector3 GetFront(Quaternion quaternion)
    {
        var front = new Vector3(0.0f, 0.0f, 1f);
        return Multiply(quaternion, front);
    }

    internal static Vector3 GetUp(Quaternion quaternion)
    {
        var upVector = new Vector3(0.0f, 1f, 0.0f);
        return Multiply(quaternion, upVector);
    }

    internal static Vector3 GetRight(Quaternion quaternion)
    {
        var rightVector = new Vector3(1f, 0.0f, 0.0f);
        return Multiply(quaternion, rightVector);
    }

    #endregion
}