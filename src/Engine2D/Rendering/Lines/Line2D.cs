using System.Numerics;
using Engine2D.GameObjects;

namespace Engine2D.Rendering.Lines;

public class Line2D
{
    public Vector2 From { get; private set; }
    public Vector2 To {get; private set;}

    public KDBColor Color { get; private set; }= new KDBColor();

    private int _lifeTime;
    
    public Line2D(Vector2 from, Vector2 to) {
        From = from;
        To = to;
        Color = new KDBColor(255, 0, 0, 255);
    }
    public Line2D(Vector2 from, Vector2 to, KDBColor color, int lifetime) {
        From = from;
        To = to;
        Color = color;
        _lifeTime = lifetime;
        Color = new KDBColor(255, 0, 0, 255);
    }
    
    
    

    public int OnRender()
    {
        _lifeTime--;
        return _lifeTime;
    }
    
    public float GetLengthSqrt()
    {
        Vector2 lt = To - From;
        return lt.LengthSquared();
    }

}