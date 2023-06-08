using ImGuiNET;
using Engine2D.Core;
using Engine2D.Logging;

namespace Engine2D.UI;

internal class Inspector : UiElemenet
{
    //internal Gameobject CurrentSelectedGameObject;

    protected override string GSetWindowTitle()
    {
        return "Inspector";
    }

    protected override ImGuiWindowFlags GetWindowFlags()
    {
        return ImGuiWindowFlags.None;
    }

    protected override Action GetWindowContent()
    {
        return () =>
        {
            Engine.Get().CurrentSelectedAsset?.OnGui();
        };
    }
}