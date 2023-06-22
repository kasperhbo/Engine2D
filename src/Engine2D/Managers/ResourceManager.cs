using Engine2D.Components.SpriteAnimations;
using Engine2D.Components.Sprites;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Engine2D.UI.Browsers;
using KDBEngine.Shaders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        private static readonly Dictionary<ShaderData, Shader> Shaders = new Dictionary<ShaderData, Shader>();
        private static readonly bool _showDebug = false;

        internal static List<SaveTextureClass> TexturesToSave = new();
        internal static List<SaveSpriteSheetClass> SpriteSheetsToSave = new();
        internal static List<SaveAnimationClass> AnimationsToSave = new();
        
        public static void OnGUI()
        {
            bool refreshobjects = false;
            foreach (var action in TexturesToSave)
            {
                refreshobjects = true;
                SaveTexture(action.defaultSaveName, action.texture, action.currentFolder, action.overwrite);
            }

            TexturesToSave = new();
            
            foreach (var action in SpriteSheetsToSave)
            {
                refreshobjects = true;
                SaveSpriteSheet(action.defaultSaveName, action.spriteSheet, action.currentFolder, action.overwrite);
            }

            SpriteSheetsToSave = new();
            
            foreach (var action in AnimationsToSave)
            {   
                refreshobjects = true;
                SaveAnimation(action.defaultSaveName, action.animation, action.currentFolder, action.overwrite);
            }
            
            AnimationsToSave = new();
            
            if(refreshobjects){
                Log.Message("Start refreshing objects");
                
                foreach (var keyvp in _items)
                {
                    if (keyvp.Value is SpriteSheet spriteSheet)
                        spriteSheet.Refresh();
                }

                foreach (var go in Engine.Get().CurrentScene.GameObjects)
                {
                    if (go.GetComponent<SpriteRenderer>() != null)
                        go.GetComponent<SpriteRenderer>().Refresh();
                }
                
                Log.Succes("Refreshed");
            }
        }
        
        static ResourceManager()
        {
            Log.Message("Loading resources");
            LoadAssets();
        }

        private static void LoadAssets()
        {
            _items.Clear();
            
            var baseAssetPath = ProjectSettings.FullProjectPath + @"\" +"assets";
            baseAssetPath.ToLower();
            var supportedExtensions = new[] { ".tex", ".sprite", ".spritesheet", ".animation" };

            var files = Directory.GetFiles(baseAssetPath, "*.*", SearchOption.AllDirectories)
                .Where(file => supportedExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase));

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file);
                var item = LoadAssetFromFile(file, extension);
                if (item != null)
                    AddItemToManager(file, item);
            }

            if (_showDebug)
                Log.Succes("Succesfully loaded all assets!");
        }

        private static AssetBrowserAsset? LoadAssetFromFile(string filePath, string extension)
        {
            try
            {
                switch (extension)
                {
                    case ".tex":
                        return LoadTextureFromJson(filePath);
                    case ".spritesheet":
                        return LoadSpriteSheetFromJson(filePath);
                    case ".animation":
                        return LoadAnimationFromJson(filePath);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed to load asset {filePath}! {e.Message}");
            }

            return null;
        }

        private static Sprite? LoadSpriteFromJson(string filePath)
        {
            try
            {
                var spriteData = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<Sprite>(spriteData);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to load sprite {filePath}! {e.Message}");
                return null;
            }
        }

        internal static Texture? LoadTextureFromJson(string filePath)
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

        private static SpriteSheet? LoadSpriteSheetFromJson(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var spriteSheetData = File.ReadAllText(filePath);
                    if (_showDebug)
                        Log.Succes("Loaded sprite sheet " + filePath);
                    return JsonConvert.DeserializeObject<SpriteSheet>(spriteSheetData);
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to load sprite sheet {filePath}! {e.Message}");
                }
            }

            return null;
        }

        private static Animation? LoadAnimationFromJson(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var animationData = File.ReadAllText(filePath);
                    if (_showDebug)
                        Log.Succes("Loaded animation " + filePath);
                    return JsonConvert.DeserializeObject<Animation>(animationData);
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to load animation {filePath}! {e.Message}");
                }
            }

            return null;
        }
