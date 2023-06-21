﻿#region

using Engine2D.Components.Sprites;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Engine2D.UI.Browsers;
using KDBEngine.Shaders;
using Newtonsoft.Json;

#endregion

namespace Engine2D.Managers;

internal struct ShaderData
{
    internal string FragPath;
    internal string VertexPath;
}

internal static class ResourceManager
{
    private static readonly Dictionary<ShaderData, Shader?> Shaders = new();
    private static readonly Dictionary<string, AssetBrowserAsset?> _items = new();
    internal static List<SpriteRenderer> SpriteRenderers = new();

    private static readonly bool _showDebug = false;

    static ResourceManager()
    {
        Log.Message("Loading resources");
        //Get all sprites in project
        var baseAssetPath = ProjectSettings.FullProjectPath + "\\assets";

        var files = Directory.GetFiles(baseAssetPath, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".tex") || s.EndsWith(".sprite") || s.EndsWith(".spritesheet"));
        ;

        var enumerable = files as string[] ?? files.ToArray();

        for (var i = 0; i < enumerable.Length; i++)
        {
            var file = enumerable[i].ToLower();

            AssetBrowserAsset? item = null;

            var extension = file.Remove(0, file.LastIndexOf(".") + 1);

            if (Enum.TryParse(extension, out ESupportedFileTypes ext))
            {
                switch (ext)
                {
                    case ESupportedFileTypes.sprite:
                        item = LoadSpriteFromJson(file);
                        break;
                    case ESupportedFileTypes.tex:
                        item = LoadTextureFromJson(file);
                        break;
                    case ESupportedFileTypes.spritesheet:
                        item = LoadSpriteSheetFromJson(file);
                        break;
                }

                if (item != null)
                    AddItemToManager(file, item);
            }
        }

        if (_showDebug)
            Log.Succes("Successfully loaded all assets!");
    }

    internal static T? GetItem<T>(string? path) where T : AssetBrowserAsset
    {
        path = path.ToLower();
        for (var i = 0; i < _items.Count; i++)
        {
            var item = _items.ElementAt(i);
            var p = item.Key;
            p = p.ToLower();
            var type = item.Value;

            if (typeof(T) == type?.GetType())
                if (p == path)
                    return
                        (type as T)!;
        }

        return null;
    }

    internal static Shader? GetShader(ShaderData shaderLocations)
    {
        Shader? shader;

        if (!Shaders.TryGetValue(shaderLocations, out shader))
        {
            shader = new Shader(shaderLocations.VertexPath, shaderLocations.FragPath);
            Shaders.Add(shaderLocations, shader);
        }

        return shader;
    }

    internal static void AddItemToManager(string fullSaveName, AssetBrowserAsset item)
    {
        if (_items.TryGetValue(fullSaveName, out var itemFoundItem))
            _items[fullSaveName] = item;
        else
            _items.Add(fullSaveName, item);
    }

    #region Saving and loading

    //Save Sprites
    internal static void SaveSprite(string? defaultSaveName, Sprite sprite, DirectoryInfo? currentFolder = null,
        bool overWrite = false)
    {
        var name = defaultSaveName;

        if (!overWrite)
            name = SaveLoad.GetNextFreeName(defaultSaveName, currentFolder);

        var fullSaveName = name;

        if (currentFolder != null)
            fullSaveName = currentFolder.FullName + "\\" + name;

        var spriteData = JsonConvert.SerializeObject(sprite, Formatting.Indented);
        File.WriteAllText(fullSaveName, spriteData);

        AddItemToManager(fullSaveName, sprite);

        AssetBrowserPanel.Refresh();

        //TODO: MAKE THIS MORE EFFECIENT SO ONLY THE SPRITES WITH THE CHANGED SPRITE SHEET/FILE GET UPDATED
        foreach (var spr in SpriteRenderers) spr.RefreshSprite();
    }

    internal static Sprite? LoadSpriteFromJson(string? filename)
    {
        if (File.Exists(filename))
        {
            var textureData = File.ReadAllText(filename);
            if (_showDebug)
                Log.Succes("Loaded texture " + filename);
            return JsonConvert.DeserializeObject<Sprite>(textureData)!;
        }

        Log.Error("Can't load texture " + filename);
        return null;
    }

    //Save Textures
    internal static void SaveTexture(string? defaultSaveName, Texture texture, DirectoryInfo? currentFolder = null,
        bool overWrite = false)
    {
        var name = defaultSaveName;

        if (!overWrite)
            name = SaveLoad.GetNextFreeName(defaultSaveName, currentFolder);

        var fullSaveName = name;

        if (currentFolder != null)
            fullSaveName = currentFolder.FullName + "\\" + name;

        var textureData = JsonConvert.SerializeObject(texture, Formatting.Indented);
        File.WriteAllText(fullSaveName, textureData);

        //TODO: ADDTEXTURE
        //AddItem(fullSaveName,texture);
        AssetBrowserPanel.Refresh();


        //TODO: MAKE THIS MORE EFFECIENT SO ONLY THE SPRITES WITH THE CHANGED SPRITE SHEET/FILE GET UPDATED
        foreach (var spr in SpriteRenderers) spr.RefreshSprite();
    }

    internal static Texture? LoadTextureFromJson(string? filename)
    {
        if (File.Exists(filename))
        {
            var textureData = File.ReadAllText(filename);
            if (_showDebug)
                Log.Succes("Loaded texture " + filename);
            return JsonConvert.DeserializeObject<Texture>(textureData)!;
        }

        Log.Error("Can't load texture " + filename);
        return null;
    }

    //Save sprite sheets
    internal static void SaveSpriteSheet(string? defaultSaveName, SpriteSheet spriteSheet,
        DirectoryInfo? currentFolder = null, bool overWrite = false)
    {
        var name = defaultSaveName;

        if (!overWrite)
            name = SaveLoad.GetNextFreeName(defaultSaveName, currentFolder);

        var fullSaveName = name;

        if (currentFolder != null)
            fullSaveName = currentFolder.FullName + "\\" + name;

        var textureData = JsonConvert.SerializeObject(spriteSheet, Formatting.Indented);
        File.WriteAllText(fullSaveName, textureData);

        AddItemToManager(fullSaveName, spriteSheet);
        AssetBrowserPanel.Refresh();

        //TODO: MAKE THIS MORE EFFECIENT SO ONLY THE SPRITES WITH THE CHANGED SPRITE SHEET/FILE GET UPDATED
        foreach (var spr in SpriteRenderers) spr.RefreshSprite();
    }

    internal static SpriteSheet? LoadSpriteSheetFromJson(string? filename)
    {
        if (File.Exists(filename))
        {
            var spritesheet = File.ReadAllText(filename);
            if (_showDebug)
                Log.Succes("Loaded sprite sheet " + filename);

            return JsonConvert.DeserializeObject<SpriteSheet>(spritesheet)!;
        }

        Log.Error("Can't load texture " + filename);
        return null;
    }

    #endregion
}