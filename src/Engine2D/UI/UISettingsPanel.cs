using Engine2D.UI.ImGuiExtension;
using ImGuiNET;
using imnodesNET;

namespace Engine2D.UI;

public class UISettingsPanel : UiElemenet
{
    private ImGuiStylePtr style;
    
    public UISettingsPanel() : base(){
    
        style = ImGui.GetStyle();
    }
    
    protected override string GetWindowTitle()
    {
        return "UI Settings Panel";
    }

    protected override ImGuiWindowFlags GetWindowFlags()
    {
        return ImGuiWindowFlags.None;
    }

    protected override Action GetWindowContent()
    {
        return () =>
        {
            if (_visibility)
            {
                OpenTkuiHelper.DrawComponentWindow(ImGui.GetID("UI Colors").ToString(), 
                    "UI Colors", GetColorComponents, 100000);
            }
        };
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