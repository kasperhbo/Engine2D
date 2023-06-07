using Engine2D.Cameras;
using Engine2D.Components;
using Engine2D.Components.TransformComponents;
using Engine2D.Testing;
using Engine2D.Core;
using OpenTK.Mathematics;

namespace Engine2D.GameObjects;

public class CameraGO : Gameobject
{
    public CameraGO(string name) : base(name)
    {
        var currentScene = Engine.Get().CurrentScene;
        Camera camera = new Camera();
        camera.Parent = this;
        components.Add(camera);
        
        if (currentScene != null)
        {
            Name = "Camera: " + currentScene.GameObjects.Count + 1;

            // currentScene.AddGameObjectToScene(this);
        }

    }
}