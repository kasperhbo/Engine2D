#region

using System.Numerics;
using ImGuiNET;

#endregion

namespace Engine2D.UI.Viewports;

internal class GameViewport : ViewportWindow
{
    internal static bool IsInViewport { get; } = false;

    internal override void BeforeImageRender()
    {
    }

    internal override void AfterImageRender()
    {
    }

    internal override Vector2 GetVPSize()
    {
        var ws = ImGui.GetContentRegionAvail();

        float targetAspectRatio = 16 / 9;
        var aspectWidth = ws.X;

        var aspectHeight = aspectWidth / targetAspectRatio;

        if (aspectHeight > ws.Y)
        {
            // We must switch to pillarbox mode
            aspectHeight = ws.Y;
            aspectWidth = aspectHeight * targetAspectRatio;
        }

        return new Vector2(aspectWidth, aspectHeight);
    }
}