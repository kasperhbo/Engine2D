using Engine2D.Logging;
using Newtonsoft.Json;

namespace Engine2D.Utilities;

internal class KDBColor
{
    [JsonRequired] internal float a;
    [JsonRequired] internal float b;
    [JsonRequired] internal float g;
    [JsonRequired] internal float r;

    internal KDBColor()
    {
        r = 255;
        g = 255;
        b = 255;
        a = 255;
    }

    internal KDBColor(KDBColor other)
    {
        r = other.r;
        g = other.g;
        b = other.b;
        a = other.a;
    }

    internal KDBColor(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    [JsonIgnore] internal float RNormalized => r / 255;
    [JsonIgnore] internal float GNormalized => g / 255;
    [JsonIgnore] internal float BNormalized => b / 255;
    [JsonIgnore] internal float ANormalized => a / 255;


    internal bool Equals(KDBColor? obj)
    {
        if (obj == null)
        {
            Log.Error("Obj to check against not set  'KDB COLOR' ");
            return false;
        }

        return r == obj.r && g == obj.g && b == obj.b && a == obj.a;
    }

    internal static void Copy(KDBColor from, KDBColor to)
    {
        from.r = to.r;
        from.g = to.g;
        from.b = to.b;
        from.a = to.a;
    }
}