#region

using System.Numerics;
using Engine2D.Core.Inputs;
using ImGuiNET;

#endregion

namespace Engine2D.UI.Viewports;

internal class GameViewport : ViewportWindow
{
    internal override void BeforeImageRender()
    {
    }

    internal override void AfterImageRender()
    {
        Input.CalculateMouseGame(this, Camera);
    }

    public GameViewport(string title) : base(title)
    {
    }
}