using System.Numerics;
using Engine2D.Cameras;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.Core.Inputs;
using Engine2D.UI;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine2D.GameObjects;

public class EditorCameraGO : Gameobject
{
    private bool reset = false;
    
    private float dragSensitivity = 15f;
    private float scrollSensitivity = 1f;
    private readonly Camera camera;

    private List<object> _linkedComponents = new();
    
    public EditorCameraGO(string name) : base(name)
    {
        var currentScene = Engine.Get().CurrentScene;
        
        camera = new Camera
        {
            Parent = this
        };

        components.Add(camera);
        
        if (currentScene != null)
        {
            Name = "Launcher Camera: " + currentScene.GameObjects.Count + 1;
        }
    }

    public override void EditorUpdate(double dt)
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


            GetComponent<Transform>()!.Position.X
                -= delta.X * (dt * dragSensitivity);

            GetComponent<Transform>()!.Position.Y
                -= delta.Y * (dt * dragSensitivity);
        }

        if (Input.MouseScroll())
        {
            camera.Size += Input.MouseScrollDelta().Y * scrollSensitivity;
        }
    }
}