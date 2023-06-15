using Engine2D.Components.Sprites;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Engine2D.UI.Browsers;
using KDBEngine.Shaders;
using Newtonsoft.Json;
using OpenTK.Graphics.ES11;

namespace Engine2D.Managers;

internal struct ShaderData
{
    internal string FragPath;
    internal string VertexPath;
}

internal static class ResourceManager
{
    private static readonly Dictionary<ShaderData, Shader> _shaders = new();

    private static Dictionary<string?, AssetBrowserAsset?> items = new Dictionary<string?, AssetBrowserAsset?>();

    static ResourceManager()
    {
        Log.Message("Loading resources");
        //Get all sprites in project
        string baseAssetPath = ProjectSettings.FullProjectPath + "\\assets";

        var files = Directory.GetFiles(baseAssetPath, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".tex") || s.EndsWith(".sprite") || s.EndsWith(".spritesheet"));;

        var enumerable = files as string[] ?? files.ToArray();
        
        for (int i = 0; i < enumerable.Length; i++)
        {
            string? file = enumerable[i].ToLower();
            //Sprites
            if (file.EndsWith(".sprite"))
            {
                Sprite? sprite = LoadSpriteFromJson(file);
                if (sprite != null)
                {
                    items.Add(file, sprite);
                }
            }
            
            if (file.EndsWith(".spritesheet"))
            {
                SpriteSheet? spriteSheet = LoadSpriteSheetFromJson(file);
                if (spriteSheet != null)
                {
                    items.Add(file, spriteSheet);
                }
            }
        }
        
        Log.Succes("Succesfully loaded all assets!");
    }

    public static T? GetItem<T>(string? path) where T : AssetBrowserAsset
    {
        path = path.ToLower();
        for (int i = 0; i < items.Count; i++)
        {
            var item = items.ElementAt(i);
            var p = item.Key;
            var type = item.Value;
            
            if (typeof(T) == type?.GetType())
                if(p == path)
                    return
                        (type as T)!;
        }

        
        return null;
    }
    
    internal static Shader GetShader(ShaderData shaderLocations)
    {
        Shader shader;
        if (!_shaders.TryGetValue(shaderLocations, out shader))
        {
            shader = new Shader(shaderLocations.VertexPath, shaderLocations.FragPath);
            _shaders.Add(shaderLocations, shader);
        }
        return shader;
    }
    
    
    #region Saving and loading
    
    //Save Sprites
    public static void SaveSprite(string? defaultSaveName,Sprite sprite, DirectoryInfo? currentFolder = null, bool overWrite = false)
    {
        string? name = defaultSaveName;
        
        if(!overWrite)
            name = SaveLoad.GetNextFreeName(defaultSaveName, currentFolder);
        
        string? fullSaveName = name;
        
        if(currentFolder != null)
            fullSaveName = currentFolder.FullName + "\\" +name;
        
        var spriteData = JsonConvert.SerializeObject(sprite, Formatting.Indented);
        File.WriteAllText(fullSaveName, spriteData);
        
        items.Add(fullSaveName, sprite);
        AssetBrowserPanel.Refresh();
    }
    
    public static Sprite? LoadSpriteFromJson(string? filename)
    {
        if (File.Exists(filename))
        {
            string textureData = File.ReadAllText(filename);
            Log.Succes("Loaded texture " + filename);
            return JsonConvert.DeserializeObject<Sprite>(textureData)!;
        }
        
        Log.Error("Can't load texture " + filename);
        return null;
    }

    
    //Save Textures
    public static void SaveTexture(string? defaultSaveName,Texture texture, DirectoryInfo? currentFolder = null, bool overWrite = false)
    {
        string? name = defaultSaveName;
        
        if(!overWrite)
             name = SaveLoad.GetNextFreeName(defaultSaveName, currentFolder);
        
        string? fullSaveName = name;
        
        if(currentFolder != null)
            fullSaveName = currentFolder.FullName + "\\" +name;
        
        var textureData = JsonConvert.SerializeObject(texture, Formatting.Indented);
        File.WriteAllText(fullSaveName, textureData);
        //TODO: ADDTEXTURE
        // ResourceManager.AddSprite(fullSaveName, textureData);
        AssetBrowserPanel.Refresh();
    }
    
    public static Texture? LoadTextureFromJson(string? filename)
    {
        if (File.Exists(filename))
        {
            string textureData = File.ReadAllText(filename);
            Log.Succes("Loaded texture " + filename);
            return JsonConvert.DeserializeObject<Texture>(textureData)!;
        }
        
        Log.Error("Can't load texture " + filename);
        return null;
    }
    
    //Save sprite sheets
    public static void SaveSpriteSheet(string? defaultSaveName,SpriteSheet spriteSheet, DirectoryInfo? currentFolder = null, bool overWrite = false)
    {
        string? name = defaultSaveName;
        
        if(!overWrite)
            name = SaveLoad.GetNextFreeName(defaultSaveName, currentFolder);
        
        string? fullSaveName = name;
        
        if(currentFolder != null)
            fullSaveName = currentFolder.FullName + "\\" +name;
        
        var textureData = JsonConvert.SerializeObject(spriteSheet, Formatting.Indented);
        File.WriteAllText(fullSaveName, textureData);
        
        items.Add(defaultSaveName, spriteSheet);
        AssetBrowserPanel.Refresh();
    }
    
    public static SpriteSheet? LoadSpriteSheetFromJson(string? filename)
    {
        if (File.Exists(filename))
        {
            string spritesheet = File.ReadAllText(filename);
            Log.Succes("Loaded sprite sheet " + filename);
            return JsonConvert.DeserializeObject<SpriteSheet>(spritesheet)!;
        }
        
        Log.Error("Can't load texture " + filename);
        return null;
    }
    
    #endregion
    
}