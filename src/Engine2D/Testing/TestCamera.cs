    using System.Numerics;
    using System.Runtime.CompilerServices;
    using Engine2D.Components;
    using Engine2D.GameObjects;
    using Engine2D.Logging;
    using Engine2D.UI;
    using GlmSharp;
    using GlmSharp.Swizzle;
    using ImGuiNET;
    using OpenTK.Graphics.ES30;

    namespace Engine2D.Testing;

    public class TestCamera : Component
    {
        private float _size = 10f;
        private float _near = 0.3f;
        private float _far = 100f;
        private Vector2 _projectionSize = new();

        private Matrix4x4 _projectionMatrix = Matrix4x4.Identity;
        
        public Vector2 ProjectionSize => _projectionSize;
        public float Size  => _size;
        public KDBColor ClearColor { get; set; } = new();
        
        public TestCamera(Vector2 projectionSize)
        {
            _projectionSize = projectionSize;
        }
        
        public TestCamera(Vector2 projectionSize, float size)
        {
            _projectionSize = projectionSize;
            _size = size;
        }
        
        public TestCamera(Vector2 projectionSize, float size, float near, float far)
        {
            _projectionSize = projectionSize;
            _size = size;
            _near = near;
            _far = far;
        }

        public Matrix4x4 GetViewMatrix()
        {
            
            System.Numerics.Vector3 Position = new(Parent.Transform.Position.X,Parent.Transform.Position.Y,0);
            float x = (_projectionSize.X / 100);
            float y = (_projectionSize.Y * 2 / 100);
            System.Numerics.Vector2 Scale = new(x,y);
            Log.Succes(Scale.ToString());
            System.Numerics.Vector3 Rotation = new(0, 0, Parent.Transform.Rotation.EulerDegrees.Yaw);

            Matrix4x4 proj =
                Matrix4x4.CreateScale(Scale.X, -Scale.Y, 1) * Matrix4x4.CreateRotationZ(Rotation.Z)*
                    Matrix4x4.CreateTranslation(new Vector3(Position.X * _projectionSize.X,
                    Position.Y * _projectionSize.Y, -Position.Z))
                ;
            return proj;

            //
            // return Matrix4x4.CreateLookAt(new System.Numerics.Vector3(
            //         Parent.Transform.Position.X, Parent.Transform.Position.Y, 20.0f), // Is the 20.0f something i want to scroll to zoom in and out?
            //     Parent.Transform.Rotation.Front + new System.Numerics.Vector3(
            //         Parent.Transform.Position.X, Parent.Transform.Position.Y, 0.0f),
            //     Parent.Transform.Rotation.Up
            // );
        }

        public Matrix4x4 GetProjectionMatrix()
        {
            // System.Numerics.Vector3 Position = new(Parent.Transform.Position.X,Parent.Transform.Position.Y,0);
            // float x = (_projectionSize.X / 100);
            // float y = (_projectionSize.Y * 2 / 100);
            // System.Numerics.Vector2 Scale = new(x,y);
            // Log.Succes(Scale.ToString());
            // System.Numerics.Vector3 Rotation = new(0, 0, Parent.Transform.Rotation.EulerDegrees.Yaw);
            //
            // Matrix4x4 proj =
            //         Matrix4x4.CreateTranslation(new Vector3(-Position.X*_projectionSize.X, -Position.Y*_projectionSize.Y,-Position.Z))*
            //         Matrix4x4.CreateScale(Scale.X, -Scale.Y, 1) * Matrix4x4.CreateRotationZ(Rotation.Z)*
            //
                ;
            return Matrix4x4.CreateOrthographicOffCenter(
                (_projectionSize.X * Size)/2,
                -(_projectionSize.X * Size)/2,
                (_projectionSize.Y * Size)/2,
                -(_projectionSize.Y * Size)/2,
                _near, _far);
            // float aspect = (float)_projectionSize.X / (float)_projectionSize.Y;
            //
            // float width = _projectionSize.X;
            // float height = _projectionSize.Y;
            //
            // Log.Message(width.ToString());
            // Log.Warning(height.ToString());
            //
            // _projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(
            //     -width*0.5f,
            //     width*0.5f,
            //     -height*0.5f,
            //     height*0.5f,
            //     _near,
            //     _far
            // );

            // float width = _projectionSize.X;
            // float height = _projectionSize.Y;
            //
            // float aspectRatio = (float)width / (float)height;
            //
            // var m_OrthoSize = ((float)width /(float)height < 16f / 9f
            //         ? (float)width / (float)height
            //         : 16f / 9f) / (16f / 9f);
            //
            // float srcAspectRatio = 1f / 1f; //* Original aspect, default: 1.0;
            //
            // float orthoLeft = 0.0f;
            // float ortholeft = orthoLeft;
            //
            // float orthoright = 100.0f;
            //
            // float orthoUp = 100.0f;
            // float orthoDown = 0.0f;
            //
            // float left =((0.5f * (orthoright - orthoLeft) + ortholeft) * srcAspectRatio) -
            //             (0.5f * (orthoright - orthoLeft) * aspectRatio * m_OrthoSize);
            //
            // float right = ((0.5f * (orthoright - orthoLeft) + ortholeft) * srcAspectRatio) +
            //               (0.5f *  (orthoright - orthoLeft) * aspectRatio * m_OrthoSize);
            //
            // float bottom =(0.5f * (orthoDown - orthoUp) + orthoUp) * srcAspectRatio- 
            //               (0.5f * (orthoDown - orthoUp) * aspectRatio * m_OrthoSize);
            //
            // float top    =(0.5f * (orthoDown - orthoUp) + orthoUp) * srcAspectRatio + 
            //               (0.5f * (orthoDown - orthoUp) * aspectRatio * m_OrthoSize);
            //
            // return Matrix4x4.CreateOrthographicOffCenter(left,right, bottom, top,-1.0f, 10000.0f);
        }

        public override string GetItemType()
        {
            return "Camera";
        }

        public override void EditorUpdate(double dt)
        {
            base.EditorUpdate(dt);
        }

        public void AdjustProjection(float X, float Y)
        {
            _projectionSize.X = X;
            _projectionSize.Y = Y;
        }

        public override float GetFieldSize()
        {
            return 100;
        }

        public override void ImGuiFields()
        {
            if (OpenTKUIHelper.DrawProperty("Size: ", ref _size, label: false))
            {
            }
            
            ImGui.Separator();
            ImGui.Text("Clipping Planes");

            if (OpenTKUIHelper.DrawProperty("Near: ", ref _near, label: false) ||
                OpenTKUIHelper.DrawProperty("Far: ", ref _far, label: false))
            {
            }
                
        }

        public void SetViewportSize(OpenTK.Mathematics.Vector2i viewportSize)
        {
            _projectionSize.X = viewportSize.X;
            _projectionSize.Y = viewportSize.Y;
        }
    }