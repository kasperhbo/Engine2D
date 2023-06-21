#region

using System.Numerics;
using Engine2D.GameObjects;

#endregion

namespace Engine2D.Rendering.Lines;

internal class Line2D
{
    private int _lifeTime;

    internal Line2D(Vector2 from, Vector2 to)
    {
        From = from;
        To = to;
        Color = new KDBColor(255, 0, 0, 255);
    }

    internal Line2D(Vector2 from, Vector2 to, KDBColor color, int lifetime)
    {
        From = from;
        To = to;
        Color = color;
        _lifeTime = lifetime;
        Color = new KDBColor(255, 0, 0, 255);
    }

    internal Vector2 From { get; }
    internal Vector2 To { get; }

    internal KDBColor Color { get; private set; } = new();


    internal int OnRender()
    {
        _lifeTime--;
        return _lifeTime;
    }

    internal float GetLengthSqrt()
    {
        var lt = To - From;
        return lt.LengthSquared();
    }
}