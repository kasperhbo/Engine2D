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
                List<Gameobject?> objs = JsonConvert.DeserializeObject<List<Gameobject>>(File.ReadAllText(sceneToLoad))!;

                foreach (var t in objs!)
                {
                    Engine.Get()?._currentScene?.AddGameObjectToScene(t);
                }
            }
        }
        #endregion
    }
}
