using Engine2D.Rendering;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.GameObjects
{
    internal class SpriteRenderer
    {
        public Vector2 Position = new();
        public Vector2 Size     = new(10,10);
        public Vector3 Color    = new(255,255,255);

        public float rot = 0;

        private Shader _shader;
        private uint _quadVAO;

        internal SpriteRenderer(Shader shader)
        {
        }
    }
}
