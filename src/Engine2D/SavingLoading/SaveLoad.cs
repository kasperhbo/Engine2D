﻿using System.Xml;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.Scenes;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Engine2D.SavingLoading;

internal static class SaveLoad
{
    #region engine settings

    internal static void LoadEngineSettings()
    {
        var saveLocation = Utils.GetBaseEngineDir() + "\\Settings\\";
        var saveFile = saveLocation + "EngineSettings.dat";

        var ok = Utils.LoadWithSoapStaticClass(typeof(EngineSettings), saveFile);
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
        Log.Message("Saving: " + scene.ScenePath);
        
        //TODO: Remove this and make just an seperate array/list in the scene
        List<Gameobject> tempList = new List<Gameobject>();
        
        foreach (var go in scene.GameObjects)
        {
            if(go.Serialize)tempList.Add(go);
        }
        
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

    #region Sprites

    public static void SaveSprite(Sprite sprite, DirectoryInfo currentFolder)
    {
        string defaultSaveName = "sprite.sprite";
        string name = GetNextFreeName(defaultSaveName, currentFolder);
        string fullSaveName = currentFolder.FullName + "\\" +name;
        sprite.FullSavePath = fullSaveName;
        var spriteData = JsonConvert.SerializeObject(sprite, Formatting.Indented);
        File.WriteAllText(fullSaveName, spriteData);
    }
    
    
    public static void OverWriteSprite(Sprite sprite)
    {
        var spriteData = JsonConvert.SerializeObject(sprite, Formatting.Indented);
        File.WriteAllText(sprite.FullSavePath, spriteData);
    }


    public static Sprite LoadSpriteFromJson(string? filename)
    {
        if (File.Exists(filename))
        {
            string spriteStringData = File.ReadAllText(filename);
            //Load gameobjects
            return JsonConvert.DeserializeObject<Sprite>(spriteStringData)!;
        }

        return null;
    }


#endregion
    
    private static string GetNextFreeName(string name, DirectoryInfo folder)
    {
        string fullName = folder.FullName + "\\" + name;
        
        if(File.Exists(fullName))
        {
            var fInfo = GetFileInfo(folder.GetFiles(), name);
            int extensionIndex = name.IndexOf(fInfo.Extension);
            name = name.Remove(extensionIndex);
            name += "1";
            name += fInfo.Extension;
            return GetNextFreeName(name, folder);
        }

        return name;
    }

    private static FileInfo GetFileInfo(FileInfo[] files, string name)
    {
        foreach (var file in files)
        {
            if (name == file.Name)
            {
                return file;
            }
        }

        return null;
    }


}