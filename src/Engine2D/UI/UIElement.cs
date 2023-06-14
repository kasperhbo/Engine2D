using System.Numerics;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;

namespace Engine2D.UI;

public abstract class UIElement
{
    protected string Title = "";

    protected ImGuiWindowFlags Flags = ImGuiWindowFlags.None;
    public bool IsVisible { get; protected set; } = true;
    protected bool IsHovering = false;

    protected bool IsFocussed = false;

    protected TopBarButton _closeButton = new TopBarButton("X", new Vector4(1, 0, 0, 1));


    public UIElement(string title)
    {
        this.Title = title;
    }

    public virtual void BeginRender()
    {
        ImGui.PushID(Title);
        ImGui.Begin(Title, Flags);

        IsFocussed = ImGui.IsWindowFocused(ImGuiFocusedFlags.RootWindow) |
                     ImGui.IsWindowFocused(ImGuiFocusedFlags.ChildWindows);
         
        RenderTopBar();
    }

    public virtual void RenderTopBar()
    {
        if (Gui.TopBarButton(ImGui.GetContentRegionMax().X-20, new(20, 20), _closeButton))
        {
            UiRenderer.RemoveGuiWindow(this);
        }
        
        ImGui.Separator();
    }

    
    
    public abstract void Render();

    public virtual void EndRender()
    {
        ImGui.End();
        ImGui.PopID();
        

    }
}