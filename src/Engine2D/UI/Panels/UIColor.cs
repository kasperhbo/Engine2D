using Engine2D.Utilities;
using OpenTK.Mathematics;

namespace Engine2D.UI;

public class UIColor
{
    public static uint GetFromV4(Vector4 color)
    {
        return GetFromV4(new System.Numerics.Vector4(color.X, color.Y, color.Z, color.W));
    }
    
    public static uint GetFromV4(System.Numerics.Vector4 color)
    {
        byte[] btbc = MathUtils.NormalizedVector4ToBytes(color);
        return BitConverter.ToUInt32(btbc);
    }
}