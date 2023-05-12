using Engine2D.Components;
using System.Numerics;

namespace ExampleGame;

public class NewComponent : Component
{
    public override string GetItemType()
    {
        return "NewComponent";
    }
}