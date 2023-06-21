#region

using Engine2D.Utilities;
using OpenTK.Mathematics;

#endregion

namespace Engine2D.UI;

internal class UIColor
{
    internal static uint GetFromV4(Vector4 color)
    {
        return GetFromV4(new System.Numerics.Vector4(color.X, color.Y, color.Z, color.W));
    }

    internal static uint GetFromV4(System.Numerics.Vector4 color)
    {
        var btbc = MathUtils.NormalizedVector4ToBytes(color);
        return BitConverter.ToUInt32(btbc);
    }
}