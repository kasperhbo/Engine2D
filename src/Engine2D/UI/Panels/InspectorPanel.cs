using ImGuiNET;
using Engine2D.Core;
using Engine2D.Logging;

namespace Engine2D.UI;

internal class InspectorPanel : UIElement
{
    //internal Gameobject CurrentSelectedGameObject;

    public InspectorPanel(string title) : base(title)
    {
    }

    public override void Render()
    {
        Engine.Get().CurrentSelectedAsset?.OnGui();
    }
}