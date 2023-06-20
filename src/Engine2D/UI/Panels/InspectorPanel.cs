using Engine2D.Core;

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