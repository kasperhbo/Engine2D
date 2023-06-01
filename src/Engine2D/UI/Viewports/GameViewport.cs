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
        var windowSize = ImGui.GetContentRegionAvail();
        windowSize.X -= ImGui.GetScrollX();
        windowSize.Y -= ImGui.GetScrollY();
        float targetAspectRatio = 16/9;
        
        if(Camera != null)
            targetAspectRatio = Camera.ProjectionSize.X / Camera.ProjectionSize.Y;
        
        var aspectWidth = windowSize.X;
        var aspectHeight = aspectWidth / targetAspectRatio;
        if (aspectHeight > windowSize.Y)
        {
            // We must switch to pillarbox mode
            aspectHeight = windowSize.Y;
            aspectWidth = aspectHeight * targetAspectRatio;
        }

        return new Vector2(aspectWidth, aspectHeight);
        
    }
}