// ...

        private static void AddItemToManager(string fullSaveName, AssetBrowserAsset item)
        {
            fullSaveName = fullSaveName.ToLower();

            _items[fullSaveName] = item;
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
            path.ToLower();

            for (var i = 0; i < _items.Count; i++)
            {
                var item = _items.ElementAt(i);
                if (string.Equals(item.Key, path, StringComparison.OrdinalIgnoreCase) && item.Value is T typedItem)
                    return typedItem;
            }

            return null;
        }

        internal static Shader? GetShader(ShaderData shaderLocations)
        {
            if (!Shaders.TryGetValue(shaderLocations, out var shader))
            {
                shader = new Shader(shaderLocations.VertexPath, shaderLocations.FragPath);
                Shaders.Add(shaderLocations, shader);
            }

            return shader;
        }

        private static void SaveTexture(string? defaultSaveName, Texture texture, DirectoryInfo? currentFolder = null,
            bool overwrite = false)
        {
            var name = defaultSaveName;

            if (!overwrite)
                name = SaveLoad.GetNextFreeName(defaultSaveName, currentFolder);

            var fullSaveName = (currentFolder?.FullName ?? string.Empty) + name;
            fullSaveName = ProjectSettings.FullProjectPath + fullSaveName;

            try
            {
                var textureData = JsonConvert.SerializeObject(texture, Formatting.Indented);
                File.WriteAllText(fullSaveName, textureData);

                AssetBrowserPanel.Refresh();
                LoadAssets();

                Log.Succes("Texture saved Succesfully: " + fullSaveName);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to save texture: {fullSaveName}. {e.Message}");
            }
        }

        private static void SaveSpriteSheet(string? defaultSaveName, SpriteSheet spriteSheet,
            DirectoryInfo? currentFolder = null, bool overwrite = false)
        {
            var name = defaultSaveName;

            if (!overwrite)
                name = SaveLoad.GetNextFreeName(defaultSaveName, currentFolder);

            var fullSaveName = (currentFolder?.FullName ?? string.Empty) + name;
            fullSaveName = ProjectSettings.FullProjectPath + fullSaveName;

            try
            {
                var spriteSheetData = JsonConvert.SerializeObject(spriteSheet, Formatting.Indented);
                File.WriteAllText(fullSaveName, spriteSheetData);

                AddItemToManager(fullSaveName, spriteSheet);
                AssetBrowserPanel.Refresh();
                
                Log.Succes("Sprite sheet saved Succesfully: " + fullSaveName);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to save sprite sheet: {fullSaveName}. {e.Message}");
            }
        }

        private static void SaveAnimation(string savePath, Animation animation, DirectoryInfo? currentFolder = null,
            bool overwrite = false)
        {
            var name = savePath;

            if (!overwrite)
                name = SaveLoad.GetNextFreeName(savePath, currentFolder);

            var fullSaveName = (currentFolder?.FullName ?? string.Empty) + name;
            fullSaveName = ProjectSettings.FullProjectPath + fullSaveName;

            try
            {
                var animationData = JsonConvert.SerializeObject(animation, Formatting.Indented);
                File.WriteAllText(fullSaveName, animationData);

                AddItemToManager(fullSaveName, animation);
                AssetBrowserPanel.Refresh();

                foreach (var gameObject in Engine.Get().CurrentScene.GameObjects)
                {
                    var spriteAnimator = gameObject.GetComponent<SpriteAnimator>();
                    spriteAnimator?.Refresh();
                }

                Log.Succes("Animation saved Succesfully: " + fullSaveName);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to save animation: {fullSaveName}. {e.Message}");
            }
        }
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

internal class SaveSpriteSheetClass
{
    internal string? defaultSaveName;
    internal SpriteSheet spriteSheet;
    internal DirectoryInfo? currentFolder = null;
    internal bool overwrite = false;

    public SaveSpriteSheetClass(string? defaultSaveName, SpriteSheet spriteSheet, DirectoryInfo? currentFolder, bool overwrite)
    {
        this.defaultSaveName = defaultSaveName;
        this.spriteSheet = spriteSheet;
        this.currentFolder = currentFolder;
        this.overwrite = overwrite;
    }
}

internal class SaveAnimationClass
{
    internal string? defaultSaveName;
    internal Animation animation;
    internal DirectoryInfo? currentFolder = null;
    internal bool overwrite = false;

    internal SaveAnimationClass(string? defaultSaveName, Animation animation, DirectoryInfo? currentFolder, bool overwrite)
    {
        this.defaultSaveName = defaultSaveName;
        this.animation = animation;
        this.currentFolder = currentFolder;
        this.overwrite = overwrite;
    }
}
