using System.Reflection;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;

namespace Engine2D.Utilities;

public static class AssemblyUtils
{
    private static bool _enableDebug = false;

    private static string _loadedAssemblyOrigin = "";
    
    private static Assembly _loadedGameAssembly;
    private static List<Type> _loadedComponents = new();

    /// <summary>
    /// Loads an assembly from a path
    /// </summary>
    /// <param name="assemblyPath"></param>
    public static void LoadAssembly(string assemblyPath)
    {
        _loadedAssemblyOrigin = assemblyPath;
        
        CopyAssembly();

        var data = File.ReadAllBytes(ProjectSettings.FullProjectPath + "\\project.DLL");
        
        _loadedComponents = new();
        _loadedGameAssembly = Assembly.Load(data);

        GetAssemblyComponents();
    }
    
    /// <summary>
    /// Copies the assembly to the project folder
    /// </summary>
    private static void CopyAssembly()
    {
        File.Copy(_loadedAssemblyOrigin, ProjectSettings.FullProjectPath + "\\project.DLL", true);
    }

    /// <summary>
    /// Gets all components from the loaded assembly
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

    /// <summary>
    /// Gets a component from the loaded assembly
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Reloads the assembly and readds all components to the gameobjects
    /// </summary> 
    public static void Reload()
    {
        LoadAssembly(_loadedAssemblyOrigin);
        
        foreach (var go in Engine.Get().CurrentScene.GameObjects)
        {
            Dictionary<Component?, FieldInfo[]> toAdd = new();
            
            
            for (int i = 0; i < go.components.Count; i++)
            {
                // Get the type of FieldsClass.
                Type fieldsType = go.components[i].GetType();
                
                FieldInfo[] fields = fieldsType.GetFields(BindingFlags.Public
                                                          | BindingFlags.Instance);
                foreach (var fieldInfo in fields)           
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("{0}:\t'{1}'", fieldInfo.Name, fieldInfo.GetValue(go.components[i]));
                }
                
                toAdd.Add(go.components[i], fields);
            }

            //FIRST REMOVE ALL COMPONENTS
            go.components = new();
            
            //THEN READD
            for (int i = 0; i < toAdd.Count; i++)
            {
                Component? comp = null;
                
                if (GetComponent(toAdd.ElementAt(i).Key.GetType().ToString()) != null)
                {
                    comp = go.AddComponent(GetComponent(toAdd.ElementAt(i).Key.GetType().ToString()));
                }
                else
                {
                    comp = go.AddComponent(toAdd.ElementAt(i).Key);
                }

                
                //NOW GO OVER ALL FIELDS AND RESET TO THE VALUE IT WAS BEFORE
                for (int j = 0; j < toAdd.ElementAt(i).Value.Length; j++)
                {
                    var element = toAdd.ElementAt(i).Value.ElementAt(j);
                    
                    Type fieldsType = comp.GetType();
                    FieldInfo[] fields = fieldsType.GetFields(BindingFlags.Public
                                                              | BindingFlags.Instance);
                    
                    //reset all fields from the GetFields function to the saved fields in toAdd dictionary
                    for (int k = 0; k < fields.Length; k++)
                    {
                        if (fields[k].Name == element.Name)
                        {
                            fields[k].SetValue(comp, element.GetValue(toAdd.ElementAt(i).Key));
                        }
                    }
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
