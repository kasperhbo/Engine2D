using ImGuiNET;
using KDBEngine.Core;
using System.Numerics;
using ImTool;
using WindowState = OpenTK.Windowing.Common.WindowState;


namespace Engine2D.UI;

public class TopMenu
{
    private int windowBtnWidth = 30;
    private int borderThickness = 0;
    private int currentMonitor = -1;
    private int settingsWidth = 340;
    private Vector2 windowButtonSize = new(30,26);
    
    private Rect windowBounds;
    private Rect titlebarBounds;
    
    private uint[] windowBorderColor;
    private uint titlebarColor;

    private Theme _theme = ThemeManager.ImGuiDark;
    
    public TopMenu()
    {
        windowBounds = new Rect();
        
        titlebarBounds = new Rect();
        
        
        
        windowBorderColor = new uint[4];

        titlebarColor = new uint();
        
        OnThemeChange();
    }
    
    private void OnThemeChange()
    {
        UpdateBorderColor();
        byte[] btbc = NormalizedVector4ToBytes(_theme.TitlebarBackgroundColor);
        titlebarColor = BitConverter.ToUInt32(btbc);
    }
    
    private void UpdateBorderColor()
    {
        
        byte[] begin = NormalizedVector4ToBytes(_theme.WindowBorderGradientBegin);
        byte[] end = NormalizedVector4ToBytes(_theme.WindowBorderGradientEnd);
        byte[] middle = new byte[]
        {
            (byte)((begin[0] + end[0]) / 2),
            (byte)((begin[1] + end[1]) / 2),
            (byte)((begin[2] + end[2]) / 2),
            (byte)((begin[3] + end[3]) / 2)
        };

        windowBorderColor[0] = BitConverter.ToUInt32(begin);
        windowBorderColor[1] = BitConverter.ToUInt32(middle);
        windowBorderColor[2] = BitConverter.ToUInt32(end);
        windowBorderColor[3] = BitConverter.ToUInt32(middle);
    }
    
    private static byte[] NormalizedVector4ToBytes(Vector4 v)
    {
        return new byte[]
        {
            (byte)(v.X * byte.MaxValue),
            (byte)(v.Y * byte.MaxValue),
            (byte)(v.Z * byte.MaxValue),
            (byte)(v.W * byte.MaxValue),
        };
    }
    
    public void OnGui() {
        
        ImGui.SetNextWindowSize(new(Engine.Get().Size.X, Engine.Get().Size.Y));
        // ImGui.SetNextWindowPos(Engine.Get().window);
        // MainWindowStyleOverrides(true);
        ImGui.Begin("MainWindow",
        ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoNavFocus
                                                                 | ImGuiWindowFlags.NoFocusOnAppearing 
                                                                 | ImGuiWindowFlags.NoBringToFrontOnFocus 
                                                                          | ImGuiWindowFlags.NoDocking);
        // MainWindowStyleOverrides(false);

        if (borderThickness > 0)
        {
            ImGui.GetWindowDrawList().AddRectFilledMultiColor(
                windowBounds.Position, windowBounds.MaxPosition, 
                windowBorderColor[0], windowBorderColor[1], windowBorderColor[2], windowBorderColor[3]);
        }

        // if (!TitlebarDisabled)
        // {
            ImGui.GetWindowDrawList().AddRectFilled(titlebarBounds.Position, titlebarBounds.MaxPosition, titlebarColor);
            SubmitWindowButtons();
        //}


        int tabHeight = 4 + (int)ImGui.CalcTextSize("ABCD").Y;
        // int yStart = TitlebarDisabled ? 0 : titlebarHeight - tabHeight;
        //     
        // ImGui.SetCursorPos(new Vector2(borderThickness + 1, yStart));
        // ImGui.BeginTabBar("Tabs");
        //     
        // ImGui.GetWindowDrawList().AddRectFilled(contentBounds.Position, contentBounds.MaxPosition, ImGui.GetColorU32(ImGuiCol.WindowBg));

        // SubmitTabs();

        ImGui.EndTabBar();
        ImGui.End();

        
    }


    private void SubmitWindowButtons()
    {
        
        
        int nBtn = 1;
        
        //Close Resize Buttons Etc
        ImGui.SetCursorPos(WindowButtonPosition(nBtn));
        
        if (ImGui.Button("\uf410", windowButtonSize))
        {
            Engine.Get().Close();
        }
        
        nBtn++;
        
        ImGui.SetCursorPos(WindowButtonPosition(nBtn));
        if (ImGui.Button(Engine.Get().WindowState == WindowState.Maximized ? "\uf2d2" : "\uf2d0", windowButtonSize))
        {
            // if (WindowState == WindowState.Maximized)
            // {
            //     WindowState = config.PreviousWindowState;
            // }
            // else
            // {
            //     WindowState = WindowState.Maximized;
            // }
        }
        nBtn++;
    
    }
    
    private Vector2 WindowButtonPosition(int n)
    {
        if (Engine.Get().WindowState == WindowState.Fullscreen || Engine.Get().WindowState == WindowState.Maximized)
        {
            return new Vector2((Engine.Get().Size.X- (windowBtnWidth + 1) * n), borderThickness + 1);
        }
        else
        {
            return new Vector2((Engine.Get().Size.X - (windowBtnWidth + 1) * n) - 1, borderThickness + 1);
        }        
    }
}