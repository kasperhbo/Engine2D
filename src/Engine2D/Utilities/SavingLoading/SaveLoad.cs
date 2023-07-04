#region

using System.Reflection;
using System.Runtime.Serialization.Formatters.Soap;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.Scenes;
using Engine2D.UI;
using Newtonsoft.Json;

#endregion

namespace Engine2D.SavingLoading;

public static class SaveLoad
{
    internal static bool SaveStatic(Type static_class, string filename)
    {
        try
        {
            var fields = static_class.GetFields(BindingFlags.Static | BindingFlags.Public);
            var a = new object[fields.Length, 2];
            var i = 0;
            foreach (var field in fields)
            {
                a[i, 0] = field.Name;
                a[i, 1] = field.GetValue(null);
                i++;
            }

            ;
            Stream f = File.Open(filename, FileMode.Create);
            var formatter = new SoapFormatter();
            formatter.Serialize(f, a);
            f.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

    internal static bool LoadStatic(Type static_class, string filename)
    {
        try
        {
            var fields = static_class.GetFields(BindingFlags.Static | BindingFlags.Public);
            object[,] a;
            Stream f = File.Open(filename, FileMode.Open);
            var formatter = new SoapFormatter();
            a = formatter.Deserialize(f) as object[,];
            f.Close();
            if (a.GetLength(0) != fields.Length) return false;
            var i = 0;
            foreach (var field in fields)
            {
                if (field.Name == a[i, 0] as string) field.SetValue(null, a[i, 1]);
                i++;
            }

            ;
            return true;
        }
        catch
        {
            return false;
        }
    }


    internal static void LoadWindowSettings()
    {
        var saveLocation = Utils.GetBaseEngineDir() + "\\Settings\\";
        var saveFile = saveLocation + "WindowSettings.dat";
        Utils.LoadWithSoapStaticClass(typeof(UiRenderer), saveFile);
    }

    internal static void SaveWindowSettings()
    {
        var saveLocation = Utils.GetBaseEngineDir() + "\\Settings\\";
        var saveFile = saveLocation + "WindowSettings.dat";
        var ok = Utils.SaveWithSoapStaticClass(typeof(UiRenderer), saveFile);
    }
    
    

    internal static string? GetNextFreeName(string? fullPath)
    {
        string directory = Path.GetDirectoryName(fullPath);
        string baseName = Path.GetFileNameWithoutExtension(fullPath);
        string extension = Path.GetExtension(fullPath);

        string fileName = fullPath;
        int counter = 1;

        while (File.Exists(fileName))
        {
            string newBaseName = $"{baseName}_{counter}";
            fileName = Path.Combine(directory, newBaseName + extension);
            counter++;
        }

        return fileName;
    }

    private static FileInfo GetFileInfo(FileInfo[] files, string? name)
    {
        foreach (var file in files)
            if (name == file.Name)
                return file;

        return null;
    }


    internal static void LoadEngineSettings()
    {
    }

    internal static void SaveEngineSettings()
    {
    }

    public static Gameobject? LoadGameobject(string relativePath)
    {
        string path = ProjectSettings.FullProjectPath + "\\" + relativePath;
        if (File.Exists(path))
        {
            var lines = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Gameobject>(lines).Clone(UIDManager.GetUID());
        }

        return null;
    }
    public static void SaveGameobject(string path, Gameobject gameobject)
    {
        var fileName = String.Format(path + "gameobject" + ".prefab");
        var lines = JsonConvert.SerializeObject(gameobject, Formatting.Indented);
        using (var fs = File.Create(fileName))
        {
            fs.Close();
        }
        File.WriteAllText(fileName, lines);
    }
    
    
    
    #region scenes

    internal static void SaveScene(Scene scene)
    {
        Log.Message("Saving: " + scene.ScenePath);

        //TODO: Remove this and make just an seperate array/list in the scene
        var tempList = new List<Gameobject?>();

        foreach (var go in scene.GameObjects)
            if (go.Serialize)
                tempList.Add(go);

        var gameObjectArray = tempList.ToArray();


        var sceneData = JsonConvert.SerializeObject(gameObjectArray, Formatting.Indented);
        //So we can see where the go array stops when we deserialize the file
        sceneData += "\n////GAMEOBJECTS////\n";

        if (File.Exists(scene.ScenePath))
        {
            File.WriteAllText(scene.ScenePath, sceneData);
        }
        else
        {
            using (var fs = File.Create(scene.ScenePath))
            {
                fs.Close();
            }

            File.WriteAllText(scene.ScenePath, sceneData);
        }

        Log.Succes("Succesfully saved: " + scene.ScenePath);
    }

    internal static List<Gameobject?> LoadScene(string sceneToLoad)
    {
        var objs = new List<Gameobject?>();
        if (File.Exists(sceneToLoad))
        {
            var lines = File.ReadAllLines(sceneToLoad);

            var gos = "";
            var lightSettingsStr = "";

            var lastIndex = 0;
            for (var i = 0; i < lines.Length; i++)
            {
                lastIndex = i;
                //if comment then we now we are at and of GO array and we break
                if (lines[i].Contains("////GAMEOBJECTS////")) break;
                gos += lines[i];
            }

            lastIndex += 1;
            for (var i = lastIndex; i < lines.Length; i++)
            {
                lastIndex++;
                if (lines[i].Contains("////LightSettings////")) break;
                lightSettingsStr += lines[i];
            }

            //Load gameobjects
            objs = JsonConvert.DeserializeObject<List<Gameobject>>(gos)!;
        }

        Log.Succes("qloaded: " + sceneToLoad);
        return objs;
    }

    #endregion
}