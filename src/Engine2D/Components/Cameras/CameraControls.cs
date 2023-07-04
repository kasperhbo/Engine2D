using System.Numerics;
using Engine2D.Cameras;
using Engine2D.Components.TransformComponents;
using Engine2D.Core.Inputs;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Engine2D.UI;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine2D.Components.Cameras;

public class CameraControls : Component
{    
    internal Camera? Camera = null;
    private readonly float _dragSensitivity = 50f;
    private readonly float _scrollSensitivity = .1f;
    
    public override void Init()
    {
        Camera = Parent?.GetComponent<Camera>();
    }
    
    public override void StartPlay()
    {
        
    }

    public override void Update(FrameEventArgs args)
    {
        
    }

    public override void EditorUpdate(double dt)
    {
        base.EditorUpdate(dt);
        MouseControl((float)dt);
    }

    private void MouseControl(float dt)
    {
        if (Camera == null)
        {
            Console.WriteLine("Camera null");
            Camera = Parent?.GetComponent<Camera>();
            return;
        }
        
        if ((UiRenderer.CurrentEditorViewport == null || !UiRenderer.CurrentEditorViewport.IsFocused()) && Settings.s_IsEngine) return;
        
        if (Input.MouseDown(MouseButton.Middle))
        {
            Vector2 delta = new(Input.ScreenX() - Input.ScreenLastX(), Input.ScreenY() - Input.ScreenLastY());
            Transform transform = Parent.GetComponent<Transform>();

            float speed = _dragSensitivity;
            if (Input.KeyDown(Keys.LeftShift))
            {
                speed *= 2;
            }
            
            if (Input.KeyDown(Keys.A))
            {
                transform.Position -= new Vector2( speed * dt, 0);
            }
            else if (Input.KeyDown(Keys.D))
            {
                transform.Position += new Vector2(speed * dt, 0);
            }
            else if (Input.KeyDown(Keys.W))
            {
                transform.Position += new Vector2(0, speed * dt);
            }
            else if (Input.KeyDown(Keys.S))
            {
                transform.Position += new Vector2(0, -speed * dt);
            }
        }

        if (Input.MouseScroll())
            Camera.Size += Input.MouseScrollDelta().Y * _scrollSensitivity;
    }
}