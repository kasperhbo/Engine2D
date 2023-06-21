#region

using System.Numerics;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;
using OpenTK.Windowing.Common;

#endregion

namespace Engine2D.UI;

internal struct UISETTINGS
{
    internal static Vector2 ImageUV0 = new(0, 1);
    internal static Vector2 ImageUV1 = new(1, 0);
}

internal abstract class UIElement
{
    protected TopBarButton _closeButton = new("X", new Vector4(1, 0, 0, 1));

    protected ImGuiWindowFlags Flags = ImGuiWindowFlags.None;

    protected bool IsFocussed;
    protected bool IsHovering;
    protected string Title = "";


    internal UIElement(string title)
    {
        Title = title;
    }

    protected internal bool IsVisible { get; set; } = true;

    internal virtual void BeginRender()
    {
        ImGui.PushID(Title);
        ImGui.Begin(Title, Flags);

        IsFocussed = ImGui.IsWindowFocused(ImGuiFocusedFlags.RootWindow) |
                     ImGui.IsWindowFocused(ImGuiFocusedFlags.ChildWindows);

        RenderTopBar();
    }

    internal virtual void RenderTopBar()
    {
        if (Gui.TopBarButton(ImGui.GetContentRegionMax().X - 20, new Vector2(20, 20), _closeButton))
            UiRenderer.RemoveGuiWindow(this);

        ImGui.Separator();
    }


    internal abstract void Render();

    internal virtual void EndRender()
    {
        if (ImGui.IsWindowHovered(ImGuiHoveredFlags.ChildWindows | ImGuiHoveredFlags.RootWindow))
            IsHovering = true;
        else
            IsHovering = false;

        ImGui.End();
        ImGui.PopID();
    }


    internal virtual void FileDrop(FileDropEventArgs obj)
    {
    }
}