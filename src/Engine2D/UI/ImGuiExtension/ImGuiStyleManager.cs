using System.Numerics;
using ImGuiNET;

namespace Engine2D.UI.ImGuiExtension;

// From https://github.com/procedural/gpulib/blob/master/gpulib_imgui.h

public static class ImGuiStyleManager
{
static void lightCyanYellow()
{
    var style = ImGui.GetStyle();

    style.Colors[(int)ImGuiCol.Text]                 = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.TextDisabled]         = new Vector4(0.59f, 0.59f, 0.59f, 1.00f);
    style.Colors[(int)ImGuiCol.WindowBg]             = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
    style.Colors[(int)ImGuiCol.ChildBg]              = new Vector4(0.92f, 0.92f, 0.92f, 1.00f);
    style.Colors[(int)ImGuiCol.PopupBg]              = new Vector4(0.92f, 0.92f, 0.92f, 1.00f);
    style.Colors[(int)ImGuiCol.Border]               = new Vector4(0.00f, 0.00f, 0.00f, 0.80f);
    style.Colors[(int)ImGuiCol.BorderShadow]         = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
    style.Colors[(int)ImGuiCol.FrameBg]              = new Vector4(0.71f, 0.71f, 0.71f, 0.39f);
    style.Colors[(int)ImGuiCol.FrameBgHovered]       = new Vector4(0.00f, 0.59f, 0.80f, 0.43f);
    style.Colors[(int)ImGuiCol.FrameBgActive]        = new Vector4(0.00f, 0.47f, 0.71f, 0.67f);
    style.Colors[(int)ImGuiCol.TitleBg]              = new Vector4(1.00f, 1.00f, 1.00f, 0.80f);
    style.Colors[(int)ImGuiCol.TitleBgCollapsed]     = new Vector4(0.78f, 0.78f, 0.78f, 0.39f);
    style.Colors[(int)ImGuiCol.TitleBgActive]        = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
    style.Colors[(int)ImGuiCol.MenuBarBg]            = new Vector4(0.90f, 0.90f, 0.90f, 1.00f);
    style.Colors[(int)ImGuiCol.ScrollbarBg]          = new Vector4(0.20f, 0.25f, 0.30f, 0.60f);
    style.Colors[(int)ImGuiCol.ScrollbarGrab]        = new Vector4(0.00f, 0.00f, 0.00f, 0.39f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.00f, 0.00f, 0.00f, 0.59f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabActive]  = new Vector4(0.00f, 0.00f, 0.00f, 0.78f);
    style.Colors[(int)ImGuiCol.CheckMark]            = new Vector4(0.27f, 0.59f, 0.75f, 1.00f);
    style.Colors[(int)ImGuiCol.SliderGrab]           = new Vector4(0.00f, 0.00f, 0.00f, 0.35f);
    style.Colors[(int)ImGuiCol.SliderGrabActive]     = new Vector4(0.00f, 0.00f, 0.00f, 0.59f);
    style.Colors[(int)ImGuiCol.Button]               = new Vector4(0.00f, 0.00f, 0.00f, 0.27f);
    style.Colors[(int)ImGuiCol.ButtonHovered]        = new Vector4(0.00f, 0.59f, 0.80f, 0.43f);
    style.Colors[(int)ImGuiCol.ButtonActive]         = new Vector4(0.00f, 0.47f, 0.71f, 0.67f);
    style.Colors[(int)ImGuiCol.Header]               = new Vector4(0.71f, 0.71f, 0.71f, 0.39f);
    style.Colors[(int)ImGuiCol.HeaderHovered]        = new Vector4(0.20f, 0.51f, 0.67f, 1.00f);
    style.Colors[(int)ImGuiCol.HeaderActive]         = new Vector4(0.08f, 0.39f, 0.55f, 1.00f);
    style.Colors[(int)ImGuiCol.Separator]            = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.SeparatorHovered]     = new Vector4(0.27f, 0.59f, 0.75f, 1.00f);
    style.Colors[(int)ImGuiCol.SeparatorActive]      = new Vector4(0.08f, 0.39f, 0.55f, 1.00f);
    style.Colors[(int)ImGuiCol.ResizeGrip]           = new Vector4(0.00f, 0.00f, 0.00f, 0.78f);
    style.Colors[(int)ImGuiCol.ResizeGripHovered]    = new Vector4(0.27f, 0.59f, 0.75f, 0.78f);
    style.Colors[(int)ImGuiCol.ResizeGripActive]     = new Vector4(0.08f, 0.39f, 0.55f, 0.78f);
    style.Colors[(int)ImGuiCol.PlotLines]            = new Vector4(0.00f, 0.00f, 0.60f, 1.00f);
    style.Colors[(int)ImGuiCol.PlotLinesHovered]     = new Vector4(0.00f, 0.30f, 0.60f, 1.00f);
    style.Colors[(int)ImGuiCol.PlotHistogram]        = new Vector4(0.90f, 0.70f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(1.00f, 0.60f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.TextSelectedBg]       = new Vector4(0.27f, 0.59f, 0.75f, 1.00f);
    style.Colors[(int)ImGuiCol.ModalWindowDimBg]     = new Vector4(0.00f, 0.00f, 0.00f, 0.35f);

}

// Dynamic theme, freely inspired from a static theme of enemymouse: 
// https://github.com/ocornut/imgui/issues/539#issuecomment-204412632
// https://gist.github.com/enemymouse/c8aa24e247a1d7b9fc33d45091cbb8f0
static void darkColorsOnBlack(Vector4 color)
{
    var style = ImGui.GetStyle();

    float kC = .25f;
    float lum = color.X*.299f + color.Y*.587f + color.Z*.114f;
    float lumK = 1 + lum * .25f;

    var wBG = new Vector4 (color.X*kC, color.Y*kC, color.Z*kC, 1.0f);

    
    var clA = new Vector4 (wBG.X*4.3f*lumK, wBG.Y*4.3f*lumK, wBG.Z*4.3f*lumK, 1.00f); //0.00f, 0.55f, 0.87f
    var clB = new Vector4 (wBG.X*1.5f*lumK, wBG.Y*1.5f*lumK, wBG.Z*1.5f*lumK, 1.00f); //0.20f, 0.22f, 0.27f
    var clC = new Vector4 (wBG.X*3.0f*lumK, wBG.Y*3.0f*lumK, wBG.Z*3.0f*lumK, 1.00f); //0.20f, 0.22f, 0.27f
    var clD = new Vector4 (wBG.X*3.7f*lumK, wBG.Y*3.7f*lumK, wBG.Z*3.7f*lumK, 1.00f);

    
    var HSV = new Vector4 ();

    float diff = 1f/(lumK*lumK*lumK);
    clD.X *= diff;
    clD.Y *= diff;
    clD.Z *= diff;

    var txt = new Vector4 ();
    ImGui.ColorConvertRGBtoHSV(wBG.X, wBG.Y, wBG.Z, out HSV.X, out HSV.Y, out HSV.Z);
    if(HSV.Y>.25) HSV.Y=.25f; 
    HSV.Z=.85f;
    ImGui.ColorConvertHSVtoRGB(HSV.X, HSV.Y, HSV.Z, out txt.X, out txt.Y, out txt.Z);

    var chk = new Vector4 ((txt.X+color.X)*.5f, (txt.Y+color.Y)*.5f, (txt.Z+color.Z)*.5f, 1.0f) ;

    var ch1 = new Vector4 (chk.X*0.60f, chk.Y*.60f, chk.Z*.60f, 1.00f);
    var ch2 = new Vector4 (chk.X*0.80f, chk.Y*.80f, chk.Z*.80f, 1.00f);


    style.Colors[(int)ImGuiCol.Text]                 = new Vector4(txt.X, txt.Y, txt.Z, 1.00f);
    style.Colors[(int)ImGuiCol.TextDisabled]         = new Vector4(txt.X, txt.Y, txt.Z, 1.00f);
    style.Colors[(int)ImGuiCol.WindowBg]             = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.ChildBg]              = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
    style.Colors[(int)ImGuiCol.PopupBg]              = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.Border]               = new Vector4(clA.X, clA.Y, clA.Z, 0.65f);
    style.Colors[(int)ImGuiCol.BorderShadow]         = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
    style.Colors[(int)ImGuiCol.FrameBg]              = new Vector4(clD.X, clD.Y, clD.Z, 0.35f);
    style.Colors[(int)ImGuiCol.FrameBgHovered]       = new Vector4(clD.X, clD.Y, clD.Z, 0.66f);
    style.Colors[(int)ImGuiCol.FrameBgActive]        = new Vector4(clD.X, clD.Y, clD.Z, 0.85f);
    style.Colors[(int)ImGuiCol.TitleBg]              = new Vector4(clB.X, clB.Y, clB.Z, 0.50f);
    style.Colors[(int)ImGuiCol.TitleBgCollapsed]     = new Vector4(0.00f, 0.00f, 0.00f, 0.75f);
    style.Colors[(int)ImGuiCol.TitleBgActive]        = new Vector4(clB.X, clB.Y, clB.Z, 0.85f);
    style.Colors[(int)ImGuiCol.MenuBarBg]            = new Vector4(0.00f, 0.00f, 0.00f, 0.20f);
    style.Colors[(int)ImGuiCol.ScrollbarBg]          = new Vector4(clB.X, clB.Y, clB.Z, 0.35f);
    style.Colors[(int)ImGuiCol.ScrollbarGrab]        = new Vector4(clA.X, clA.Y, clA.Z, 0.44f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(clA.X, clA.Y, clA.Z, 0.74f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabActive]  = new Vector4(clA.X, clA.Y, clA.Z, 1.00f);
    style.Colors[(int)ImGuiCol.CheckMark]            = new Vector4(chk.X, chk.Y, chk.Z, 0.80f);
    style.Colors[(int)ImGuiCol.SliderGrab]           = new Vector4(clA.X, clA.Y, clA.Z, 0.36f);
    style.Colors[(int)ImGuiCol.SliderGrabActive]     = new Vector4(clA.X, clA.Y, clA.Z, 0.76f);
    style.Colors[(int)ImGuiCol.Button]               = new Vector4(clC.X, clC.Y, clC.Z, 0.35f);
    style.Colors[(int)ImGuiCol.ButtonHovered]        = new Vector4(clA.X, clA.Y, clA.Z, 0.43f);
    style.Colors[(int)ImGuiCol.ButtonActive]         = new Vector4(clA.X, clA.Y, clA.Z, 0.62f);
    style.Colors[(int)ImGuiCol.Header]               = new Vector4(clA.X, clA.Y, clA.Z, 0.33f);
    style.Colors[(int)ImGuiCol.HeaderHovered]        = new Vector4(clA.X, clA.Y, clA.Z, 0.42f);
    style.Colors[(int)ImGuiCol.HeaderActive]         = new Vector4(clA.X, clA.Y, clA.Z, 0.54f);
    style.Colors[(int)ImGuiCol.Separator]            = new Vector4(clB.X, clB.Y, clB.Z, 0.50f);
    style.Colors[(int)ImGuiCol.SeparatorHovered]     = new Vector4(clB.X, clB.Y, clB.Z, 0.75f);
    style.Colors[(int)ImGuiCol.SeparatorActive]      = new Vector4(clC.X, clC.Y, clC.Z, 1.00f);
    style.Colors[(int)ImGuiCol.ResizeGrip]           = new Vector4(clA.X, clA.Y, clA.Z, 0.54f);
    style.Colors[(int)ImGuiCol.ResizeGripHovered]    = new Vector4(clA.X, clA.Y, clA.Z, 0.74f);
    style.Colors[(int)ImGuiCol.ResizeGripActive]     = new Vector4(clA.X, clA.Y, clA.Z, 1.00f);
    style.Colors[(int)ImGuiCol.PlotLines]            = new Vector4(clA.X, clA.Y, clA.Z, 1.00f);
    style.Colors[(int)ImGuiCol.PlotLinesHovered]     = new Vector4(clA.X, clA.Y, clA.Z, 1.00f);
    style.Colors[(int)ImGuiCol.PlotHistogram]        = new Vector4(chk.X, chk.Y, chk.Z, 0.40f);
    style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(chk.X, chk.Y, chk.Z, 0.50f);
    style.Colors[(int)ImGuiCol.TextSelectedBg]       = new Vector4(clA.X, clA.Y, clA.Z, 0.22f);
    style.Colors[(int)ImGuiCol.ModalWindowDimBg]     = new Vector4(clB.X, clB.Y, clB.Z, 0.40f);
}


