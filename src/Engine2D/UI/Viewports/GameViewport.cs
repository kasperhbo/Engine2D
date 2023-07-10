#region

using System.Numerics;
using Engine2D.Core.Inputs;
using Engine2D.Rendering.NewRenderer;
using ImGuiNET;

#endregion

namespace Engine2D.UI.Viewports;

internal class GameViewport : ViewportWindow
{
    protected override void BeforeImageRender()
    {
    }

    protected override void RenderImage()
    {
        if(Renderer.GameFrameBuffer != null)
            ImGui.Image(Renderer.GameFrameBuffer.TextureID, new Vector2(WindowSize.X, WindowSize.Y), new Vector2(0, 1), new Vector2(1, 0));
        else
            ImGui.Image(IntPtr.Zero, new Vector2(WindowSize.X, WindowSize.Y), new Vector2(0, 1), new Vector2(1, 0));
    }

    protected override void AfterImageRender()
    {
        Input.CalculateMouseGame(this, Camera);
    }

    public GameViewport(string title) : base(title)
    {
    }
}