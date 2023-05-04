using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Engine2D.Rendering
{
    internal class GameRenderer {

        public int DrawCalls { get; internal set; } = 0;

        private Vector3 camerapos;

        // IMPORTANT: Must be in counter-clockwise order
        private int[] elementArray = {
            /*
                    x        x
                    x        x
             */
            2, 1, 0, // Top right triangle
            0, 1, 3 // bottom left triangle
        };

        private float[] vertexArray = {
            // position               // color                    // UV Coordinates
            100f,   0f, 0.0f,       1.0f, 0.0f, 0.0f, 1.0f,     1, 1, // Bottom right 0
            0f, 100f, 0.0f,         0.0f, 1.0f, 0.0f, 1.0f,     0, 0, // Top left     1
            100f, 100f, 0.0f ,      1.0f, 0.0f, 1.0f, 1.0f,     1, 0, // Top right    2
            0f,   0f, 0.0f,         1.0f, 1.0f, 0.0f, 1.0f,     0, 1  // Bottom left  3
        };

        private int vertexID, fragmentID, shaderProgram;

        private int vaoID, vboID, eboID;

        private Shader defaultShader;
        private Texture testTexture;
        private Camera camera; 

        internal void Init(Vector2i clientSize)
        {
            camerapos = new(-300, -200, 0);
            camera = new Camera(camerapos);
            //camera = new Camera(new Vector2(0,0));

            // ============================================================
            // Generate VAO, VBO, and EBO buffer objects, and send to GPU
            // ============================================================
            vaoID = GL.GenVertexArray();
            GL.BindVertexArray(vaoID);

            vboID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexArray.Length * sizeof(float), vertexArray,
                BufferUsageHint.StaticDraw);

            eboID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, elementArray.Length * sizeof(int), elementArray,
                BufferUsageHint.StaticDraw);

            defaultShader = new Shader("shaders/shader_files/default.vert", "shaders/shader_files/default.frag");
            defaultShader.compile();
            defaultShader.use();

            testTexture = new Texture("images/testImage.png", false);

            var positionSize = 3;
            var colorSize = 4;
            var uvSize = 2;

            var vertexSizeBytes = (positionSize + colorSize + uvSize) * sizeof(float);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);


            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3 * sizeof(float));


            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 9 * sizeof(float), 7 * sizeof(float));

            GL.ActiveTexture(TextureUnit.Texture0);
            testTexture.bind();

            defaultShader.uploadInt("TEX_SAMPLER", 0);
        }

        internal void OnClose()
        {
            
        }

        internal void OnResize(Vector2i newSize)
        {
        }

        internal void Render()
        {
            DrawCalls++;

            GL.ClearColor(1f, .3f, .3f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Enable(EnableCap.Blend);

            GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);

            GL.BindVertexArray(vaoID);

            GL.ActiveTexture(TextureUnit.Texture0);
            testTexture.bind();

            defaultShader.use();
            defaultShader.uploadInt("TEX_SAMPLER", 0);

            defaultShader.uploadMat4f("uProjection", camera.GetProjectionMatrix());
            defaultShader.uploadMat4f("uView", camera.getViewMatrix());
            defaultShader.uploadFloat("uTime", 1);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.DrawElements(PrimitiveType.Triangles, elementArray.Length, DrawElementsType.UnsignedInt, 0);

            // Unbind everything
            GL.DisableVertexAttribArray(0);
            // GL.DisableVertexAttribArray(1);

            GL.BindVertexArray(0);

            defaultShader.detach();
            
        }

        internal void Update(double dt)
        {
        }
    }
}
