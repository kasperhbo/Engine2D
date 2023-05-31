using ImGuiNET;
using Engine2D.Core;

namespace Engine2D.UI;

internal class Inspector : UiElemenet
{
    //internal Gameobject CurrentSelectedGameObject;

    protected override string SetWindowTitle()
    {
        return "Inspector";
    }

    protected override ImGuiWindowFlags SetWindowFlags()
    {
        return ImGuiWindowFlags.None;
    }

    protected override Action SetWindowContent()
    {
        return () => { Engine.Get().CurrentSelectedAsset?.OnGui(); };
    }
}