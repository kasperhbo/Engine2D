#region

using Engine2D.Core.Inputs;

#endregion

namespace Engine2D.UI.Viewports;

internal class GameViewport : ViewportWindow
{
    protected override void BeforeImageRender()
    {
    }

    protected override void AfterImageRender()
    {
        Input.CalculateMouseGame(this, Camera);
    }

    public GameViewport(string title) : base(title)
    {
    }
}