//Light grey + Green
static void colorTheme4()
{

    var style = ImGui.GetStyle();

    style.Colors[(int)ImGuiCol.Text]                 = new Vector4(0.40f, 0.39f, 0.38f, 1.00f);
    style.Colors[(int)ImGuiCol.TextDisabled]         = new Vector4(0.40f, 0.39f, 0.38f, 0.77f);
    style.Colors[(int)ImGuiCol.WindowBg]             = new Vector4(0.92f, 0.91f, 0.88f, 0.70f);
    style.Colors[(int)ImGuiCol.ChildBg]              = new Vector4(1.00f, 0.98f, 0.95f, 0.58f);
    style.Colors[(int)ImGuiCol.PopupBg]              = new Vector4(0.92f, 0.91f, 0.88f, 0.92f);
    style.Colors[(int)ImGuiCol.Border]               = new Vector4(0.84f, 0.83f, 0.80f, 0.65f);
    style.Colors[(int)ImGuiCol.BorderShadow]         = new Vector4(0.92f, 0.91f, 0.88f, 0.00f);
    style.Colors[(int)ImGuiCol.FrameBg]              = new Vector4(1.00f, 0.98f, 0.95f, 1.00f);
    style.Colors[(int)ImGuiCol.FrameBgHovered]       = new Vector4(0.99f, 1.00f, 0.40f, 0.78f);
    style.Colors[(int)ImGuiCol.FrameBgActive]        = new Vector4(0.26f, 1.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.TitleBg]              = new Vector4(1.00f, 0.98f, 0.95f, 1.00f);
    style.Colors[(int)ImGuiCol.TitleBgCollapsed]     = new Vector4(1.00f, 0.98f, 0.95f, 0.75f);
    style.Colors[(int)ImGuiCol.TitleBgActive]        = new Vector4(0.25f, 1.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.MenuBarBg]            = new Vector4(1.00f, 0.98f, 0.95f, 0.47f);
    style.Colors[(int)ImGuiCol.ScrollbarBg]          = new Vector4(1.00f, 0.98f, 0.95f, 1.00f);
    style.Colors[(int)ImGuiCol.ScrollbarGrab]        = new Vector4(0.00f, 0.00f, 0.00f, 0.21f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.90f, 0.91f, 0.00f, 0.78f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabActive]  = new Vector4(0.25f, 1.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.CheckMark]            = new Vector4(0.25f, 1.00f, 0.00f, 0.80f);
    style.Colors[(int)ImGuiCol.SliderGrab]           = new Vector4(0.00f, 0.00f, 0.00f, 0.14f);
    style.Colors[(int)ImGuiCol.SliderGrabActive]     = new Vector4(0.25f, 1.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.Button]               = new Vector4(0.00f, 0.00f, 0.00f, 0.14f);
    style.Colors[(int)ImGuiCol.ButtonHovered]        = new Vector4(0.99f, 1.00f, 0.22f, 0.86f);
    style.Colors[(int)ImGuiCol.ButtonActive]         = new Vector4(0.25f, 1.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.Header]               = new Vector4(0.25f, 1.00f, 0.00f, 0.76f);
    style.Colors[(int)ImGuiCol.HeaderHovered]        = new Vector4(0.25f, 1.00f, 0.00f, 0.86f);
    style.Colors[(int)ImGuiCol.HeaderActive]         = new Vector4(0.25f, 1.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.Separator]            = new Vector4(0.00f, 0.00f, 0.00f, 0.32f);
    style.Colors[(int)ImGuiCol.SeparatorHovered]     = new Vector4(0.25f, 1.00f, 0.00f, 0.78f);
    style.Colors[(int)ImGuiCol.SeparatorActive]      = new Vector4(0.25f, 1.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.ResizeGrip]           = new Vector4(0.00f, 0.00f, 0.00f, 0.04f);
    style.Colors[(int)ImGuiCol.ResizeGripHovered]    = new Vector4(0.25f, 1.00f, 0.00f, 0.78f);
    style.Colors[(int)ImGuiCol.ResizeGripActive]     = new Vector4(0.25f, 1.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.PlotLines]            = new Vector4(0.40f, 0.39f, 0.38f, 0.63f);
    style.Colors[(int)ImGuiCol.PlotLinesHovered]     = new Vector4(0.25f, 1.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.PlotHistogram]        = new Vector4(0.40f, 0.39f, 0.38f, 0.63f);
    style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.25f, 1.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.TextSelectedBg]       = new Vector4(0.25f, 1.00f, 0.00f, 0.43f);
    style.Colors[(int)ImGuiCol.ModalWindowDimBg]     = new Vector4(1.00f, 0.98f, 0.95f, 0.73f);
}


