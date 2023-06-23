#region

using System.Numerics;
using System.Transactions;
using Engine2D.Cameras;
using Engine2D.Components.Sprites;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.Core.Inputs;
using Engine2D.UI;
using OpenTK.Windowing.GraphicsLibraryFramework;

#endregion

namespace Engine2D.GameObjects;

internal class EditorCameraGO : Gameobject
{
    private readonly List<object> _linkedComponents = new();
    private readonly Camera camera;

    private readonly float dragSensitivity = 50f;
    private readonly float scrollSensitivity = .1f;
    private bool reset = false;

    internal EditorCameraGO(string name) : base(name)
    {
        var currentScene = Engine.Get().CurrentScene;

        camera = new Camera
        {
            Parent = this
        };

        Components.Add(camera);
        
        if (currentScene != null) Name = "Editor Camera: " + currentScene.GameObjects.Count + 1;
    }

    internal override void EditorUpdate(double dt)
    {
        base.EditorUpdate(dt);
        MouseControl((float)dt);

        foreach (var f in _linkedComponents)
        {
        }
    }

    private void MouseControl(float dt)
    {
        if (UiRenderer.CurrentEditorViewport == null || !UiRenderer.CurrentEditorViewport.IsWindowHovered) return;

        if (Input.MouseDown(MouseButton.Middle))
        {
            Vector2 delta = new(Input.ScreenX() - Input.ScreenLastX(), Input.ScreenY() - Input.ScreenLastY());
            Transform transform = this.GetComponent<Transform>();

            float speed = dragSensitivity;
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

        if (Input.MouseScroll()) camera.Size += Input.MouseScrollDelta().Y * scrollSensitivity;
    }
}