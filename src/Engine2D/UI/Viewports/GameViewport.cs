using System.Numerics;
using ImGuiNET;

namespace Engine2D.UI.Viewports;

internal class GameViewport : ViewportWindow
{
    public static bool IsInViewport { get; } = false;

    public override void BeforeImageRender()
    {
        
    }

    public override void AfterImageRender()
    {
        
    }

    public override Vector2 GetVPSize()
    {
        var ws = ImGui.GetContentRegionAvail();

        float targetAspectRatio = 16 / 9;
        float aspectWidth = ws.X;
        
        float aspectHeight = aspectWidth / targetAspectRatio;
        
        if (aspectHeight > ws.Y) {
            // We must switch to pillarbox mode
            aspectHeight = ws.Y;
            aspectWidth = aspectHeight * targetAspectRatio;
        }

        return new Vector2(aspectWidth, aspectHeight);
    }
}