//Dark + Blu
static void colorTheme5b()
{
    var  style = ImGui.GetStyle();
    //Like Solar
    //var wBG = new Vector4(0.00f, 0.045f, 0.0525f, 1.00f);
    var wBG = new Vector4(0.00f, 0.06f, 0.075f, 1.00f);
    //var wBG = new Vector4(0.00f, 0.12f, 0.15f, 1.00f);
    
    //Blu
    //var wBG = new Vector4(0.00f, 0.03f, 0.09f, 1.00f);
    //var wBG = new Vector4(0.00f, 0.045f, 0.075f, 1.00f);
    //var wBG = new Vector4(0.00f, 0.06f, 0.09f, 1.00f);
    //var wBG = new Vector4(0.07f, 0.07f, 0.00f, 1.00f);
    
    //0.00,0.09,0.16

    //var clA = new Vector4(0.17f, 0.32f, 0.35f, 1.00f); //0.00f, 0.55f, 0.87f
    
    var clA = new Vector4(wBG.X*3.5f, wBG.Y*3.5f, wBG.Z*3.5f, 1.00f); //0.00f, 0.55f, 0.87f
    var clB = new Vector4(wBG.X*2f, wBG.Y*2f, wBG.Z*2f, 1.00f); //0.20f, 0.22f, 0.27f
    var clC = new Vector4(wBG.X*5f, wBG.Y*5f, wBG.Z*5f, 1.00f); //0.20f, 0.22f, 0.27f
    
    //var txt = new Vector4(0.55f, 0.70f, 0.70f, 1.00f); //0.86f, 0.93f, 0.89f       
    var act = new Vector4(0.00f, 0.33f, 0.66f, 1.00f); //0.86f, 0.93f, 0.89f
    //var act = new Vector4(wBG.X*7f, wBG.Y*7f, wBG.Z*7f, 1.00f);

    var txt = new Vector4();
    var HSV = new Vector4();
    ImGui.ColorConvertRGBtoHSV(wBG.X, wBG.Y, wBG.Z, out HSV.X, out HSV.Y, out HSV.Z);
    if(HSV.Y>.25) HSV.Y=.25f; 
    HSV.Z=.75f;
    ImGui.ColorConvertHSVtoRGB(HSV.X, HSV.Y, HSV.Z, out txt.X, out txt.Y, out txt.Z);

    


    style.Colors[(int)ImGuiCol.Text]                 = new Vector4(txt.X, txt.Y, txt.Z, 1.00f);
    style.Colors[(int)ImGuiCol.TextDisabled]         = new Vector4(txt.X, txt.Y, txt.Z, 0.45f);
    style.Colors[(int)ImGuiCol.WindowBg]             = new Vector4(wBG.X, wBG.Y, wBG.Z, 1.00f);
    style.Colors[(int)ImGuiCol.ChildBg]              = new Vector4(clB.X, clB.Y, clB.Z, 0.30f);
    style.Colors[(int)ImGuiCol.Border]               = new Vector4(clB.X, clB.Y, clB.Z, 0.75f);
    style.Colors[(int)ImGuiCol.BorderShadow]         = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
    style.Colors[(int)ImGuiCol.FrameBg]              = new Vector4(clC.X, clC.Y, clC.Z, 0.25f);
    style.Colors[(int)ImGuiCol.FrameBgHovered]       = new Vector4(clC.X, clC.Y, clC.Z, 0.70f);
    style.Colors[(int)ImGuiCol.FrameBgActive]        = new Vector4(act.X, act.Y, act.Z, 1.00f);
    style.Colors[(int)ImGuiCol.TitleBg]              = new Vector4(clB.X, clB.Y, clB.Z, 0.70f);
    style.Colors[(int)ImGuiCol.TitleBgCollapsed]     = new Vector4(wBG.X, wBG.Y, wBG.Z, 0.50f);
    style.Colors[(int)ImGuiCol.TitleBgActive]        = new Vector4(act.X, act.Y, act.Z, 0.90f);
    style.Colors[(int)ImGuiCol.MenuBarBg]            = new Vector4(clB.X, clB.Y, clB.Z, 0.47f);
    style.Colors[(int)ImGuiCol.ScrollbarBg]          = new Vector4(clB.X, clB.Y, clB.Z, 0.25f);
    style.Colors[(int)ImGuiCol.ScrollbarGrab]        = new Vector4(clA.X, clA.Y, clA.Z, 0.66f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(clA.X, clA.Y, clA.Z, 0.85f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabActive]  = new Vector4(clA.X, clA.Y, clA.Z, 1.00f);
    style.Colors[(int)ImGuiCol.CheckMark]            = new Vector4(0.71f, 0.22f, 0.27f, 1.00f);
    style.Colors[(int)ImGuiCol.SliderGrab]           = new Vector4(clC.X, clC.Y, clC.Z, 0.55f);
    style.Colors[(int)ImGuiCol.SliderGrabActive]     = new Vector4(clC.X, clC.Y, clC.Z, 0.70f);
    style.Colors[(int)ImGuiCol.Button]               = new Vector4(clC.X, clC.Y, clC.Z, 0.40f);
    style.Colors[(int)ImGuiCol.ButtonHovered]        = new Vector4(clC.X, clC.Y, clC.Z, 0.50f);
    style.Colors[(int)ImGuiCol.ButtonActive]         = new Vector4(act.X, act.Y, act.Z, 1.00f);
    style.Colors[(int)ImGuiCol.Header]               = new Vector4(act.X, act.Y, act.Z, 0.50f);
    style.Colors[(int)ImGuiCol.HeaderHovered]        = new Vector4(act.X, act.Y, act.Z, 0.75f);
    style.Colors[(int)ImGuiCol.HeaderActive]         = new Vector4(act.X, act.Y, act.Z, 1.00f);
    style.Colors[(int)ImGuiCol.Separator]            = new Vector4(wBG.X, wBG.Y, wBG.Z, 1.00f);
    style.Colors[(int)ImGuiCol.SeparatorHovered]     = new Vector4(clA.X, clA.Y, clA.Z, 0.78f);
    style.Colors[(int)ImGuiCol.SeparatorActive]      = new Vector4(clA.X, clA.Y, clA.Z, 1.00f);
    style.Colors[(int)ImGuiCol.ResizeGrip]           = new Vector4(clB.X, clB.Y, clB.Z, 0.50f);
    style.Colors[(int)ImGuiCol.ResizeGripHovered]    = new Vector4(clA.X, clA.Y, clA.Z, 0.78f);
    style.Colors[(int)ImGuiCol.ResizeGripActive]     = new Vector4(clA.X, clA.Y, clA.Z, 1.00f);
    style.Colors[(int)ImGuiCol.PlotLines]            = new Vector4(txt.X, txt.Y, txt.Z, 0.63f);
    style.Colors[(int)ImGuiCol.PlotLinesHovered]     = new Vector4(clA.X, clA.Y, clA.Z, 1.00f);
    style.Colors[(int)ImGuiCol.PlotHistogram]        = new Vector4(txt.X, txt.Y, txt.Z, 0.63f);
    style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(clA.X, clA.Y, clA.Z, 1.00f);
    style.Colors[(int)ImGuiCol.TextSelectedBg]       = new Vector4(clA.X, clA.Y, clA.Z, 0.43f);
    style.Colors[(int)ImGuiCol.PopupBg]				= new Vector4(clB.X, clB.Y, clB.Z, 0.9f);
    style.Colors[(int)ImGuiCol.ModalWindowDimBg]     = new Vector4(clB.X, clB.Y, clB.Z, 0.73f);
}


static void darkDefaultTheme(Vector4 color, bool wantAlwaysRedButtons=false)
{
    var  style = ImGui.GetStyle();
    //Like Solar
    //var wBG = new Vector4(0.00f, 0.045f, 0.0525f, 1.00f);
    //var wBG = new Vector4(0.00f, 0.06f, 0.075f, 1.00f);
    //var wBG = new Vector4(0.00f, 0.12f, 0.15f, 1.00f);
    
    //Blu
    float kC = .2f;
    float lum = color.X*.299f + color.Y*.587f + color.Z*.114f;
    float lumK = 1 + lum * .25f;

    var wBG = new Vector4(color.X*kC, color.Y*kC, color.Z*kC, 1.0f);

    var act = new Vector4(wBG.X*6.0f*lumK, wBG.Y*6.0f*lumK, wBG.Z*6.0f*lumK, 1.00f);

    var clA = new Vector4(wBG.X*3.0f*lumK, wBG.Y*3.0f*lumK, wBG.Z*3.0f*lumK, 1.00f); //0.00f, 0.55f, 0.87f
    var clB = new Vector4(wBG.X*1.5f*lumK, wBG.Y*1.5f*lumK, wBG.Z*1.5f*lumK, 1.00f); //0.20f, 0.22f, 0.27f
    var clC = new Vector4(wBG.X*4.0f*lumK, wBG.Y*4.0f*lumK, wBG.Z*4.0f*lumK, 1.00f); //0.20f, 0.22f, 0.27f

    
    var HSV = new Vector4();

     float diff = 1f/(lumK*lumK*lumK);
     act.X *= diff;
    act.Y *= diff;
    act.Z *= diff;

    var txt = new Vector4();

    ImGui.ColorConvertRGBtoHSV(wBG.X, wBG.Y, wBG.Z, out HSV.X, out HSV.Y, out HSV.Z);
    if(HSV.Y>.25) HSV.Y=.25f; 
    HSV.Z=.85f;
     float baseH = HSV.X;
    float tmpH = HSV.X;
    
    
    ImGui.ColorConvertHSVtoRGB(HSV.X, HSV.Y, HSV.Z, out txt.X, out txt.Y, out txt.Z);

    var lin = new Vector4() ;
    ImGui.ColorConvertRGBtoHSV(1.0f, 1.0f, .75f, out HSV.X, out HSV.Y, out HSV.Z);
    HSV.X += baseH - tmpH;
    if(HSV.X>1.0)      HSV.X-=1.0f; 
    else if(HSV.X<0.0) HSV.X+=1.0f; 
    ImGui.ColorConvertHSVtoRGB(HSV.X, HSV.Y, HSV.Z, out lin.X, out lin.Y, out lin.Z);

    var chk = new Vector4() ;
    if(wantAlwaysRedButtons) {
        chk = new Vector4(0.60f, 0.00f, 0.00f, 1.0f);
    } else {
        ImGui.ColorConvertRGBtoHSV(0.60f, 0.00f, 0.00f, out HSV.X, out HSV.Y, out HSV.Z);
        HSV.X += baseH - tmpH;
        if(HSV.X>1.0)      HSV.X-=1.0f; 
        else if(HSV.X<0.0) HSV.X+=1.0f; 
        ImGui.ColorConvertHSVtoRGB(HSV.X, HSV.Y, HSV.Z, out chk.X, out chk.Y, out chk.Z);
    }

    chk.X *= lumK;
    chk.Y *= lumK;
    chk.Z *= lumK;

    float l1 =  .60f;
    float l2 =  .80f;
    var ch1 = new Vector4 (chk.X*l1, chk.Y*l1, chk.Z*l1, 1.00f);
    var ch2 = new Vector4 (chk.X*l2, chk.Y*l2, chk.Z*l2, 1.00f);


    style.Colors[(int)ImGuiCol.Text]                 = new Vector4(txt.X, txt.Y, txt.Z, 1.00f);
    style.Colors[(int)ImGuiCol.TextDisabled]         = new Vector4(txt.X, txt.Y, txt.Z,  0.45f);
    style.Colors[(int)ImGuiCol.WindowBg]             = new Vector4(wBG.X, wBG.Y, wBG.Z, 1.00f);
    style.Colors[(int)ImGuiCol.ChildBg]              = new Vector4(clB.X, clB.Y, clB.Z, 0.30f);
    style.Colors[(int)ImGuiCol.PopupBg]              = new Vector4(wBG.X, wBG.Y, wBG.Z, 1.00f);
    style.Colors[(int)ImGuiCol.Border]               = new Vector4(clC.X, clC.Y, clC.Z, 1.00f);
    style.Colors[(int)ImGuiCol.BorderShadow]         = new Vector4(clC.X, clC.Y, clC.Z, 0.66f);
    style.Colors[(int)ImGuiCol.FrameBg]              = new Vector4(clA.X, clA.Y, clA.Z, 0.60f);
    style.Colors[(int)ImGuiCol.FrameBgHovered]       = new Vector4(act.X, act.Y, act.Z, 0.75f);
    style.Colors[(int)ImGuiCol.FrameBgActive]        = new Vector4(act.X, act.Y, act.Z, 1.00f);
    style.Colors[(int)ImGuiCol.TitleBg]              = new Vector4(clB.X, clB.Y, clB.Z, 0.85f);
    style.Colors[(int)ImGuiCol.TitleBgCollapsed]     = new Vector4(wBG.X, wBG.Y, wBG.Z, 0.66f);
    style.Colors[(int)ImGuiCol.TitleBgActive]        = new Vector4(act.X, act.Y, act.Z, 0.85f);
    style.Colors[(int)ImGuiCol.MenuBarBg]            = new Vector4(clB.X, clB.Y, clB.Z, 0.47f);
    style.Colors[(int)ImGuiCol.ScrollbarBg]          = new Vector4(clB.X, clB.Y, clB.Z, 0.25f);
    style.Colors[(int)ImGuiCol.ScrollbarGrab]        = new Vector4(clA.X, clA.Y, clA.Z, 0.50f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = ch1;
    style.Colors[(int)ImGuiCol.ScrollbarGrabActive]  = ch2;
    style.Colors[(int)ImGuiCol.CheckMark]            = new Vector4(chk.X, chk.Y, chk.Z, 1.00f);
    style.Colors[(int)ImGuiCol.SliderGrab]           = ch1;
    style.Colors[(int)ImGuiCol.SliderGrabActive]     = ch2;
    style.Colors[(int)ImGuiCol.Button]               = new Vector4(clC.X, clC.Y, clC.Z, 0.40f);
    style.Colors[(int)ImGuiCol.ButtonHovered]        = new Vector4(act.X, act.Y, act.Z, 0.50f);
    style.Colors[(int)ImGuiCol.ButtonActive]         = new Vector4(act.X, act.Y, act.Z, 1.00f);
    style.Colors[(int)ImGuiCol.Header]               = new Vector4(act.X, act.Y, act.Z, 0.50f);
    style.Colors[(int)ImGuiCol.HeaderHovered]        = new Vector4(act.X, act.Y, act.Z, 0.75f);
    style.Colors[(int)ImGuiCol.HeaderActive]         = new Vector4(act.X, act.Y, act.Z, 1.00f);
    style.Colors[(int)ImGuiCol.Separator]            = new Vector4(wBG.X, wBG.Y, wBG.Z, 1.00f);
    style.Colors[(int)ImGuiCol.SeparatorHovered]     = new Vector4(clA.X, clA.Y, clA.Z, 0.78f);
    style.Colors[(int)ImGuiCol.SeparatorActive]      = new Vector4(clA.X, clA.Y, clA.Z, 1.00f);
    style.Colors[(int)ImGuiCol.ResizeGrip]           = new Vector4(clA.X, clA.Y, clA.Z, 0.75f);
    style.Colors[(int)ImGuiCol.ResizeGripHovered]    = ch1;
    style.Colors[(int)ImGuiCol.ResizeGripActive]     = ch2;
    style.Colors[(int)ImGuiCol.PlotLines]            = new Vector4(lin.X, lin.Y, lin.Z, 0.75f);
    style.Colors[(int)ImGuiCol.PlotLinesHovered]     = new Vector4(lin.X, lin.Y, lin.Z, 1.00f);
    style.Colors[(int)ImGuiCol.PlotHistogram]        = ch1;
    style.Colors[(int)ImGuiCol.PlotHistogramHovered] = ch2;
    style.Colors[(int)ImGuiCol.TextSelectedBg]       = new Vector4(clA.X, clA.Y, clA.Z, 0.43f);
    style.Colors[(int)ImGuiCol.PopupBg]				= new Vector4(clB.X, clB.Y, clB.Z, 0.9f);
    style.Colors[(int)ImGuiCol.ModalWindowDimBg]     = new Vector4(clB.X, clB.Y, clB.Z, 0.73f);
}

//Dark + Blu
static void darkDefaultGrayButt(Vector4 color)
{
    var  style = ImGui.GetStyle();


    float kC = .2f;
    float lum = color.X*.299f + color.Y*.587f + color.Z*.114f;
    float lumK = 1 + lum * .25F;

    var wBG = new Vector4(color.X*kC, color.Y*kC, color.Z*kC, 1.0f);

    //var clA = new Vector4(0.00f, 0.31f, 0.40f, 1.00f);
    //var clB = new Vector4(0.00f, 0.21f, 0.27f, 1.00f);

    var clA = new Vector4(wBG.X*3.0f*lumK, wBG.Y*3.0f*lumK, wBG.Z*3.0f*lumK, 1.00f); //0.00f, 0.55f, 0.87f
    var clB = new Vector4(wBG.X*1.5f*lumK, wBG.Y*1.5f*lumK, wBG.Z*1.5f*lumK, 1.00f); //0.20f, 0.22f, 0.27f
    var clC = new Vector4(wBG.X*4.0f*lumK, wBG.Y*4.0f*lumK, wBG.Z*4.0f*lumK, 1.00f); //0.20f, 0.22f, 0.27f


    var txt = new Vector4(0.90f, 0.90f, 0.90f, 1.00f); //0.86f, 0.93f, 0.89f    

    //var act = new Vector4(0.00f, 0.25f, 0.57f, 1.00f); //0.86f, 0.93f, 0.89f
    var act = new Vector4(wBG.X*3.75f, wBG.Y*3.75f, wBG.Z*3.75f, 1.00f);
    float diff = 1f/(lumK*lumK);
    act.X *= diff;
    act.Y *= diff;
    act.Z *= diff;

    var lin = new Vector4 (1.0f, 1.0f, 1.0f, 1.0f);
    var chk = new Vector4 (0.60f*lumK, 0.60f*lumK, 0.60f*lumK, 1.0f);

    var ch1 = new Vector4 (chk.X*.60f, chk.Y*.60f, chk.Z*.60f, 1.00f);
    var ch2 = new Vector4 (chk.X*.80f, chk.Y*.80f, chk.Z*.80f, 1.00f);



    //new Vector4 chk(0.00f, 0.25f, 0.57f, 1.00f);
    
    style.Colors[(int)ImGuiCol.Text]                 = new Vector4(txt.X, txt.Y, txt.Z, 1.00f);
    style.Colors[(int)ImGuiCol.TextDisabled]         = new Vector4(txt.X, txt.Y, txt.Z, 0.45f);
    style.Colors[(int)ImGuiCol.WindowBg]             = new Vector4(wBG.X, wBG.Y, wBG.Z, 1.00f);
    style.Colors[(int)ImGuiCol.ChildBg]              = new Vector4(clB.X, clB.Y, clB.Z, 0.30f);
    style.Colors[(int)ImGuiCol.PopupBg]              = new Vector4(wBG.X, wBG.Y, wBG.Z, 1.00f);
    style.Colors[(int)ImGuiCol.Border]               = new Vector4(clA.X, clA.Y, clA.Z, 0.66f);
    style.Colors[(int)ImGuiCol.BorderShadow]         = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
    style.Colors[(int)ImGuiCol.FrameBg]              = new Vector4(clA.X, clA.Y, clA.Z, 0.60f);
    style.Colors[(int)ImGuiCol.FrameBgHovered]       = new Vector4(act.X, act.Y, act.Z, 0.75f);
    style.Colors[(int)ImGuiCol.FrameBgActive]        = new Vector4(act.X, act.Y, act.Z, 1.00f);
    style.Colors[(int)ImGuiCol.TitleBg]              = new Vector4(clB.X, clB.Y, clB.Z, 0.85f);
    style.Colors[(int)ImGuiCol.TitleBgCollapsed]     = new Vector4(wBG.X, wBG.Y, wBG.Z, 0.66f);
    style.Colors[(int)ImGuiCol.TitleBgActive]        = new Vector4(act.X, act.Y, act.Z, 0.85f);
    style.Colors[(int)ImGuiCol.MenuBarBg]            = new Vector4(clB.X, clB.Y, clB.Z, 0.47f);
    style.Colors[(int)ImGuiCol.ScrollbarBg]          = new Vector4(clB.X, clB.Y, clB.Z, 0.25f);
    style.Colors[(int)ImGuiCol.ScrollbarGrab]        = new Vector4(clA.X, clA.Y, clA.Z, 0.50f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = ch1;
    style.Colors[(int)ImGuiCol.ScrollbarGrabActive]  = ch2;
    style.Colors[(int)ImGuiCol.CheckMark]            = new Vector4(chk.X, chk.Y, chk.Z, 1.00f);
    style.Colors[(int)ImGuiCol.SliderGrab]           = ch1;
    style.Colors[(int)ImGuiCol.SliderGrabActive]     = ch2;
    style.Colors[(int)ImGuiCol.Button]               = new Vector4(clC.X, clC.Y, clC.Z, 0.40f);
    style.Colors[(int)ImGuiCol.ButtonHovered]        = new Vector4(act.X, act.Y, act.Z, 0.50f);
    style.Colors[(int)ImGuiCol.ButtonActive]         = new Vector4(act.X, act.Y, act.Z, 1.00f);
    style.Colors[(int)ImGuiCol.Header]               = new Vector4(act.X, act.Y, act.Z, 0.50f);
    style.Colors[(int)ImGuiCol.HeaderHovered]        = new Vector4(act.X, act.Y, act.Z, 0.75f);
    style.Colors[(int)ImGuiCol.HeaderActive]         = new Vector4(act.X, act.Y, act.Z, 1.00f);
    style.Colors[(int)ImGuiCol.Separator]            = new Vector4(wBG.X, wBG.Y, wBG.Z, 1.00f);
    style.Colors[(int)ImGuiCol.SeparatorHovered]     = new Vector4(clA.X, clA.Y, clA.Z, 0.78f);
    style.Colors[(int)ImGuiCol.SeparatorActive]      = new Vector4(clA.X, clA.Y, clA.Z, 1.00f);
    style.Colors[(int)ImGuiCol.ResizeGrip]           = new Vector4(clA.X, clA.Y, clA.Z, 0.75f);
    style.Colors[(int)ImGuiCol.ResizeGripHovered]    = ch1;
    style.Colors[(int)ImGuiCol.ResizeGripActive]     = ch2;
    style.Colors[(int)ImGuiCol.PlotLines]            = new Vector4(lin.X, lin.Y, lin.Z, 0.75f);
    style.Colors[(int)ImGuiCol.PlotLinesHovered]     = new Vector4(lin.X, lin.Y, lin.Z, 1.00f);
    style.Colors[(int)ImGuiCol.PlotHistogram]        = ch1;
    style.Colors[(int)ImGuiCol.PlotHistogramHovered] = ch2;
    style.Colors[(int)ImGuiCol.TextSelectedBg]       = new Vector4(clA.X, clA.Y, clA.Z, 0.43f);
    style.Colors[(int)ImGuiCol.PopupBg]				= new Vector4(clB.X, clB.Y, clB.Z, 0.9f);
    style.Colors[(int)ImGuiCol.ModalWindowDimBg]     = new Vector4(clB.X, clB.Y, clB.Z, 0.73f);

}

static void darkSolarized()
{
    var  style = ImGui.GetStyle();

    var wBG = new Vector4(0.00f, 0.10f, 0.125f, 1.00f);

    //var clA = new Vector4(0.00f, 0.31f, 0.40f, 1.00f);
    //var clB = new Vector4(0.00f, 0.21f, 0.27f, 1.00f);

    var clA = new Vector4(wBG.X*2.50f, wBG.Y*2.50f, wBG.Z*2.50f, 1.00f);
    var clB = new Vector4(wBG.X*1.75f, wBG.Y*1.75f, wBG.Z*1.75f, 1.00f);


    var txt = new Vector4(0.66f, 0.81f, 0.78f, 1.00f); //0.86f, 0.93f, 0.89f    
//    var act = new Vector4(0.00f, 0.25f, 0.57f, 1.00f); //0.86f, 0.93f, 0.89f
    var act = new Vector4(wBG.X*3.75f, wBG.Y*3.75f, wBG.Z*3.75f, 1.00f);


    //new Vector4 chk(0.00f, 0.25f, 0.57f, 1.00f);
    

    style.Colors[(int)ImGuiCol.Text]                 = new Vector4(txt.X, txt.Y, txt.Z, 1.00f);
    style.Colors[(int)ImGuiCol.TextDisabled]         = new Vector4(txt.X, txt.Y, txt.Z, 0.28f);
    style.Colors[(int)ImGuiCol.WindowBg]             = new Vector4(wBG.X, wBG.Y, wBG.Z, 1.00f);
    style.Colors[(int)ImGuiCol.ChildBg]              = new Vector4(clB.X, clB.Y, clB.Z, 0.40f);
    style.Colors[(int)ImGuiCol.PopupBg]              = new Vector4(wBG.X, wBG.Y, wBG.Z, 1.00f);
    style.Colors[(int)ImGuiCol.Border]               = new Vector4(0.31f, 0.31f, 1.00f, 0.00f);
    style.Colors[(int)ImGuiCol.BorderShadow]         = new Vector4(0.99f, 0.00f, 0.00f, 0.00f);
    style.Colors[(int)ImGuiCol.FrameBg]              = new Vector4(clB.X, clB.Y, clB.Z, 1.00f);
    style.Colors[(int)ImGuiCol.FrameBgHovered]       = new Vector4(clA.X, clA.Y, clA.Z, 0.78f);
    style.Colors[(int)ImGuiCol.FrameBgActive]        = new Vector4(txt.X, txt.Y, txt.Z, 0.60f);
    style.Colors[(int)ImGuiCol.TitleBg]              = new Vector4(wBG.X, wBG.Y, wBG.Z, 1.00f);
    style.Colors[(int)ImGuiCol.TitleBgCollapsed]     = new Vector4(wBG.X, wBG.Y, wBG.Z, 0.60f);
    style.Colors[(int)ImGuiCol.TitleBgActive]        = new Vector4(act.X, act.Y, act.Z, 1.00f);
    style.Colors[(int)ImGuiCol.MenuBarBg]            = new Vector4(clB.X, clB.Y, clB.Z, 0.47f);
    style.Colors[(int)ImGuiCol.ScrollbarBg]          = new Vector4(wBG.X, wBG.Y, wBG.Z, 1.00f);
    style.Colors[(int)ImGuiCol.ScrollbarGrab]        = new Vector4(clB.X, clB.Y, clB.Z, 1.00f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(clA.X, clA.Y, clA.Z, 0.78f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabActive]  = new Vector4(clA.X, clA.Y, clA.Z, 1.00f);
    style.Colors[(int)ImGuiCol.CheckMark]            = new Vector4(txt.X, txt.Y, txt.Z, 0.65f);
    style.Colors[(int)ImGuiCol.SliderGrab]           = new Vector4(0.47f, 0.77f, 0.83f, 0.14f);
    style.Colors[(int)ImGuiCol.SliderGrabActive]     = new Vector4(clA.X, clA.Y, clA.Z, 1.00f);
    style.Colors[(int)ImGuiCol.Button]               = new Vector4(0.47f, 0.77f, 0.83f, 0.14f);
    style.Colors[(int)ImGuiCol.ButtonHovered]        = new Vector4(clA.X, clA.Y, clA.Z, 0.86f);
    style.Colors[(int)ImGuiCol.ButtonActive]         = new Vector4(txt.X, txt.Y, txt.Z, 0.65f);
    style.Colors[(int)ImGuiCol.Header]               = new Vector4(clA.X, clA.Y, clA.Z, 0.76f);
    style.Colors[(int)ImGuiCol.HeaderHovered]        = new Vector4(clA.X, clA.Y, clA.Z, 0.86f);
    style.Colors[(int)ImGuiCol.HeaderActive]         = new Vector4(act.X, act.Y, act.Z, 1.00f);
    style.Colors[(int)ImGuiCol.Separator]            = new Vector4(wBG.X, wBG.Y, wBG.Z, 1.00f);
    style.Colors[(int)ImGuiCol.SeparatorHovered]     = new Vector4(clA.X, clA.Y, clA.Z, 0.78f);
    style.Colors[(int)ImGuiCol.SeparatorActive]      = new Vector4(clA.X, clA.Y, clA.Z, 1.00f);
    style.Colors[(int)ImGuiCol.ResizeGrip]           = new Vector4(0.47f, 0.77f, 0.83f, 0.04f);
    style.Colors[(int)ImGuiCol.ResizeGripHovered]    = new Vector4(clA.X, clA.Y, clA.Z, 0.78f);
    style.Colors[(int)ImGuiCol.ResizeGripActive]     = new Vector4(clA.X, clA.Y, clA.Z, 1.00f);
    style.Colors[(int)ImGuiCol.PlotLines]            = new Vector4(1.00f, 1.00f, 0.75f, 0.80f);
    style.Colors[(int)ImGuiCol.PlotLinesHovered]     = new Vector4(clA.X, clA.Y, clA.Z, 1.00f);
    style.Colors[(int)ImGuiCol.PlotHistogram]        = new Vector4(txt.X, txt.Y, txt.Z, 0.40f);
    style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(txt.X, txt.Y, txt.Z, 0.50f);
    style.Colors[(int)ImGuiCol.TextSelectedBg]       = new Vector4(clA.X, clA.Y, clA.Z, 0.43f);
    style.Colors[(int)ImGuiCol.PopupBg]				= new Vector4(clB.X, clB.Y, clB.Z, 0.9f);
    style.Colors[(int)ImGuiCol.ModalWindowDimBg]     = new Vector4(clB.X, clB.Y, clB.Z, 0.73f);
}

//Dark + red
static void colorTheme6()
{
    var  style = ImGui.GetStyle();

    style.Colors[(int)ImGuiCol.Text]                 = new Vector4(0.86f, 0.93f, 0.89f, 0.78f);
    style.Colors[(int)ImGuiCol.TextDisabled]         = new Vector4(0.86f, 0.93f, 0.89f, 0.28f);
    style.Colors[(int)ImGuiCol.WindowBg]             = new Vector4(0.13f, 0.14f, 0.17f, 1.00f);
    style.Colors[(int)ImGuiCol.ChildBg]              = new Vector4(0.20f, 0.22f, 0.27f, 0.58f);
    style.Colors[(int)ImGuiCol.Border]               = new Vector4(0.31f, 0.31f, 1.00f, 0.00f);
    style.Colors[(int)ImGuiCol.BorderShadow]         = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
    style.Colors[(int)ImGuiCol.FrameBg]              = new Vector4(0.20f, 0.22f, 0.27f, 1.00f);
    style.Colors[(int)ImGuiCol.FrameBgHovered]       = new Vector4(0.10f, 0.67f, 1.00f, 0.78f);
    style.Colors[(int)ImGuiCol.FrameBgActive]        = new Vector4(0.10f, 0.67f, 1.00f, 1.00f);
    style.Colors[(int)ImGuiCol.TitleBg]              = new Vector4(0.20f, 0.22f, 0.27f, 1.00f);
    style.Colors[(int)ImGuiCol.TitleBgCollapsed]     = new Vector4(0.20f, 0.22f, 0.27f, 0.75f);
    style.Colors[(int)ImGuiCol.TitleBgActive]        = new Vector4(0.10f, 0.67f, 1.00f, 1.00f);
    style.Colors[(int)ImGuiCol.MenuBarBg]            = new Vector4(0.20f, 0.22f, 0.27f, 0.47f);
    style.Colors[(int)ImGuiCol.ScrollbarBg]          = new Vector4(0.20f, 0.22f, 0.27f, 1.00f);
    style.Colors[(int)ImGuiCol.ScrollbarGrab]        = new Vector4(0.09f, 0.15f, 0.16f, 1.00f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.10f, 0.67f, 1.00f, 0.78f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabActive]  = new Vector4(0.10f, 0.67f, 1.00f, 1.00f);
    style.Colors[(int)ImGuiCol.CheckMark]            = new Vector4(0.71f, 0.22f, 0.27f, 1.00f);
    style.Colors[(int)ImGuiCol.SliderGrab]           = new Vector4(0.47f, 0.77f, 0.83f, 0.14f);
    style.Colors[(int)ImGuiCol.SliderGrabActive]     = new Vector4(0.10f, 0.67f, 1.00f, 1.00f);
    style.Colors[(int)ImGuiCol.Button]               = new Vector4(0.47f, 0.77f, 0.83f, 0.14f);
    style.Colors[(int)ImGuiCol.ButtonHovered]        = new Vector4(0.10f, 0.67f, 1.00f, 0.86f);
    style.Colors[(int)ImGuiCol.ButtonActive]         = new Vector4(0.10f, 0.67f, 1.00f, 1.00f);
    style.Colors[(int)ImGuiCol.Header]               = new Vector4(0.10f, 0.67f, 1.00f, 0.76f);
    style.Colors[(int)ImGuiCol.HeaderHovered]        = new Vector4(0.10f, 0.67f, 1.00f, 0.86f);
    style.Colors[(int)ImGuiCol.HeaderActive]         = new Vector4(0.10f, 0.67f, 1.00f, 1.00f);
    style.Colors[(int)ImGuiCol.Separator]            = new Vector4(0.14f, 0.16f, 0.19f, 1.00f);
    style.Colors[(int)ImGuiCol.SeparatorHovered]     = new Vector4(0.10f, 0.67f, 1.00f, 0.78f);
    style.Colors[(int)ImGuiCol.SeparatorActive]      = new Vector4(0.10f, 0.67f, 1.00f, 1.00f);
    style.Colors[(int)ImGuiCol.ResizeGrip]           = new Vector4(0.47f, 0.77f, 0.83f, 0.04f);
    style.Colors[(int)ImGuiCol.ResizeGripHovered]    = new Vector4(0.10f, 0.67f, 1.00f, 0.78f);
    style.Colors[(int)ImGuiCol.ResizeGripActive]     = new Vector4(0.10f, 0.67f, 1.00f, 1.00f);
    style.Colors[(int)ImGuiCol.PlotLines]            = new Vector4(0.86f, 0.93f, 0.89f, 0.63f);
    style.Colors[(int)ImGuiCol.PlotLinesHovered]     = new Vector4(0.10f, 0.67f, 1.00f, 1.00f);
    style.Colors[(int)ImGuiCol.PlotHistogram]        = new Vector4(0.86f, 0.93f, 0.89f, 0.63f);
    style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.10f, 0.67f, 1.00f, 1.00f);
    style.Colors[(int)ImGuiCol.TextSelectedBg]       = new Vector4(0.10f, 0.67f, 1.00f, 0.43f);
    style.Colors[(int)ImGuiCol.PopupBg]				= new Vector4(0.20f, 0.22f, 0.27f, 0.9f);
    style.Colors[(int)ImGuiCol.ModalWindowDimBg]     = new Vector4(0.20f, 0.22f, 0.27f, 0.73f);
}

static void darkCyanYellow()
{
    var  style = ImGui.GetStyle();

    style.Colors[(int)ImGuiCol.Text]                 = new Vector4(0.90f, 0.90f, 0.90f, 0.90f);
    style.Colors[(int)ImGuiCol.TextDisabled]         = new Vector4(0.60f, 0.60f, 0.60f, 1.00f);
    style.Colors[(int)ImGuiCol.WindowBg]             = new Vector4(0.09f, 0.09f, 0.15f, 1.00f);
    style.Colors[(int)ImGuiCol.ChildBg]              = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
    style.Colors[(int)ImGuiCol.PopupBg]              = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.Border]               = new Vector4(0.70f, 0.70f, 0.70f, 0.65f);
    style.Colors[(int)ImGuiCol.BorderShadow]         = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
    style.Colors[(int)ImGuiCol.FrameBg]              = new Vector4(0.00f, 0.00f, 0.01f, 1.00f);
    style.Colors[(int)ImGuiCol.FrameBgHovered]       = new Vector4(0.60f, 0.60f, 0.60f, 0.15f);
    style.Colors[(int)ImGuiCol.FrameBgActive]        = new Vector4(0.60f, 0.60f, 0.60f, 0.30f);
    style.Colors[(int)ImGuiCol.TitleBg]              = new Vector4(0.00f, 0.00f, 0.00f, 0.83f);
    style.Colors[(int)ImGuiCol.TitleBgCollapsed]     = new Vector4(0.40f, 0.40f, 0.80f, 0.20f);
    style.Colors[(int)ImGuiCol.TitleBgActive]        = new Vector4(0.00f, 0.00f, 0.00f, 0.87f);
    style.Colors[(int)ImGuiCol.MenuBarBg]            = new Vector4(0.01f, 0.01f, 0.02f, 0.80f);
    style.Colors[(int)ImGuiCol.ScrollbarBg]          = new Vector4(0.20f, 0.25f, 0.30f, 0.60f);
    style.Colors[(int)ImGuiCol.ScrollbarGrab]        = new Vector4(0.55f, 0.53f, 0.55f, 0.51f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.56f, 0.56f, 0.56f, 1.00f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabActive]  = new Vector4(0.56f, 0.56f, 0.56f, 0.91f);
    style.Colors[(int)ImGuiCol.CheckMark]            = new Vector4(0.90f, 0.90f, 0.90f, 0.83f);
    style.Colors[(int)ImGuiCol.SliderGrab]           = new Vector4(0.70f, 0.70f, 0.70f, 0.62f);
    style.Colors[(int)ImGuiCol.SliderGrabActive]     = new Vector4(0.30f, 0.30f, 0.30f, 0.84f);
    style.Colors[(int)ImGuiCol.Button]               = new Vector4(0.48f, 0.72f, 0.89f, 0.49f);
    style.Colors[(int)ImGuiCol.ButtonHovered]        = new Vector4(0.50f, 0.69f, 0.99f, 0.68f);
    style.Colors[(int)ImGuiCol.ButtonActive]         = new Vector4(0.80f, 0.50f, 0.50f, 1.00f);
    style.Colors[(int)ImGuiCol.Header]               = new Vector4(0.30f, 0.69f, 1.00f, 0.53f);
    style.Colors[(int)ImGuiCol.HeaderHovered]        = new Vector4(0.44f, 0.61f, 0.86f, 1.00f);
    style.Colors[(int)ImGuiCol.HeaderActive]         = new Vector4(0.38f, 0.62f, 0.83f, 1.00f);
    style.Colors[(int)ImGuiCol.Separator]            = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
    style.Colors[(int)ImGuiCol.SeparatorHovered]     = new Vector4(0.70f, 0.60f, 0.60f, 1.00f);
    style.Colors[(int)ImGuiCol.SeparatorActive]      = new Vector4(0.90f, 0.70f, 0.70f, 1.00f);
    style.Colors[(int)ImGuiCol.ResizeGrip]           = new Vector4(1.00f, 1.00f, 1.00f, 0.85f);
    style.Colors[(int)ImGuiCol.ResizeGripHovered]    = new Vector4(1.00f, 1.00f, 1.00f, 0.60f);
    style.Colors[(int)ImGuiCol.ResizeGripActive]     = new Vector4(1.00f, 1.00f, 1.00f, 0.90f);
    style.Colors[(int)ImGuiCol.PlotLines]            = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
    style.Colors[(int)ImGuiCol.PlotLinesHovered]     = new Vector4(0.90f, 0.70f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.PlotHistogram]        = new Vector4(0.70f, 0.50f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.90f, 0.50f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.TextSelectedBg]       = new Vector4(0.00f, 0.00f, 1.00f, 0.35f);
    style.Colors[(int)ImGuiCol.ModalWindowDimBg]     = new Vector4(0.20f, 0.20f, 0.20f, 0.35f);
}

static void darkBlackGrayRed()
{
    var  style = ImGui.GetStyle();

    var chk = new Vector4 (.6f, 0.0f, 0.0f, 1.00f);
    var ch1 = new Vector4 (.4f, 0.0f, 0.0f, 1.00f);
    var ch2 = new Vector4 (.5f, 0.0f, 0.0f, 1.00f);
    var but = new Vector4 (0.40f, 0.50f, 0.75f, 1.00f);


    style.Colors[(int)ImGuiCol.Text]                 = new Vector4(0.90f, 0.90f, 0.90f, 0.90f);
    style.Colors[(int)ImGuiCol.TextDisabled]         = new Vector4(0.60f, 0.60f, 0.60f, 1.00f);
    style.Colors[(int)ImGuiCol.WindowBg]             = new Vector4(0.09f, 0.09f, 0.15f, 1.00f);
    style.Colors[(int)ImGuiCol.ChildBg]              = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
    style.Colors[(int)ImGuiCol.PopupBg]              = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.Border]               = new Vector4(0.70f, 0.70f, 0.70f, 0.65f);
    style.Colors[(int)ImGuiCol.BorderShadow]         = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
    style.Colors[(int)ImGuiCol.FrameBg]              = new Vector4(0.00f, 0.00f, 0.01f, 1.00f);
    style.Colors[(int)ImGuiCol.FrameBgHovered]       = new Vector4(0.70f, 0.70f, 0.70f, 0.20f);
    style.Colors[(int)ImGuiCol.FrameBgActive]        = new Vector4(0.90f, 0.00f, 0.00f, 0.75f);
    style.Colors[(int)ImGuiCol.TitleBg]              = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.TitleBgCollapsed]     = new Vector4(but.X, but.Y, but.Z, 0.35f);
    style.Colors[(int)ImGuiCol.TitleBgActive]        = new Vector4(0.30f, 0.30f, 0.37f, 0.70f);
    style.Colors[(int)ImGuiCol.MenuBarBg]            = new Vector4(0.01f, 0.01f, 0.02f, 0.80f);
    style.Colors[(int)ImGuiCol.ScrollbarBg]          = new Vector4(0.20f, 0.25f, 0.30f, 0.60f);
    style.Colors[(int)ImGuiCol.ScrollbarGrab]        = new Vector4(0.55f, 0.53f, 0.55f, 0.51f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = ch1;
    style.Colors[(int)ImGuiCol.ScrollbarGrabActive]  = ch2;
    style.Colors[(int)ImGuiCol.CheckMark]            = new Vector4(chk.X, chk.Y, chk.Z, 1.00f);
    style.Colors[(int)ImGuiCol.SliderGrab]           = ch1;
    style.Colors[(int)ImGuiCol.SliderGrabActive]     = ch2;
    style.Colors[(int)ImGuiCol.Button]               = new Vector4(0.50f, 0.50f, 0.50f, 0.50f);
    style.Colors[(int)ImGuiCol.ButtonHovered]        = new Vector4(but.X, but.Y, but.Z, 1.00f);
    style.Colors[(int)ImGuiCol.ButtonActive]         = new Vector4(0.60f, 0.00f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.Header]               = new Vector4(but.X, but.Y, but.Z, 0.30f);
    style.Colors[(int)ImGuiCol.HeaderHovered]        = new Vector4(but.X, but.Y, but.Z, 0.75f);
    style.Colors[(int)ImGuiCol.HeaderActive]         = new Vector4(but.X, but.Y, but.Z, 0.50f);
    style.Colors[(int)ImGuiCol.Separator]            = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
    style.Colors[(int)ImGuiCol.SeparatorHovered]     = new Vector4(0.70f, 0.60f, 0.60f, 1.00f);
    style.Colors[(int)ImGuiCol.SeparatorActive]      = new Vector4(0.90f, 0.70f, 0.70f, 1.00f);
    style.Colors[(int)ImGuiCol.ResizeGrip]           = new Vector4(0.50f, 0.50f, 0.50f, 0.50f);
    style.Colors[(int)ImGuiCol.ResizeGripHovered]    = ch1;
    style.Colors[(int)ImGuiCol.ResizeGripActive]     = ch2;
    style.Colors[(int)ImGuiCol.PlotLines]            = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
    style.Colors[(int)ImGuiCol.PlotLinesHovered]     = new Vector4(0.90f, 0.70f, 0.00f, 1.00f);
    style.Colors[(int)ImGuiCol.PlotHistogram]        = ch1;
    style.Colors[(int)ImGuiCol.PlotHistogramHovered] = ch2;
    style.Colors[(int)ImGuiCol.TextSelectedBg]       = new Vector4(0.00f, 0.00f, 1.00f, 0.35f);
    style.Colors[(int)ImGuiCol.ModalWindowDimBg]     = new Vector4(0.20f, 0.20f, 0.20f, 0.35f);
}

static void lightGrayScale()
{
    var  style = ImGui.GetStyle();

    style.Colors[(int)ImGuiCol.Text]                 = new Vector4(0.00f, 0.00f, 0.00f, 0.85f);
    style.Colors[(int)ImGuiCol.TextDisabled]         = new Vector4(0.60f, 0.60f, 0.60f, 1.00f);
    style.Colors[(int)ImGuiCol.WindowBg]             = new Vector4(0.94f, 0.94f, 0.94f, 1.00f);
    style.Colors[(int)ImGuiCol.ChildBg]              = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
    style.Colors[(int)ImGuiCol.PopupBg]              = new Vector4(1.00f, 1.00f, 1.00f, 0.98f);
    style.Colors[(int)ImGuiCol.Border]               = new Vector4(0.00f, 0.00f, 0.00f, 0.44f);
    style.Colors[(int)ImGuiCol.BorderShadow]         = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
    style.Colors[(int)ImGuiCol.FrameBg]              = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
    style.Colors[(int)ImGuiCol.FrameBgHovered]       = new Vector4(0.64f, 0.65f, 0.66f, 0.40f);
    style.Colors[(int)ImGuiCol.FrameBgActive]        = new Vector4(0.64f, 0.65f, 0.66f, 0.40f);
    style.Colors[(int)ImGuiCol.TitleBg]              = new Vector4(0.82f, 0.82f, 0.82f, 1.00f);
    style.Colors[(int)ImGuiCol.TitleBgActive]        = new Vector4(0.71f, 0.70f, 0.70f, 1.00f);
    style.Colors[(int)ImGuiCol.TitleBgCollapsed]     = new Vector4(1.00f, 1.00f, 1.00f, 0.51f);
    style.Colors[(int)ImGuiCol.MenuBarBg]            = new Vector4(0.86f, 0.86f, 0.86f, 1.00f);
    style.Colors[(int)ImGuiCol.ScrollbarBg]          = new Vector4(0.98f, 0.98f, 0.98f, 0.53f);
    style.Colors[(int)ImGuiCol.ScrollbarGrab]        = new Vector4(0.69f, 0.69f, 0.69f, 0.80f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.49f, 0.49f, 0.49f, 0.80f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabActive]  = new Vector4(0.49f, 0.49f, 0.49f, 1.00f);
    style.Colors[(int)ImGuiCol.CheckMark]            = new Vector4(0.43f, 0.43f, 0.43f, 1.00f);
    style.Colors[(int)ImGuiCol.SliderGrab]           = new Vector4(0.63f, 0.63f, 0.63f, 0.78f);
    style.Colors[(int)ImGuiCol.SliderGrabActive]     = new Vector4(0.43f, 0.44f, 0.46f, 0.78f);
    style.Colors[(int)ImGuiCol.Button]               = new Vector4(0.61f, 0.61f, 0.62f, 0.40f);
    style.Colors[(int)ImGuiCol.ButtonHovered]        = new Vector4(0.57f, 0.57f, 0.57f, 0.52f);
    style.Colors[(int)ImGuiCol.ButtonActive]         = new Vector4(0.61f, 0.63f, 0.64f, 1.00f);
    style.Colors[(int)ImGuiCol.Header]               = new Vector4(0.64f, 0.64f, 0.65f, 0.31f);
    style.Colors[(int)ImGuiCol.HeaderHovered]        = new Vector4(0.58f, 0.58f, 0.59f, 0.55f);
    style.Colors[(int)ImGuiCol.HeaderActive]         = new Vector4(0.52f, 0.52f, 0.52f, 0.55f);
    style.Colors[(int)ImGuiCol.Separator]            = new Vector4(0.56f, 0.56f, 0.56f, 1.00f);
    style.Colors[(int)ImGuiCol.SeparatorHovered]     = new Vector4(0.17f, 0.17f, 0.17f, 0.89f);
    style.Colors[(int)ImGuiCol.SeparatorActive]      = new Vector4(0.17f, 0.17f, 0.17f, 0.89f);
    style.Colors[(int)ImGuiCol.ResizeGrip]           = new Vector4(0.80f, 0.80f, 0.80f, 0.56f);
    style.Colors[(int)ImGuiCol.ResizeGripHovered]    = new Vector4(0.39f, 0.39f, 0.40f, 0.67f);
    style.Colors[(int)ImGuiCol.ResizeGripActive]     = new Vector4(0.39f, 0.39f, 0.40f, 0.67f);
    style.Colors[(int)ImGuiCol.PlotLines]            = new Vector4(0.39f, 0.39f, 0.39f, 1.00f);
    style.Colors[(int)ImGuiCol.PlotLinesHovered]     = new Vector4(1.00f, 0.43f, 0.35f, 1.00f);
    style.Colors[(int)ImGuiCol.PlotHistogram]        = new Vector4(0.78f, 0.78f, 0.78f, 1.00f);
    style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.56f, 0.56f, 0.56f, 1.00f);
    style.Colors[(int)ImGuiCol.TextSelectedBg]       = new Vector4(0.71f, 0.72f, 0.73f, 0.57f);
    style.Colors[(int)ImGuiCol.ModalWindowDimBg]     = new Vector4(0.20f, 0.20f, 0.20f, 0.35f);
    style.Colors[(int)ImGuiCol.DragDropTarget]       = new Vector4(0.16f, 0.16f, 0.17f, 0.95f);
}

