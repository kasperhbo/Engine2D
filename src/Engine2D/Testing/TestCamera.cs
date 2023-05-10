using KDBEngine.Core;
using OpenTK.Mathematics;
using System.Runtime.Intrinsics;

namespace Engine2D.Testing
{
    public class TestCamera
    {
        private Vector2 projectionSize = new Vector2(Engine.Get().getWidth(), Engine.Get().getHeight());


        // Projection matrix say how big the screen is going to be.
        private Matrix4 projectionMatrix = new Matrix4();
        // The inverse projection matrix
        private Matrix4 inverseProjectionMatrix = new Matrix4();

        // View Matrix says where the camera is in relation to our world.
        private Matrix4 viewMatrix = new Matrix4();
        // The inverse view matrix
        private Matrix4 inverseViewMatrix = new Matrix4();

        private Vector2 position;
        private float zoom = 1.0f;

        public TestCamera()
        {
            this.init(new Vector2());
        }

        public TestCamera(Vector2 position)
        {
            this.init(position);
        }

        private void init(Vector2 position)
        {
            this.position = position;
            this.projectionMatrix = new Matrix4();
            this.inverseProjectionMatrix = new Matrix4();
            this.viewMatrix = new Matrix4();
            this.inverseViewMatrix = new Matrix4();
            this.adjustProjection();
        }

        public void adjustProjection()
        {
            projectionMatrix = Matrix4.Identity;

            // Somehow this defines how many tiles are visible on the screen (40 * 21).
            //projectionMatrix.ortho(0.0f, 32.0f * 40.0f, 0, 32.0f * 21.0f, 0, 100);
            projectionMatrix = Matrix4.CreateOrthographicOffCenter(
                    0.0f,
                    projectionSize.X * zoom,
                    0,
                    projectionSize.Y * zoom,
                    0,
                    100
            );
            //projectionMatrix = this.createProjectionMatrix();
            // Save the inverted matrix
            this.inverseProjectionMatrix = Matrix4.Invert(projectionMatrix);
        }

        public Matrix4 getProjectionMatrix()
        {
            return this.projectionMatrix;
        }

        public Matrix4 getViewMatrix()
        {
            Vector3 cameraFront = new Vector3(0.0f, 0.0f, -1.0f);
            Vector3 cameraUp = new Vector3(0.0f, 1.0f, 0.0f);

            this.viewMatrix = Matrix4.Identity;
            this.viewMatrix = Matrix4.LookAt(
            new Vector3(position.X, position.Y, 20.0f), // Is the 20.0f something i want to scroll to zoom in and out?
                    cameraFront + (position.X, position.Y, 0.0f),
                    cameraUp
            );
            inverseViewMatrix = Matrix4.Invert(viewMatrix);
            return this.viewMatrix;
        }

        public Matrix4 getInverseProjection()
        {
            return inverseProjectionMatrix;
        }

        public Matrix4 getInverseViewMatrix()
        {
            return inverseViewMatrix;
        }

        public Vector2 getProjectionSize()
        {
            return this.projectionSize;
        }

        public Vector2 getPosition()
        {
            return this.position;
        }

        public float getZoom()
        {
            return zoom;
        }

        public void setZoom(float zoom)
        {
            this.zoom = zoom;
            this.adjustProjection();
        }

        public void addZoom(float value)
        {
            this.zoom += value;
        }

    }
}
