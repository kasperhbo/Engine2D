using ImGuiNET;
using KDBEngine.Core;
using KDBEngine.UI;

namespace Engine2D.UI;

internal class Inspector : UIElemenet
{
    //internal Gameobject CurrentSelectedGameObject;

    internal Inspector()
    {
        Title = "Inspector";
        _flags = ImGuiWindowFlags.None;
        _windowContents = () => { Engine.Get().CurrentSelectedAsset?.OnGui(); };
    }
}