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

            Vector4 dest = new((float)xN, (float)yN, (float)zN, w);

            return dest;
        }

        public static void Test(Vector4 V4, Matrix4 mat,out Vector4 dest)
        {
            float x = V4.X;
            float y = V4.Y;
            float z = V4.Z;
            float w = V4.W;

            dest.X = (float)Math.FusedMultiplyAdd(mat.Column0.X, x, Math.FusedMultiplyAdd(mat.Column1.X, y, Math.FusedMultiplyAdd(mat.Column2.X, z, mat.Column3.X * w)));

            dest.Y = (float)Math.FusedMultiplyAdd(mat.Column0.Y, x, Math.FusedMultiplyAdd(mat.Column1.Y, y, Math.FusedMultiplyAdd(mat.Column2.Y, z, mat.Column3.Y * w)));
            dest.Z = (float)Math.FusedMultiplyAdd(mat.Column0.Z, x, Math.FusedMultiplyAdd(mat.Column1.Z, y, Math.FusedMultiplyAdd(mat.Column2.Z, z, mat.Column3.Z * w)));
            dest.W = w;
        }
    }
}
