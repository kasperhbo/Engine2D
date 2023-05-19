using Engine2D.Components;
using Engine2D.Core;
using ExampleGame.Registers;
using KDBEngine.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

public class Program
{
    static void Main()
    {
        //Utils.CreateEntry(ProjectSettings.s_FullProjectPath + "\\Registers\\CustomComponentRegister.cs", "//LAST LINE 01", "//NEW CREATED LINE");

        NativeWindowSettings st = new NativeWindowSettings();
        st = NativeWindowSettings.Default;
        
        st.Size = new Vector2i(1080, 720);
        TempEngine temp = new TempEngine(GameWindowSettings.Default, st);
temp.Run();
        // ComponentSerializer.AddAction(() => { CustomDeserializer.Deserialize(); });
        // CustomComponentRegister.StartRegister();
        //
        // Console.WriteLine(Utils.GetAllScriptFiles());
        // Settings.s_IsEngine = false;        
        // Engine.Get().Run();
    }
}