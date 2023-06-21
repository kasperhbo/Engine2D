//FOLDER:  D:\dev\Engine2D\src\ExampleGame\Assets\TestClass.cs


using Engine2D.Components;
using Engine2D.Logging;

namespace ExampleGame.Assets;

public class RunnerComponent : Component
{
    public int counter = 01;
    
    public override void EditorUpdate(double dt)
    {

        base.EditorUpdate(dt);
    }

    public override string GetItemType()
    {
        return "ExampleGame.Assets.RunnerComponent";
    }
}