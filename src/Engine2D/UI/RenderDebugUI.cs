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
        return Renderer.GetDebugGUI();
    }
}