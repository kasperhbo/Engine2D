using System.Diagnostics;
using System.Numerics;
using Dear_ImGui_Sample;
using Engine2D.Cameras;
using Engine2D.Components;
using Engine2D.Components.TransformComponents;
using Engine2D.Testing;
using Engine2D.Core;
using Engine2D.Core.Inputs;
using Engine2D.Logging;
using Engine2D.UI.Viewports;
using Newtonsoft.Json.Serialization;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vortice.Direct3D;
using Vulkan;

namespace Engine2D.GameObjects;

public class EditorCameraGO : Gameobject
{
    private bool reset = false;
    
    private float dragSensitivity = 15f;
    private float scrollSensitivity = 1f;
    private readonly Camera camera;
    
    public EditorCameraGO(string name) : base(name)
    {
        var currentScene = Engine.Get().CurrentScene;
        
        camera = new Camera()
        {
            Parent = this
        };

        components.Add(camera);
        
        if (currentScene != null)
        {
            Name = "Editor Camera: " + currentScene.GameObjects.Count + 1;
        }

    }

    public override void EditorUpdate(double dt)
    {
        base.EditorUpdate(dt);
        MouseControl((float)dt);
    }

    private void MouseControl(float dt)
    {
        if (UIRenderer.CurrentEditorViewport != null && UIRenderer.CurrentEditorViewport.IsWindowHovered)
        {
            if (Input.MouseDown(MouseButton.Middle))
            {
                Vector2 delta = new(Input.ScreenX() - Input.ScreenLastX(), Input.ScreenY() - Input.ScreenLastY());


                this.GetComponent<Transform>()!.Position.X
                    -= delta.X * (dt * dragSensitivity);

                this.GetComponent<Transform>()!.Position.Y
                    -= delta.Y * (dt * dragSensitivity);
            }

            if (Input.MouseScroll())
            {
                camera.Size += Input.MouseScrollDelta().Y * scrollSensitivity;
            }
        }
    }
}