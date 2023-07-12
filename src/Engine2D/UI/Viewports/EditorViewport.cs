#region

using System.Numerics;
using Engine2D.Core.Inputs;
using Engine2D.Rendering.NewRenderer;
using ImGuiNET;

#endregion

namespace Engine2D.UI.Viewports;

internal class EditorViewport : ViewportWindow
{
   
    protected override void BeforeImageRender()
    {
    }

    protected override void RenderImage()
    {
        if(Renderer.EditorFrameBuffer != null)
            ImGui.Image(Renderer.EditorFrameBuffer.TextureID, new Vector2(WindowSize.X, WindowSize.Y), new Vector2(0, 1), new Vector2(1, 0));
        else
            ImGui.Image(IntPtr.Zero, new Vector2(WindowSize.X, WindowSize.Y), new Vector2(0, 1), new Vector2(1, 0));
    }

    protected override void AfterImageRender()
    {
        Input.CalculateMouseEditor(this, Camera);
        SceneControls.GuizmoControls(Origin, Size, Camera);
    }
    
    public EditorViewport(string title) : base(title)
    {
    }
}