static void chatgpt_dark()
{

    stylegpt2();
}

private static void chatgpt_light()
{
    var style = ImGui.GetStyle();

    style.Colors[(int)ImGuiCol.Text] = new Vector4(0.2f, 0.2f, 0.2f, 1f);
    style.Colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.6f, 0.6f, 0.6f, 1f);
    style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.9f, 0.9f, 0.9f, 1f);
    style.Colors[(int)ImGuiCol.ChildBg] = new Vector4(0.95f, 0.95f, 0.95f, 1f);
    style.Colors[(int)ImGuiCol.PopupBg] = new Vector4(0.95f, 0.95f, 0.95f, 1f);
    style.Colors[(int)ImGuiCol.Border] = new Vector4(0.6f, 0.6f, 0.6f, 1f);
    style.Colors[(int)ImGuiCol.BorderShadow] = new Vector4(0f, 0f, 0f, 0f);
    style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(0.85f, 0.85f, 0.85f, 1f);
    style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.75f, 0.75f, 0.75f, 1f);
    style.Colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.95f, 0.95f, 0.95f, 1f);
    style.Colors[(int)ImGuiCol.TitleBg] = new Vector4(0.8f, 0.8f, 0.8f, 1f);
    style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.8f, 0.8f, 0.8f, 1f);
    style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.9f, 0.9f, 0.9f, 1f);
    style.Colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.85f, 0.85f, 0.85f, 1f);
    style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.95f, 0.95f, 0.95f, 1f);
    style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.7f, 0.7f, 0.7f, 1f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.5f, 0.5f, 0.5f, 1f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.3f, 0.3f, 0.3f, 1f);
    style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(0.2f, 0.2f, 0.2f, 1f);
    style.Colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.2f, 0.2f, 0.2f, 1f);
    style.Colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.3f, 0.3f, 0.3f, 1f);
    style.Colors[(int)ImGuiCol.Button] = new Vector4(0.85f, 0.85f, 0.85f, 1f);
    style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.75f, 0.75f, 0.75f, 1f);
    style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.95f, 0.95f, 0.95f, 1f);
    style.Colors[(int)ImGuiCol.Header] = new Vector4(0.85f, 0.85f, 0.85f, 1f);
    style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.75f, 0.75f, 0.75f, 1f);
    style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.95f, 0.95f, 0.95f, 1f);
    style.Colors[(int)ImGuiCol.Separator] = new Vector4(0.6f, 0.6f, 0.6f, 1f);
    style.Colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.5f, 0.5f, 0.5f, 1f);
    style.Colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.3f, 0.3f, 0.3f, 1f);
    style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.85f, 0.85f, 0.85f, 1f);
    style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.5f, 0.5f, 0.5f, 1f);
    style.Colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.3f, 0.3f, 0.3f, 1f);
    style.Colors[(int)ImGuiCol.PlotLines] = new Vector4(0.2f, 0.2f, 0.2f, 1f);
    style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(0.3f, 0.3f, 0.3f, 1f);
    style.Colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.2f, 0.2f, 0.2f, 1f);
    style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.3f, 0.3f, 0.3f, 1f);
    style.Colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.55f, 0.55f, 0.55f, 1f);
    style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.5f, 0.5f, 0.5f, 0.7f);
    style.Colors[(int)ImGuiCol.Tab] = new Vector4(0.85f, 0.85f, 0.85f, 1f);
    style.Colors[(int)ImGuiCol.TabHovered] = new Vector4(0.95f, 0.95f, 0.95f, 1f);
    style.Colors[(int)ImGuiCol.TabActive] = new Vector4(0.95f, 0.95f, 0.95f, 1f);
    style.Colors[(int)ImGuiCol.TabUnfocused] = new Vector4(0.85f, 0.85f, 0.85f, 1f);
    style.Colors[(int)ImGuiCol.TabUnfocusedActive] = new Vector4(0.95f, 0.95f, 0.95f, 1f);
}


