using Engine2D.UI.ImGuiExtension;
using ImGuiNET;

namespace Engine2D.UI;

public class UISettingsPanel : UIElement
{
    private ImGuiStylePtr style;
    
    public UISettingsPanel(string title) : base(title)
    {
        style = ImGui.GetStyle();
    }


    public override void Render()
    {
        GetColorComponents();
    }

    private readonly string[] colors =
    {
        "Text",
        "TextDisabled",
        "WindowBg",
        "ChildBg",
        "PopupBg",
        "Border",
        "BorderShadow",
        "FrameBg",
        "FrameBgHovered",
        "FrameBgActive",
        "TitleBg",
        "TitleBgActive",
        "TitleBgCollapsed",
        "MenuBarBg",
        "ScrollbarBg",
        "ScrollbarGrab",
        "ScrollbarGrabHovered",
        "ScrollbarGrabActive",
        "CheckMark",
        "SliderGrab",
        "SliderGrabActive",
        "Button",
        "ButtonHovered",
        "ButtonActive",
        "Header",
        "HeaderHovered",
        "HeaderActive",
        "Separator",
        "SeparatorHovered",
        "SeparatorActive",
        "ResizeGrip",
        "ResizeGripHovered",
        "ResizeGripActive",
        "Tab",
        "TabHovered",
        "TabActive",
        "TabUnfocused",
        "TabUnfocusedActive",
        "DockingPreview",
        "DockingEmptyBg",
        "PlotLines",
        "PlotLinesHovered",
        "PlotHistogram",
        "PlotHistogramHovered",
        "TableHeaderBg",
        "TableBorderStrong",
        "TableBorderLight",
        "TableRowBg",
        "TableRowBgAlt",
        "TextSelectedBg",
        "DragDropTarget",
        "NavHighlight",
        "NavWindowingHighlight",
        "NavWindowingDimBg",
        "ModalWindowDimBg",
    };

    private void GetColorComponents()
    {
        for (int i = 0; i < colors.Length; i++)
        {
            OpenTkuiHelper.DrawProperty(colors[i], ref style.Colors[i]);
        }
    }
    
    
}