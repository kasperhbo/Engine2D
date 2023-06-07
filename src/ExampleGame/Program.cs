using Engine2D.Core;

public class Program
{
    static void Main()
    {
        //Utils.CreateEntry(ProjectSettings.s_FullProjectPath + "\\Registers\\CustomComponentRegister.cs", "//LAST LINE 01", "//NEW CREATED LINE");

        Settings.s_IsEngine = true;        
        Engine.Get().Run();
        
    }
}