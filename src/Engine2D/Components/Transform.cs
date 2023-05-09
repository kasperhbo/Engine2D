
using System.Numerics;

namespace Engine2D.Components
{
    internal class Transform
    {
        public Vector2 position = new();
        public Vector2 size = new(32,32);
        public float rotation = new();
        

        public Transform() { }

        /// <summary>
        /// Custom copy method, if you dont do this the project will make an reference to the other transform object instead of copying the properties
        /// </summary>
        /// <param name="to">The transform where it needs to copy to</param>
        public void Copy(Transform to)
        {
            to.position = position;
            to.size = size;
            to.rotation = rotation;
        }

        public override bool Equals(object o)
        {
            if (o == null) return false;
            if (!(o is Transform)) return false;

            var t = (Transform)o;

            return t.position.Equals(position)&& t.size.Equals(size) && t.rotation.Equals(rotation);
        }


    }
}
