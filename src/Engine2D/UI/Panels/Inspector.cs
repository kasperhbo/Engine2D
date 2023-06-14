using ImGuiNET;
using Engine2D.Core;
using Engine2D.Logging;

namespace Engine2D.UI;

internal class Inspector : UIElement
{
    //internal Gameobject CurrentSelectedGameObject;

    public Inspector(string title) : base(title)
    {
    }

    public override void Render()
    {
        Engine.Get().CurrentSelectedAsset?.OnGui();
    }
}