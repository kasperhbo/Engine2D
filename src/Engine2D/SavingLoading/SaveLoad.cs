using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Scenes;
using KDBEngine.Core;
using Newtonsoft.Json;

namespace Engine2D.SavingLoading;

internal static class SaveLoad
{
    #region engine settings

    internal static void LoadEngineSettings()
    {
        var saveLocation = Utils.GetBaseEngineDir() + "\\Settings\\";
        var saveFile = saveLocation + "EngineSettings.dat";

        var ok = Utils.LoadWithSoapStaticClass(typeof(EngineSettings), saveFile);
        Console.WriteLine("Loading engine: " + ok);
        //DeserializeStaticClass(File.ReadAllText(saveFile), typeof(EngineSettings));           
    }

    internal static void SaveEngineSettings()
    {
        var saveLocation = Utils.GetBaseEngineDir() + "\\Settings\\";
        var saveFile = saveLocation + "EngineSettings.dat";
        var ok = Utils.SaveWithSoapStaticClass(typeof(EngineSettings), saveFile);
        Console.WriteLine("Saving engine: " + ok);
    }

    #endregion

    #region scenes

    internal static void SaveScene(Scene scene)
    {
        var gameObjectArray = scene.Gameobjects.ToArray();

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

    internal static List<Gameobject> LoadScene(string sceneToLoad)
    {
        List<Gameobject?> objs = new List<Gameobject?>();
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