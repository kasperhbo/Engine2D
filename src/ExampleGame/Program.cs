using Engine2D.Components;
using Engine2D.Core;
using ExampleGame.Registers;
using KDBEngine.Core;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

public class Program
{
    static void Main()
    {
        //Utils.CreateEntry(ProjectSettings.s_FullProjectPath + "\\Registers\\CustomComponentRegister.cs", "//LAST LINE 01", "//NEW CREATED LINE");
        
        ComponentSerializer.AddAction(() => { CustomDeserializer.Deserialize(); });
        CustomComponentRegister.StartRegister();

        Console.WriteLine(Utils.GetAllScriptFiles());
        
        Settings.s_IsEngine = true;
        GameWindowSettings gws = new GameWindowSettings();
        NativeWindowSettings nws = new NativeWindowSettings();

        nws.Vsync = VSyncMode.On;
        
        Engine.Get(gws, nws).Run();
    }
}