
using System.Numerics;

namespace Engine2D.Components
{
    internal class Transform
    {
        public Vector2 position = new();
        public Vector2 size = new(32,32);
        public float rotation = new();
        

        public Transform() { }
    }
}
