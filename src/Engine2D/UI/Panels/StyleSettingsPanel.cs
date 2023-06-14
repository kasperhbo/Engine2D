using System.Numerics;
using Dear_ImGui_Sample;
using Engine2D.Core;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;

namespace Engine2D.UI;

public class StyleSettingsPanel : UIElement
{
    private int _currentStyle = 0;
    private int _currentFont = 0;
    
    public StyleSettingsPanel(string title) : base(title)
    {
    }

    public override void Render()
    {
        {

            if (ImGui.Combo("Style: ", ref _currentStyle, 
                    "Default (dark)\0" + 
                    "Default always RED buttons (dark)\0" +
                    "Default gray buttons (dark)\0" + 
                    "Colors on black (dark)\0" + 
                    "Solarized (dark)\0" + 
                    "SetupStyle\0" + 
                    "Cyan/White on Grey (dark)\0" + 
                    "Cyan/Yellow on Gray/Black (dark)\0" + 
                    "Red on Gray/Black (dark)\0" +
                    "Cyan/Yellow on White (light)\0" +
                    "Grey scale on White (light)\0" +
                    "ImGui Classic\0" +
                    "ImGui Dark\0" +
                    "ImGui Light\0" +
                    "chatgpt\0" +
                    "chatgptlight\0"
                ))
            {
                ImGuiStyleManager.SelectTheme(_currentStyle);
                ImGui.EndCombo();
            }

            string robotoPath = Utils.GetBaseEngineDir() + "\\UI\\Fonts\\Roboto\\";
            string[] fonts = Directory.GetFiles(robotoPath);
            string[] fontsShortened = new string[fonts.Length];

            for (int i = 0; i < fonts.Length; i++)
            {
                fontsShortened[i] = fonts[i].Remove(0, robotoPath.Length);
            }
            
            if (ImGui.BeginCombo("FontsCombo", fontsShortened[_currentFont])) {
                for (int i = 0; i < fonts.Length; ++i) { 
                    bool isSelected = (_currentFont == i);
                    if (ImGui.Selectable(fontsShortened[i], isSelected)) {
                        _currentFont = i;
                        UiRenderer.ChangeFont(fonts[i], 22);
                    }
                    
                    

                    // Set the initial focus when opening the combo
                    // (scrolling + keyboard navigation focus)
                    if (isSelected) {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }
        };
    }
}