static void stylegpt2()
{
    var  style = ImGui.GetStyle();
    style.Colors[(int)ImGuiCol.Text] = new Vector4(0.95f, 0.95f, 0.95f, 1f);
style.Colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.6f, 0.6f, 0.6f, 1f);
style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.2f, 0.2f, 0.2f, 1f);
style.Colors[(int)ImGuiCol.ChildBg] = new Vector4(0.25f, 0.25f, 0.25f, 1f);
style.Colors[(int)ImGuiCol.PopupBg] = new Vector4(0.3f, 0.3f, 0.3f, 1f);
style.Colors[(int)ImGuiCol.Border] = new Vector4(0.6f, 0.6f, 0.6f, 1f);
style.Colors[(int)ImGuiCol.BorderShadow] = new Vector4(0f, 0f, 0f, 0f);
style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(0.35f, 0.35f, 0.35f, 1f);
style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.4f, 0.4f, 0.4f, 1f);
style.Colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.6f, 0.6f, 0.6f, 1f);
style.Colors[(int)ImGuiCol.TitleBg] = new Vector4(0.15f, 0.15f, 0.15f, 1f);
style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.15f, 0.15f, 0.15f, 1f);
style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.15f, 0.15f, 0.15f, 1f);
style.Colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.25f, 0.25f, 0.25f, 1f);
style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.3f, 0.3f, 0.3f, 1f);
style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.6f, 0.6f, 0.6f, 1f);
style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.8f, 0.8f, 0.8f, 1f);
style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.4f, 0.4f, 0.4f, 1f);
style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(0.8f, 0.8f, 0.8f, 1f);
style.Colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.8f, 0.8f, 0.8f, 1f);
style.Colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.4f, 0.4f, 0.4f, 1f);
style.Colors[(int)ImGuiCol.Button] = new Vector4(0.35f, 0.35f, 0.35f, 1f);
style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.4f, 0.4f, 0.4f, 1f);
style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.6f, 0.6f, 0.6f, 1f);
style.Colors[(int)ImGuiCol.Header] = new Vector4(0.35f, 0.35f, 0.35f, 1f);
style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.4f, 0.4f, 0.4f, 1f);
style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.6f, 0.6f, 0.6f, 1f);
style.Colors[(int)ImGuiCol.Separator] = new Vector4(0.6f, 0.6f, 0.6f, 1f);
style.Colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.8f, 0.8f, 0.8f, 1f);
style.Colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.4f, 0.4f, 0.4f, 1f);
style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.35f, 0.35f, 0.35f, 1f);
style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.8f, 0.8f, 0.8f, 1f);
style.Colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.4f, 0.4f, 0.4f, 1f);
style.Colors[(int)ImGuiCol.PlotLines] = new Vector4(0.8f, 0.8f, 0.8f, 1f);
style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(0.4f, 0.4f, 0.4f, 1f);
style.Colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.8f, 0.8f, 0.8f, 1f);
style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.4f, 0.4f, 0.4f, 1f);
style.Colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.4f, 0.4f, 0.4f, 1f);
style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.5f, 0.5f, 0.5f, 0.7f);
style.Colors[(int)ImGuiCol.Tab] = new Vector4(0.4f, 0.4f, 0.4f, 1f);
style.Colors[(int)ImGuiCol.TabHovered] = new Vector4(0.55f, 0.55f, 0.55f, 1f);
style.Colors[(int)ImGuiCol.TabActive] = new Vector4(0.7f, 0.7f, 0.7f, 1f);
style.Colors[(int)ImGuiCol.TabUnfocused] = new Vector4(0.4f, 0.4f, 0.4f, 1f);
style.Colors[(int)ImGuiCol.TabUnfocusedActive] = new Vector4(0.55f, 0.55f, 0.55f, 1f);

}

