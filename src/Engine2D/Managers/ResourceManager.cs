using Engine2D.Components.Sprites;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Engine2D.UI.Browsers;
using Engine2D.UI.Debug;
using KDBEngine.Shaders;
using Newtonsoft.Json;

namespace Engine2D.Managers
{
    internal struct ShaderData
    {
        internal string FragPath;
        internal string VertexPath;
    }

    internal static class ResourceManager
    {
        private static Dictionary<string, AssetBrowserAsset> _items =
            new Dictionary<string, AssetBrowserAsset>(StringComparer.OrdinalIgnoreCase);

        private static  Dictionary<ShaderData, Shader> Shaders = new Dictionary<ShaderData, Shader>();
        private static  bool _showDebug = false;

        internal static List<SaveTextureClass> TexturesToSave = new();
        
        static ResourceManager()
        {
            Log.Message("Loading resources");
            LoadAssets();
        }

        public static void Flush()
        {
            _items =
            new Dictionary<string, AssetBrowserAsset>(StringComparer.OrdinalIgnoreCase);

            Shaders = new Dictionary<ShaderData, Shader>();
            _showDebug = false;
    
            TexturesToSave = new();
            
            LoadAssets();
        }

        private static void LoadAssets()
        {
            _items.Clear();
            
            LoadTextures();
            LoadSprites();

            if (_showDebug)
                Log.Succes("Succesfully loaded all assets!");
        }
        
        private static void LoadTextures()
        {
            var baseAssetPath = ProjectSettings.FullProjectPath + @"\" +"assets";
            baseAssetPath.ToLower();
            var supportedExtensions = new[] { ".tex", };

            var files = Directory.GetFiles(baseAssetPath, "*.*", SearchOption.AllDirectories)
                .Where(file => supportedExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase));

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file);
                var item = LoadAssetFromFile(file, extension);
                if (item != null)
                {
                    AddItemToManager(file, item);
                }
            }
        }

        private static void LoadSprites()
        {
            var baseAssetPath = ProjectSettings.FullProjectPath + @"\" +"assets";
            baseAssetPath.ToLower();
            var supportedExtensions = new[] { ".sprite", };

            var files = Directory.GetFiles(baseAssetPath, "*.*", SearchOption.AllDirectories)
                .Where(file => supportedExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase));

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file);
                var item = LoadAssetFromFile(file, extension);
                if (item != null)
                    AddItemToManager(file, item);
            }
        }

        private static AssetBrowserAsset? LoadAssetFromFile(string filePath, string extension)
        {
            try
            {
                switch (extension)
                {
                    case ".tex":
                        return LoadTextureFromJson(filePath);
                    case ".sprite":
                        return LoadSpriteFromJson(filePath);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed to load asset {filePath}! {e.Message}");
            }

            return null;
        }
        
        
        private static void AddItemToManager(string fullSaveName, AssetBrowserAsset item)
        {
            fullSaveName = fullSaveName.ToLower();

            _items[fullSaveName] = item;

            if (item is Texture)
            {
                DebugStats.TexturesLoadedByResourceManager++;
            }
            
            if (item is Sprite)
            {
                DebugStats.SpritesLoadedByResourceManager++;
            }
            
        }

        public static int GetItemIndex(string? path)
        {
            if (path == null) return -1;

            // path = Path.Combine(ProjectSettings.FullProjectPath, path.ToLower());
            path = ProjectSettings.FullProjectPath + path;
            path.ToLower();
            
            for (var i = 0; i < _items.Count; i++)
            {
                var item = _items.ElementAt(i);
                if (string.Equals(item.Key, path, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return -1;
        }

        public static T? GetItem<T>(string? path) where T : AssetBrowserAsset
        {
            if (path == null) return null;
            
            // path = Path.Combine(ProjectSettings.FullProjectPath, path.ToLower());
            path = ProjectSettings.FullProjectPath + path;
            path = path.ToLower();
            
                                               
            for (var i = 0; i < _items.Count; i++)
            {
                var item = _items.ElementAt(i);
                if (string.Equals(item.Key, path, StringComparison.OrdinalIgnoreCase))
                    if(item.Value is T typedItem)
                        return typedItem;
            }
            
            

            return null;
        }

        #region Loading

        internal static Shader? GetShader(ShaderData shaderData)
        {
            if (!Shaders.TryGetValue(shaderData, out var shader))
            {
                shader = new Shader(shaderData.VertexPath, shaderData.FragPath);
                Shaders.Add(shaderData, shader);
            }

            return shader;
        }

        private static Texture? LoadTextureFromJson(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var textureData = File.ReadAllText(filePath);
                    if (_showDebug)
                        Log.Succes("Loaded texture " + filePath);
                    return JsonConvert.DeserializeObject<Texture>(textureData);
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to load texture {filePath}! {e.Message}");
                }
            }
            return null;
        }
        
        private static Sprite? LoadSpriteFromJson(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var spriteData = File.ReadAllText(filePath);
                    if (_showDebug)
                        Log.Succes("Loaded sprite " + filePath);
                    return JsonConvert.DeserializeObject<Sprite>(spriteData);
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to load sprite {filePath}! {e.Message}");
                }
            }
            return null;
        }
        
    #endregion

        
        #region Saving
        
        internal static void SaveTexture(string? defaultSaveName, Texture texture, DirectoryInfo? currentFolder = null,
            bool overwrite = false)
        {
            var name = defaultSaveName;

            if (!overwrite)
            {
                string extension = Path.GetExtension(name);
                name = SaveLoad.GetNextFreeName(defaultSaveName);
            }

            var fullSaveName = (currentFolder?.FullName ?? string.Empty) + name;
            fullSaveName = ProjectSettings.FullProjectPath + fullSaveName;

            try
            {
                var textureData = JsonConvert.SerializeObject(texture, Formatting.None);
                File.WriteAllText(fullSaveName, textureData);
                AddItemToManager(fullSaveName, texture);
                Log.Succes("Texture saved Succesfully: " + fullSaveName);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to save texture: {fullSaveName}. {e.Message}");
            }
        }
        
        internal static void SaveSprite(string? defaultSaveName, Sprite sprite, DirectoryInfo? currentFolder = null,
            bool overwrite = true)
        {
            var name = defaultSaveName;
            
            if(name[0].ToString() != "\\")
                name = "\\" + name;
            
            if (!overwrite)
            {
                string extension = Path.GetExtension(name);
                name = SaveLoad.GetNextFreeName(defaultSaveName);
            }
            
            name = "\\assets" + name;

            sprite.SavePath = name; 
            
            
            var fullSaveName = (currentFolder?.FullName ?? string.Empty) + name;
            fullSaveName = ProjectSettings.FullProjectPath + fullSaveName;

            try
            {
                var dir = Path.GetDirectoryName(fullSaveName);
                if(!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                
                var spriteData = JsonConvert.SerializeObject(sprite, Formatting.None);
                File.WriteAllText(fullSaveName, spriteData);

                AddItemToManager(fullSaveName, sprite);

                Log.Succes("Texture saved Succesfully: " + fullSaveName);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to save texture: {fullSaveName}. {e.Message}");
            }
        }
        
        #endregion
    }
}

internal class SaveTextureClass
{
    internal string? defaultSaveName;
    internal Texture texture;
    internal DirectoryInfo? currentFolder = null;
    internal bool overwrite = false;

    public SaveTextureClass(string? defaultSaveName, Texture texture, DirectoryInfo? currentFolder, bool overwrite)
    {
        this.defaultSaveName = defaultSaveName;
        this.texture = texture;
        this.currentFolder = currentFolder;
        this.overwrite = overwrite;
    }
}