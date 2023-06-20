//FOLDER:  D:\dev\Engine2D\src\ExampleGame\Assets\TestClass.cs

using System.Drawing;
using Engine2D.Components;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Rendering;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ExampleGame.Assets;

public class RunnerComponent : Component
{
    public override void Init(Gameobject parent)
    {
        Log.Succes("Loaded RunnerComponent class");
        base.Init(parent);
    }

    public override void Init(Gameobject parent, Renderer? renderer)
    {
        Log.Succes("Loaded RunnerComponent class");
        base.Init(parent, renderer);
    }

    public override void EditorUpdate(double dt)
    {


        base.EditorUpdate(dt);
    }

    public override string GetItemType()
    {
        return "ExampleGame.Assets.RunnerComponent";
    }
}