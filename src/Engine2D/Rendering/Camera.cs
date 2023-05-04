using OpenTK.Mathematics;

namespace Engine2D.Rendering
{
    internal class Camera
    {

        private bool _firstMove = true;

        // The field of view of the camera (radians)
        private float _fov = MathHelper.PiOver2;

        // Those vectors are directions pointing outwards from the camera to define how it rotated.
        private readonly Vector3 _front = -Vector3.UnitZ;
        private readonly Vector3 _up = Vector3.UnitY;

        // Rotation around the Y axis (radians)
        private float _yaw = -MathHelper.PiOver2; // Without this, you would be started rotated 90 degrees right.

        public Vector3 position;

        //TODO: CHANGE SPAWN POSITION OF CAMERA WHEN CHANGING TO ORTHOGRAPHIC!!
        public Camera(Vector3 position)
        {
            this.position = position;
        }


        public Matrix4 getViewMatrix()
        {
            return Matrix4.LookAt(position, position + _front, _up);
        }


        private Vector2 projectionSize = new Vector2(6, 3);
        public Matrix4 GetProjectionMatrix()
        {
            // return Matrix4.CreatePerspectiveFieldOfView(_fov, 800f/600f, 0.01f, 100f);
            return Matrix4.CreateOrthographicOffCenter(0, 
                32f * 40f, 
                0, 
                32f * 21f, 0, 100f);
        }


        public Matrix4 GetInverseProjection()
        {
            Matrix4 inverse = new Matrix4();
            Matrix4.Invert(GetProjectionMatrix(), out inverse);

            return inverse;
        }

        public Matrix4 GetInverseView()
        {
            Matrix4 inverse = new Matrix4();
            Matrix4.Invert(getViewMatrix(), out inverse);
            return inverse;
        }

        #region test1 old

        //public Matrix4 projection, view, inverseProjection, inverseView;
        //public Vector2 position;
        //public static float projectionWidth = 6;
        //public static float projectionHeight = 3;

        //private Vector2 projectionSize = new Vector2(projectionWidth, projectionHeight);

        //private float zoom = 1;

        //internal Camera(Vector2 position)
        //{
        //    this.position = position;

        //    this.projection = new Matrix4();
        //    this.view = new Matrix4();
        //    this.inverseProjection = new Matrix4();
        //    this.inverseView = new Matrix4();

        //    AdjustProjection();
        //}

        //internal void AdjustProjection()
        //{
        //    projection = Matrix4.Identity;
        //    projection = Matrix4.CreateOrthographicOffCenter(0.0f, projectionSize.X * this.zoom,
        //            0.0f, projectionSize.Y * zoom, 0.0f, 100.0f);

        //    inverseProjection = Matrix4.Invert(projection);

        //    Console.WriteLine("inverse: " +  inverseProjection);
        //}

        //internal Matrix4 GetViewMatrix()
        //{
        //    Vector3 front = new(0, 0, -1);
        //    Vector3 up = new(0, 1, 0);

        //    view = Matrix4.Identity;
        //    view = Matrix4.LookAt(new Vector3(position.X, position.Y, 20),
        //        front + new Vector3(position.X, position.Y, 0),
        //        up);

        //    inverseView = Matrix4.Invert(view);

        //    return this.view;
        //}

        //public Matrix4 getProjectionMatrix()
        //{
        //    return this.projection;
        //}

        //public Matrix4 getInverseProjection()
        //{
        //    return this.inverseProjection;
        //}

        //public Matrix4 getInverseView()
        //{
        //    return this.inverseView;
        //}

        //public Vector2 getProjectionSize()
        //{
        //    return projectionSize;
        //}

        #endregion

    }
}