static void stylegpt1()
{
    var  style = ImGui.GetStyle();
    style.Colors[(int)ImGuiCol.Text] = new Vector4(0.8f, 0.8f, 0.8f, 1f);
    style.Colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.53f, 0.53f, 0.53f, 1f);
    style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.96f, 0.96f, 0.96f, 1f);
    style.Colors[(int)ImGuiCol.ChildBg] = new Vector4(0.87f, 0.87f, 0.87f, 1f);
    style.Colors[(int)ImGuiCol.PopupBg] = new Vector4(0.13f, 0.13f, 0.13f, 1f);
    style.Colors[(int)ImGuiCol.Border] = new Vector4(0.87f, 0.87f, 0.87f, 1f);
    style.Colors[(int)ImGuiCol.BorderShadow] = new Vector4(0f, 0f, 0f, 0f);
    style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(0.87f, 0.87f, 0.87f, 1f);
    style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.7f, 0.92f, 0.95f, 1f);
    style.Colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0f, 1f, 1f, 1f);
    style.Colors[(int)ImGuiCol.TitleBg] = new Vector4(0.87f, 0.87f, 0.87f, 1f);
    style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.87f, 0.87f, 0.87f, 1f);
    style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.87f, 0.87f, 0.87f, 1f);
    style.Colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.87f, 0.87f, 0.87f, 1f);
    style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.96f, 0.96f, 0.96f, 1f);
    style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.87f, 0.87f, 0.87f, 1f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.53f, 0.53f, 0.53f, 1f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.33f, 0.33f, 0.33f, 1f);
    style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(0f, 1f, 1f, 1f);
    style.Colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.87f, 0.87f, 0.87f, 1f);
    style.Colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0f, 1f, 1f, 1f);
    style.Colors[(int)ImGuiCol.Button] = new Vector4(0.87f, 0.87f, 0.87f, 1f);
    style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.7f, 0.92f, 0.95f, 1f);
    style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(0f, 1f, 1f, 1f);
    style.Colors[(int)ImGuiCol.Header] = new Vector4(0.87f, 0.87f, 0.87f, 1f);
    style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.7f, 0.92f, 0.95f, 1f);
    style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(0f, 1f, 1f, 1f);
    style.Colors[(int)ImGuiCol.Separator] = new Vector4(0.87f, 0.87f, 0.87f, 1f);
    style.Colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.7f, 0.92f, 0.95f, 1f);
    style.Colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0f, 1f, 1f, 1f);
    style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.87f, 0.87f, 0.87f, 1f);
    style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.53f, 0.53f, 0.53f, 1f);
    style.Colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.33f, 0.33f, 0.33f, 1f);
    style.Colors[(int)ImGuiCol.PlotLines] = new Vector4(0.87f, 0.87f, 0.87f, 1f);
    style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(0f, 1f, 1f, 1f);
    style.Colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.87f, 0.87f, 0.87f, 1f);
    style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0f, 1f, 1f, 1f);
    style.Colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0f, 1f, 1f, 1f);
    style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0f, 0f, 0f, 0.7f);
}

