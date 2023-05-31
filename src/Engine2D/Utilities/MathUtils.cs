using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using Engine2D.Components;
using Engine2D.Components.TransformComponents;
using OpenTK.Graphics.ES30;
using OpenTK.Mathematics;
using Veldrid.ImageSharp;
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
        vector3.X = (float) ((1.0 - ((double) num5 + (double) num6)) * (double) point.X + ((double) num7 - (double) num12) * (double) point.Y + ((double) num8 + (double) num11) * (double) point.Z);
        vector3.Y = (float) (((double) num7 + (double) num12) * (double) point.X + (1.0 - ((double) num4 + (double) num6)) * (double) point.Y + ((double) num9 - (double) num10) * (double) point.Z);
        vector3.Z = (float) (((double) num8 - (double) num11) * (double) point.X + ((double) num9 + (double) num10) * (double) point.Y + (1.0 - ((double) num4 + (double) num5)) * (double) point.Z);
        return vector3;
    }


    public static void UpdateVectors(System.Numerics.Quaternion QRot, out System.Numerics.Vector3 _front, out System.Numerics.Vector3 _right, out System.Numerics.Vector3 _up)  
    {
        System.Numerics.Vector3 forwardVector = new System.Numerics.Vector3(0.0f, 0.0f, 1f);
        System.Numerics.Vector3 upVector = new System.Numerics.Vector3(0.0f, 1f, 0.0f);
        System.Numerics.Vector3 rightVector = new System.Numerics.Vector3(1f, 0.0f, 0.0f);
        _front = Multiply(QRot, forwardVector);
        _up = Multiply(QRot, upVector);
        _right = Multiply(QRot, rightVector);


    }


    #endregion
}

