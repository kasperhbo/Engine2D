using Engine2D.Cameras;
using Engine2D.Testing;
using ImGuiNET;
using Engine2D.Core;
using OpenTK.Mathematics;
using Veldrid;
using Vector2 = System.Numerics.Vector2;

namespace Engine2D.UI.Viewports;

public abstract class ViewportWindow
{
    protected Vector2 Origin = new();
    protected Vector2 Sz = new();

    protected Vector2i WindowSize;

    protected Camera Camera;
    private TestFrameBuffer _frameBuffer;
    
    public virtual void Begin(string title, Camera? cameraToRender, TestFrameBuffer buffer)
    {
        Camera = cameraToRender;
        _frameBuffer = buffer;
        
        ImGui.Begin(title);
        BeforeImageRender();
        RenderImage();
        AfterImageRender();
        End();
    }
    
    public abstract void BeforeImageRender();
    
    public virtual void RenderImage()
    {
        WindowSize = new Vector2i((int)GetVPSize().X, (int)GetVPSize().Y);

        var cursorPos = ImGui.GetCursorPos();
        ImGui.SetCursorPos(cursorPos);
        ImGui.Image(_frameBuffer.TextureID, new(WindowSize.X, WindowSize.Y),
            new(0, 1), new(1, 0));

        Origin = ImGui.GetItemRectMin();
        Sz = ImGui.GetItemRectSize();
    }

    public abstract void AfterImageRender();

    public virtual void End()
    {
        ImGui.End();
    }
    
    
    public abstract Vector2 GetVPSize();
    
}