static void imgui_easy_theming(Vector3 color_for_text, Vector3 color_for_head, Vector3 color_for_area, Vector3 color_for_body, Vector3 color_for_pops)
{
    var  style = ImGui.GetStyle();
    style.Colors[(int)ImGuiCol.Text]                 = new Vector4(color_for_text.X, color_for_text.Y, color_for_text.Z, 1.00f);
    style.Colors[(int)ImGuiCol.TextDisabled]         = new Vector4(color_for_text.X, color_for_text.Y, color_for_text.Z, 0.58f);
    style.Colors[(int)ImGuiCol.WindowBg]             = new Vector4(color_for_body.X, color_for_body.Y, color_for_body.Z, 1.00f);
    style.Colors[(int)ImGuiCol.ChildBg]              = new Vector4(color_for_area.X, color_for_area.Y, color_for_area.Z, 0.58f);
    style.Colors[(int)ImGuiCol.PopupBg]              = new Vector4(color_for_area.X, color_for_area.Y, color_for_area.Z, 1.00f);
    style.Colors[(int)ImGuiCol.PopupBg]              = new Vector4(color_for_pops.X, color_for_pops.Y, color_for_pops.Z, 0.92f);
    style.Colors[(int)ImGuiCol.Border]               = new Vector4(color_for_body.X, color_for_body.Y, color_for_body.Z, 0.00f);
    style.Colors[(int)ImGuiCol.BorderShadow]         = new Vector4(color_for_body.X, color_for_body.Y, color_for_body.Z, 0.00f);
    style.Colors[(int)ImGuiCol.FrameBg]              = new Vector4(color_for_area.X, color_for_area.Y, color_for_area.Z, 1.00f);
    style.Colors[(int)ImGuiCol.FrameBgHovered]       = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.78f);
    style.Colors[(int)ImGuiCol.FrameBgActive]        = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
    style.Colors[(int)ImGuiCol.TitleBg]              = new Vector4(color_for_area.X, color_for_area.Y, color_for_area.Z, 1.00f);
    style.Colors[(int)ImGuiCol.TitleBgCollapsed]     = new Vector4(color_for_area.X, color_for_area.Y, color_for_area.Z, 0.75f);
    style.Colors[(int)ImGuiCol.TitleBgActive]        = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
    style.Colors[(int)ImGuiCol.MenuBarBg]            = new Vector4(color_for_area.X, color_for_area.Y, color_for_area.Z, 1.00f);
    style.Colors[(int)ImGuiCol.ScrollbarBg]          = new Vector4(color_for_area.X, color_for_area.Y, color_for_area.Z, 1.00f);
    style.Colors[(int)ImGuiCol.ScrollbarGrab]        = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.50f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.78f);
    style.Colors[(int)ImGuiCol.ScrollbarGrabActive]  = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
    style.Colors[(int)ImGuiCol.CheckMark]            = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
    style.Colors[(int)ImGuiCol.SliderGrab]           = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.50f);
    style.Colors[(int)ImGuiCol.SliderGrabActive]     = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
    style.Colors[(int)ImGuiCol.Button]               = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.50f);
    style.Colors[(int)ImGuiCol.ButtonHovered]        = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.86f);
    style.Colors[(int)ImGuiCol.ButtonActive]         = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
    style.Colors[(int)ImGuiCol.Header]               = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.76f);
    style.Colors[(int)ImGuiCol.HeaderHovered]        = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.86f);
    style.Colors[(int)ImGuiCol.HeaderActive]         = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
    style.Colors[(int)ImGuiCol.Separator]            = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.32f);
    style.Colors[(int)ImGuiCol.SeparatorHovered]     = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.78f);
    style.Colors[(int)ImGuiCol.SeparatorActive]      = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
    style.Colors[(int)ImGuiCol.ResizeGrip]           = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.15f);
    style.Colors[(int)ImGuiCol.ResizeGripHovered]    = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.78f);
    style.Colors[(int)ImGuiCol.ResizeGripActive]     = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
    style.Colors[(int)ImGuiCol.PlotLines]            = new Vector4(color_for_text.X, color_for_text.Y, color_for_text.Z, 0.63f);
    style.Colors[(int)ImGuiCol.PlotLinesHovered]     = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 1.00f);
    style.Colors[(int)ImGuiCol.PlotHistogram]        = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.50f);
    style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.75f);
    style.Colors[(int)ImGuiCol.TextSelectedBg]       = new Vector4(color_for_head.X, color_for_head.Y, color_for_head.Z, 0.43f);
    style.Colors[(int)ImGuiCol.ModalWindowDimBg]     = new Vector4(color_for_area.X, color_for_area.Y, color_for_area.Z, 0.73f);
}

