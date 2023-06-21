#region

using Engine2D.Core;

#endregion

namespace Engine2D.UI;

internal class InspectorPanel : UIElement
{
    //internal Gameobject CurrentSelectedGameObject;

    internal InspectorPanel(string title) : base(title)
    {
        IsVisible = true;
    }

    internal override void Render()
    {
        Engine.Get().CurrentSelectedAsset?.OnGui();
    }
}