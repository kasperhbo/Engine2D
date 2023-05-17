using System.Numerics;
using Engine2D.Rendering;
using Engine2D.Testing;
using ImGuiNET;
using KDBEngine.UI;

namespace Engine2D.UI;

public class RenderDebugUI : UIElemenet
{
    protected override string SetWindowTitle()
    {
        return "Render DEBUG UI";
    }

    protected override ImGuiWindowFlags SetWindowFlags()
    {
        return ImGuiWindowFlags.None;
    }

    protected override Action SetWindowContent()
    {
        return () =>
        {
            var windowSize = TestViewportWindow.getLargestSizeForViewport()/3;
            ImGui.Text("Draw calls: " + GameRenderer.DrawCalls);
            ImGui.Text("Render Batches: " + GameRenderer.RenderBatches);
            ImGui.Text("Light Frame Buffer");
            ImGui.Image((IntPtr)GameRenderer.LightFrameBuffer.GetTextureID, new Vector2(windowSize.X, windowSize.Y), new Vector2(0, 1), new Vector2(1, 0));
            ImGui.Text("Scene Frame Buffer");
            ImGui.Image((IntPtr)GameRenderer.SceneFrameBuffer.GetTextureID, new Vector2(windowSize.X, windowSize.Y), new Vector2(0, 1), new Vector2(1, 0));
            ImGui.Text("Game Frame Buffer");
            ImGui.Image((IntPtr)GameRenderer.FrameBuffer.GetTextureID, new Vector2(windowSize.X, windowSize.Y), new Vector2(0, 1), new Vector2(1, 0));
        };
    }
}