static void SetupImGuiStyle2()
{

    Vector3 color_for_text = new Vector3(255f / 255f, 255f / 255f, 255f / 255f);
    Vector3 color_for_head = new Vector3(41f / 255f, 128f / 255f, 185f / 255f);
    Vector3 color_for_area = new Vector3(57f / 255f, 79f / 255f, 105f / 255f);
    Vector3 color_for_body = new Vector3(44f / 255f, 62f / 255f, 80f / 255f);
    Vector3 color_for_pops = new Vector3(33f / 255f, 46f / 255f, 60f / 255f);
    
    /*
    Original post
    static ImVec3 color_for_text = ImVec3( .80f,  .80f,  .80f);
    static ImVec3 color_for_head = ImVec3(0.10f, 0.67f, 1.00f);
    static ImVec3 color_for_area = ImVec3(0.20f, 0.22f, 0.27f);
    static ImVec3 color_for_body = ImVec3(0.13f, 0.14f, 0.17f);
    //static ImVec3 color_for_pops = ImVec3(33f / 255f, 46f / 255f, 60f / 255f);
  */
    ImGui.StyleColorsDark();
    imgui_easy_theming(color_for_text, color_for_head, color_for_area, color_for_body, color_for_pops);
}


public static void SetGUIStyle()
{
    var  style = ImGui.GetStyle();


    style.ItemSpacing = new(2,3); 
    style.ItemInnerSpacing = new(2,3); 

    style.FrameRounding = 1.2f;
    style.WindowRounding = 1.5f;
    style.TabRounding = 1.5f;
    style.ChildRounding = 1.5f;
    style.GrabRounding = 1;
    style.ScrollbarRounding = 2;
    
    style.WindowPadding = new(3,3);
    
    style.ScrollbarSize = 11;
    style.FramePadding = new(3,3);

    
    style.AntiAliasedLines = true;
    style.AntiAliasedFill = true;

    // style.ScaleAllSizes(1.3f);

    var io = ImGui.GetIO();
    io.FontGlobalScale = 1f;
    
    chatgpt_dark();
}

//
//
// // Demo helper function to select among default colors. See ShowStyleEditor() for more advanced options.
// // Here we use the simplified Combo() api that packs items into a single literal string. Useful for quick combo boxes where the choices are known locally.
//
//
    public static void selectTheme(int style_idx)
    {

        switch (style_idx)
        {
            // case  0: darkDefaultTheme(theDlg.getGuiThemeBaseColor()); break;
            // case  1: darkDefaultTheme(theDlg.getGuiThemeBaseColor(), true); break;
            // case  2: darkDefaultGrayButt(theDlg.getGuiThemeBaseColor()); break;
            // case  3: darkColorsOnBlack(theDlg.getGuiThemeBaseColor()); break;
            case  4: darkSolarized(); break;
                  
            case  5: SetupImGuiStyle2(); break;
            case  6: colorTheme6(); break;
                  
            case  7: darkCyanYellow(); break;
            case  8: darkBlackGrayRed(); break;
            case  9: lightCyanYellow(); break;
            case 10: lightGrayScale(); break;

            case 11: ImGui.StyleColorsClassic(); break;
            case 12: ImGui.StyleColorsDark(); break;
            case 13: ImGui.StyleColorsLight(); break;
            case 14: chatgpt_dark(); break;
            case 15: chatgpt_light(); break;
            default: SetupImGuiStyle2();
                break;
        }
    }

   
    //
//
// int ShowStyleSelector(float wB)
// {
//      char* label = "##cmbThem";
//     int style_idx = theDlg.getSelectedGuiTheme();
//
//     float wButt = style_idx < 3f
//         ? wB - ImGui.GetFrameHeightWithSpacing()
//         : wB - ImGui.GetFrameHeightWithSpacing() * 2;
//
//     ImGui.PushItemWidth(wButt);
//     int index = 0;
//
//     {
//         selectTheme(style_idx);
//         theDlg.setSelectedGuiTheme(style_idx);
//     }
//     ImGui.PopItemWidth();
//
//     ImGui.SameLine();
//
//     {
//         ImGuiStyle & style = ImGui.GetStyle();
//
//         style.Colors[(int)ImGuiCol.WindowBg].w = theDlg.getGuiThemeBaseColor().w;
//         style.Alpha = theDlg.getGuiThemeBaseColor().w < .2 ? .2 : theDlg.getGuiThemeBaseColor().w;
//     }
//     ;
//
//     if (style_idx < 3f)
//     {
//         //Show Color picker
//         new Vector4 col = theDlg.getGuiThemeBaseColor();
//         if (ImGui.ColorEdit4("##baseCol", (float*)&col, ImGuiColorEditFlags_NoInputs))
//         {
//             theDlg.setGuiThemeBaseColor(col);
//             selectTheme(style_idx);
//
//             setAlpha();// /
//
//         }
//     }
//     else
//     {
//         //Show only alpha
//         ImGui.PushItemWidth(ImGui.GetFrameHeightWithSpacing() * 2);
//         float f = theDlg.getGuiThemeBaseColor().w;
//         if (ImGui.DragFloat("##alphThem", &f, 0.01f, 0f, 1.0f, "%.2f"))
//         {
//             theDlg.getGuiThemeBaseColor().w = f;
//             setAlpha();
//         }
//
//         ImGui.PopItemWidth();
//     }
//
//
//     return style_idx;
//
// }
}       