#region

using System.Reflection;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.UI.Debug;

#endregion

namespace Engine2D.Utilities;

internal static class AssemblyUtils
{
    private static readonly bool _enableDebug = false;

    private static string _loadedAssemblyOrigin = "";

    private static Assembly _loadedGameAssembly;
    private static List<Type> _loadedComponents = new();

    /// <summary>
    ///     Loads an assembly from a path
    /// </summary>
    /// <param name="assemblyPath"></param>
    internal static void LoadAssembly(string assemblyPath)
    {
        DebugStats.AssemblyReloaded++;
        _loadedAssemblyOrigin = assemblyPath;

        CopyAssembly();

        var data = File.ReadAllBytes(ProjectSettings.FullProjectPath + "\\project.DLL");

        _loadedComponents = new List<Type>();
        _loadedGameAssembly = Assembly.Load(data);

        GetAssemblyComponents();
    }

    /// <summary>
    ///     Copies the assembly to the project folder
    /// </summary>
    private static void CopyAssembly()
    {
        File.Copy(_loadedAssemblyOrigin, ProjectSettings.FullProjectPath+ "\\project.DLL", true);
    }

    /// <summary>
    ///     Gets all components from the loaded assembly
    /// </summary>
    private static void GetAssemblyComponents()
    {
        if (_loadedGameAssembly == null)
        {
            Log.Error("No Assembly Is Loaded!");
            return;
        }
        
        var types = _loadedGameAssembly.GetTypes();
        
        foreach (var type in types)
            try
            {
                var theType = _loadedGameAssembly.GetType(type.ToString());
                var obj = Activator.CreateInstance(theType);
                var comp = (Component)obj;
        
                if (comp != null)
                {
                    _loadedComponents.Add(theType);
                }
            }
            catch
            {
                if (_enableDebug)
                    Log.Warning(type.FullName + " is not an component, don't add it in to the project");
            }
    }

    /// <summary>
    ///     Gets a component from the loaded assembly
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    internal static Component? GetComponent(string type)
    {
        for (var i = 0; i < _loadedComponents.Count; i++)
        {
            var t = _loadedComponents[i];

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

    /// <summary>
    ///     Reloads the assembly and readds all components to the gameobjects
    /// </summary>
    internal static void Reload()
    {
        LoadAssembly(_loadedAssemblyOrigin);
        Engine.Get().CurrentScene?.ReloadScene();
    }

    internal static List<Type> GetComponents()
    {
        return _loadedComponents;
    }
}

///EXAMPLE

// var DLL = Assembly.LoadFile(@"D:\dev\Engine2D\src\ExampleGame\bin\Debug\net7.0\ExampleGame.dll");
// var theType = DLL.GetType("ExampleGame.TestClass");
// var c = Activator.CreateInstance(theType);
// var t = (Component)c;
// Console.WriteLine(t.GetFieldSize());