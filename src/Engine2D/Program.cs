//using ImGuiNET;
using KDBEngine.Core;
using OpenTK.Core;

public class Program { 
    public static void Main()
    {
        Console.WriteLine("Creating Window");
        
        Window window = new(
            OpenTK.Windowing.Desktop.GameWindowSettings.Default,
            OpenTK.Windowing.Desktop.NativeWindowSettings.Default);

        window.Run();
    }
}