using Engine2D.Components;
using Engine2D.Core;
using ExampleGame.Registers;
using KDBEngine.Core;

public class Program
{
    static void Main()
    {
        //Utils.CreateEntry(ProjectSettings.s_FullProjectPath + "\\Registers\\CustomComponentRegister.cs", "//LAST LINE 01", "//NEW CREATED LINE");
        
        ComponentSerializer.AddAction(() => { CustomDeserializer.Deserialize(); });
        CustomComponentRegister.StartRegister();

        Console.WriteLine(Utils.GetAllScriptFiles());
        Settings.s_IsEngine = true;        
        Engine.Get().Run();
    }
}