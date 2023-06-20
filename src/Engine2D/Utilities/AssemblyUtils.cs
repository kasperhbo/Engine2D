using System.Reflection;
using System.Runtime.CompilerServices;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;

namespace Engine2D.Utilities;

public static class AssemblyUtils
{
    private static bool _enableDebug = false;

    private static string _loadedAssemblyOrigin = "";
    
    private static Assembly _loadedGameAssembly = null;
    private static List<Type> _loadedComponents = new();

    public static void LoadAssembly(string assemblyPath)
    {
        _loadedAssemblyOrigin = assemblyPath;
        
        CopyAssembly();

        var data = File.ReadAllBytes(ProjectSettings.FullProjectPath + "\\project.DLL");
        
        _loadedComponents = new();
        _loadedGameAssembly = Assembly.Load(data);

        GetAssemblyComponents();
    }

    private static void CopyAssembly()
    {
        File.Copy(_loadedAssemblyOrigin, ProjectSettings.FullProjectPath + "\\project.DLL", true);
    }

    private static void GetAssemblyComponents()
    {
        if (_loadedGameAssembly == null)
        {
            Log.Error("No Assembly Is Loaded!");
            return;
        }

        var types = _loadedGameAssembly.GetTypes();
        foreach (var type in types)
        {
            try
            {
                Type theType = _loadedGameAssembly.GetType(type.ToString());
                var obj = Activator.CreateInstance(theType);
                var comp = (Component)obj;

                if (comp != null)
                    _loadedComponents.Add(theType);
            }
            catch
            {
                if (_enableDebug)
                    Log.Warning(type.FullName + " is not an component, don't add it in to the project");
            }
        }
    }

    public static Component? GetComponent(string type)
    {
        for (int i = 0; i < _loadedComponents.Count; i++)
        {
            Type t = _loadedComponents[i];

            if (type == t.FullName)
            {
                if (_enableDebug)
                    Log.Succes("Found: " + type);
                return (Component)Activator.CreateInstance(t);
            }
        }

        Log.Error(type + " not found. Is the Assembly loaded?");
        return null;
    }

    private static Dictionary<Gameobject, List<string>> _toReAddAfterReloadingAssembly = new();

    public static void Reload()
    {
        LoadAssembly(_loadedAssemblyOrigin);
        
        foreach (var go in Engine.Get().CurrentScene.GameObjects)
        {
            List<Component> toAdd = new();
            
            for (int i = 0; i < go.components.Count; i++)
            {
                toAdd.Add(go.components[i]);
            }

            //FIRST REMOVE ALL COMPONENTS
            go.components = new();
            
            //THEN READD
            for (int i = 0; i < toAdd.Count; i++)
            {
                if (GetComponent(toAdd[i].GetType().ToString()) != null)
                {
                    go.AddComponent(GetComponent(toAdd[i].GetType().ToString()));
                }
                else
                {
                    go.AddComponent(toAdd[i]);
                }
            }
        }
    }
}

///EXAMPLE

// var DLL = Assembly.LoadFile(@"D:\dev\Engine2D\src\ExampleGame\bin\Debug\net7.0\ExampleGame.dll");
// var theType = DLL.GetType("ExampleGame.TestClass");
// var c = Activator.CreateInstance(theType);
// var t = (Component)c;
// Console.WriteLine(t.GetFieldSize());
