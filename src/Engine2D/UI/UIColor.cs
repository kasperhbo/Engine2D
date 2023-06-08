using System.Numerics;
using Engine2D.Utilities;

namespace Engine2D.UI;

public class UIColor
{
    public static uint GetFromV4(OpenTK.Mathematics.Vector4 color)
    {
        return GetFromV4(new Vector4(color.X, color.Y, color.Z, color.W));
    }
    
    public static uint GetFromV4(Vector4 color)
    {
        byte[] btbc = MathUtils.NormalizedVector4ToBytes(color);
        return BitConverter.ToUInt32(btbc);
    }
}