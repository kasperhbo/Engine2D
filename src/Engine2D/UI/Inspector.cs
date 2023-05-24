using ImGuiNET;
using KDBEngine.Core;
using KDBEngine.UI;

namespace Engine2D.UI;

internal class Inspector : UIElemenet
{
    //internal Gameobject CurrentSelectedGameObject;

    internal Inspector()
    {
    }

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