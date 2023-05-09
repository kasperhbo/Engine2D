using OpenTK.Mathematics;

namespace Engine2D
{
    internal static class MathHelper
    {
        public static Vector4 Multiply(Matrix4 matrix4, Vector4 vector4)
        {
            double xN;
            double yN;
            double zN;

            float x = vector4.X;
            float y = vector4.Y;
            float z = vector4.Z;
            float w = vector4.W;

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

            Vector4 dest = new((float)xN, -(float)yN, (float)zN, w);

            return dest;
        }
    }
}
