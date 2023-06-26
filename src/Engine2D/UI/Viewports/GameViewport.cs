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
        // if(Camera != null)
        //     Camera.ProjectionSize = new(1920, 1080);
    }

    internal override void AfterImageRender()
    {
    }

    internal override Vector2 GetVPSize()
    {
        var ws = ImGui.GetContentRegionAvail();

        var targetAspectRatio = 16f / 9f;

        var aspectWidth = ws.X;
        var aspectHeight = aspectWidth / targetAspectRatio;

        if (aspectHeight > ws.Y)
        {
            aspectHeight = ws.Y;
            aspectWidth = aspectHeight * targetAspectRatio;
        }

        return new Vector2(aspectWidth, aspectHeight);
    }
}