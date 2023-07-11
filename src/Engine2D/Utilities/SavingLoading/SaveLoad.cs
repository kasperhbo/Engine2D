#region

using System.Reflection;
using System.Runtime.Serialization.Formatters.Soap;
using Engine2D.Components.ENTT;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.Scenes;
using Engine2D.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

    // public static Gameobject? LoadGameobject(string relativePath)
    // {
    //     string path = ProjectSettings.FullProjectPath + "\\" + relativePath;
    //     if (File.Exists(path))
    //     {
    //         var lines = File.ReadAllText(path);
    //         return JsonConvert.DeserializeObject<Gameobject>(lines).Clone(UIDManager.GetUID());
    //     }
    //
    //     return null;
    // }
    // public static void SaveGameobject(string fileName, Gameobject gameobject)
    // {
    //     fileName += ".prefab";
    //     var lines = JsonConvert.SerializeObject(gameobject, Formatting.Indented);
    //     using (var fs = File.Create(fileName))
    //     {
    //         fs.Close();
    //     }
    //     File.WriteAllText(fileName, lines);
    // }
    
    internal static void SaveScene(Scene scene)
    {
        Log.Message("Saving: " + scene.ScenePath);
            
            // Serialize the EntityRegistry to a JSON string
            // string json = JsonConvert.SerializeObject(registry, Formatting.Indented);
            // Create a dictionary to hold entity IDs and their corresponding serialized data
            Dictionary<int, JObject> serializedEntities = new Dictionary<int, JObject>();

            foreach (var entity in scene.Entities)
            {
                // Entity entity = entityPair.Value;

                // Serialize the components of the entity
                JObject serializedEntity = new JObject();
                
                string serializedData = JsonConvert.SerializeObject(entity);
                serializedEntity[entity.GetType().FullName] = JToken.Parse(serializedData);
               
                if(entity.HasComponent<ENTTTransformComponent>())
                {
                    var component = entity.GetComponent<ENTTTransformComponent>();
                    string serializedComponent = JsonConvert.SerializeObject(component);
                    serializedEntity[component.GetType().FullName] = JToken.Parse(serializedComponent);
                }  
                
                if(entity.HasComponent<ENTTTagComponent>())
                {
                    var component = entity.GetComponent<ENTTTagComponent>();
                    string serializedComponent = JsonConvert.SerializeObject(component);
                    serializedEntity[component.GetType().FullName] = JToken.Parse(serializedComponent);
                }  
                
                if(entity.HasComponent<ENTTSpriteRenderer>())
                {
                    var component = entity.GetComponent<ENTTSpriteRenderer>();
                    string serializedComponent = JsonConvert.SerializeObject(component);
                    serializedEntity[component.GetType().FullName] = JToken.Parse(serializedComponent);
                }  
                
                // Add the serialized entity to the dictionary
                serializedEntities.Add(entity.UUID, serializedEntity);
            }

            // Serialize the entity registry to a JSON string
            string json = JsonConvert.SerializeObject(serializedEntities, Formatting.Indented);

            
            if (File.Exists(scene.ScenePath))
            {
                File.WriteAllText(scene.ScenePath, json);
            }
            else
            {
                using (var fs = File.Create(scene.ScenePath))
                {
                    fs.Close();
                }
            
                File.WriteAllText(scene.ScenePath, json);
            }
            
            Log.Succes("Succesfully saved: " + scene.ScenePath);
    }

    public static void LoadScene(string filePath, Scene scene)
    {
        // Read the JSON string from the file
        string json = File.ReadAllText(filePath);

        //Check if json file is empty
        if (json == "[]" || json == "") return;
        
        // Deserialize the JSON string to recreate the serialized entities
        var serializedEntities = JsonConvert.DeserializeObject<Dictionary<int, JObject>>(json);

        // Clear the existing entity registry
        scene.Entities.Clear();

        // Deserialize each entity and its components
        foreach (var serializedEntityPair in serializedEntities)
        {
            JObject serializedEntity = serializedEntityPair.Value;

            var props = serializedEntity.Properties();
            var prop = props.First();
            
            string typeName = prop.Name;
            string serializedEnt = prop.Value.ToString();

            // Deserialize each component and add it to the entity
            Type type = Type.GetType(typeName);
            object obj = JsonConvert.DeserializeObject(serializedEnt, type);
            
            Entity entityClone = (Entity) obj;
            
            // Create a new entity
            var entity = scene.CreateEntity(entityClone.UUID);
            
            foreach (var componentProperty in serializedEntity.Properties())
            {
                string componentTypeName = componentProperty.Name;
                string serializedComponent = componentProperty.Value.ToString();

                // Deserialize each component and add it to the entity
                Type componentType = Type.GetType(componentTypeName);
                object component = JsonConvert.DeserializeObject(serializedComponent, componentType);

                if(component is ENTTTransformComponent transformComponent)
                    entity.AddComponent(transformComponent);
                else if(component is ENTTTagComponent tagComponent)
                    entity.AddComponent(tagComponent);
                else if(component is ENTTSpriteRenderer spriteRenderer)
                {
                    spriteRenderer.SetParent(entity);
                    entity.AddComponent(spriteRenderer);
                }
                else
                    Log.Error("Component not found: " + component.GetType().FullName);
                
            }
        }
    }
    //
    // public static List<Entity>? LoadScene(string scenePath)
    // {
    //     var lines = File.ReadAllText(scenePath);
    //     var objs = JsonConvert.DeserializeObject<List<Entity>>(lines);
    //
    //     return objs;
    // }
}