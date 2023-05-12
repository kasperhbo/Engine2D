using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Scenes;
using KDBEngine.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.SavingLoading
{
    internal static class SaveLoad
    {
        #region engine settings
        internal static void LoadEngineSettings()
        {
            string saveLocation = Utils.GetBaseEngineDir() + "\\Settings\\";
            string saveFile = saveLocation + "EngineSettings.dat";

            bool ok = Utils.LoadWithSoapStaticClass(typeof(EngineSettings), saveFile);
            Console.WriteLine("Loading engine: " + ok);
            //DeserializeStaticClass(File.ReadAllText(saveFile), typeof(EngineSettings));           

        }

        internal static void SaveEngineSettings()
        {
            string saveLocation = Utils.GetBaseEngineDir() + "\\Settings\\";
            string saveFile = saveLocation + "EngineSettings.dat";
            bool ok = Utils.SaveWithSoapStaticClass(typeof(EngineSettings), saveFile);
            Console.WriteLine("Saving engine: " + ok);
        }
        #endregion

        #region scenes
        internal static void SaveScene(Scene scene)
        {
            var gameObjectArray = scene.Gameobjects.ToArray();

            string sceneData = JsonConvert.SerializeObject(gameObjectArray, Formatting.Indented);
            //So we can see where the go array stops when we deserialize the file
            sceneData += "\n////GAMEOBJECTS////\n";
            //Save Light settings
            sceneData += JsonConvert.SerializeObject(Engine.Get()._currentScene.LightSettings, Formatting.Indented);
            sceneData += "\n////LightSettings////\n";
            
            if (File.Exists(scene.ScenePath))
            {
                File.WriteAllText(scene.ScenePath, sceneData);
            }
            else
            {
                using (FileStream fs = File.Create(scene.ScenePath))
                {
                    fs.Close();
                }

                File.WriteAllText(scene.ScenePath, sceneData);
            }
        }

        internal static void LoadScene(string sceneToLoad)
        {
            Engine.Get().SwitchScene(sceneToLoad);

            if (File.Exists(sceneToLoad))
            {
                string[] lines = File.ReadAllLines(sceneToLoad);

                string gos = "";
                string lightSettingsStr = "";
                
                int lastIndex=0;
                for (int i = 0; i < lines.Length; i++)
                {
                    lastIndex = i;
                    //if comment then we now we are at and of GO array and we break
                    if (lines[i].Contains("////GAMEOBJECTS////")) break;
                    gos += lines[i];
                }

                lastIndex += 1;
                for (int i = lastIndex; i < lines.Length; i++)
                {
                    lastIndex++;
                    if (lines[i].Contains("////LightSettings////")) break;
                    lightSettingsStr += lines[i];
                }

                //Load gameobjects
                List<Gameobject?> objs = JsonConvert.DeserializeObject<List<Gameobject>>(gos)!;
                
                //Load light settings
                LightSettings lightSettings = JsonConvert.DeserializeObject<LightSettings>(lightSettingsStr);
                
                foreach (var t in objs!)
                {
                    Engine.Get()?._currentScene?.AddGameObjectToScene(t);
                }

                Engine.Get()._currentScene.LightSettings = lightSettings;
            }
        }
        #endregion
    }
}
