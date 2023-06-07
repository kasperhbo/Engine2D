using Engine2D.Components;
using Engine2D.Core;
using ExampleGame.Registers;
using OpenTK.Windowing.Desktop;

public class Program
{
    static void Main()
    {
        //Utils.CreateEntry(ProjectSettings.s_FullProjectPath + "\\Registers\\CustomComponentRegister.cs", "//LAST LINE 01", "//NEW CREATED LINE");
        
        ComponentSerializer.AddAction(() => { CustomDeserializer.Deserialize(); });
        CustomComponentRegister.StartRegister();
        
        Settings.s_IsEngine = true;        
        Engine.Get().Run();
        
    }
}