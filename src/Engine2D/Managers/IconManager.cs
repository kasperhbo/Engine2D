#region

using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Rendering;
using OpenTK.Graphics.OpenGL4;

#endregion

namespace Engine2D.Managers;

internal static class IconManager
{
    //name of icons is: filename - extension so: \Images\Icons\file-icon.png = file-icon
    //file names must contain the word icon
    private static readonly Dictionary<string, Texture> _icons = new();

    private static readonly Texture _textureNotFoundIcon;

    static IconManager()
    {
        _textureNotFoundIcon = new Texture(Utils.GetBaseEngineDir() + "\\Images\\Icons\\not-found-icon.jpg", "", false,
            TextureMinFilter.Linear, TextureMagFilter.Linear);

        var path = Utils.GetBaseEngineDir() + "\\Images\\Icons\\";

        GetFileNames(new DirectoryInfo(path), true);
    }

    private static void GetFileNames(DirectoryInfo dir, bool recursive)
    {
        foreach (var fInfo in dir.GetFiles())
            if (fInfo.Name.Contains("icon") && (fInfo.Extension == ".png" || fInfo.Extension == ".jpg"))
                AddIcons(fInfo);

        if (recursive)
            foreach (var childDir in dir.GetDirectories())
                GetFileNames(childDir, recursive);
    }

    private static void AddIcons(FileInfo fInfo)
    {
        var name = Path.GetFileNameWithoutExtension(fInfo.FullName);

        if (_icons.ContainsKey(name))
        {
            Log.Error("Already icon with same name, make sure there are no duplicates! Also not in sub dirs");
            return;
        }

        var tex = new Texture(fInfo.FullName, "", false, TextureMinFilter.Linear, TextureMagFilter.Linear);
        _icons.Add(name, tex);
    }

    private static void RemoveIcon()
    {
    }

    internal static Texture GetIcon(string iconName)
    {
        if (_icons.TryGetValue(iconName, out var tex))
        {
            if (tex == null) return _textureNotFoundIcon;
            return tex;
        }

        return _textureNotFoundIcon;
    }
}