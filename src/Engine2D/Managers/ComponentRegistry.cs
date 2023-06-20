using System.Reflection;
using Engine2D.Components;
using Engine2D.Logging;

namespace Engine2D.Managers;

public static class ComponentRegistry
{
                        //Path //Component
    private static Dictionary<string, Component> _componentsInProject = new();

    static ComponentRegistry()
    {
        LoadProject();
    }

    static void LoadProject()
    {
        var DLL = Assembly.LoadFile(@"D:\dev\Engine2D\src\ExampleGame\bin\Debug\net7.0\ExampleGame.dll");

        var c = Activator.CreateInstance(DLL.ExportedTypes.ElementAt(0));
        
        Log.Error(c.GetType().ToString());        
    }
}