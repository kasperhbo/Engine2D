using System.Net.Mime;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Rendering;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.Managers;

public static class IconManager
{
    //name of icons is: filename - extension so: \Images\Icons\file-icon.png = file-icon
    //file names must contain the word icon
    private static Dictionary<string, Texture> _icons = new Dictionary<string, Texture>();

    private static Texture _textureNotFoundIcon;
    
    static IconManager()
    {
        _textureNotFoundIcon = new Texture(Utils.GetBaseEngineDir() + "\\Images\\Icons\\not-found-icon.jpg", false,
            TextureMinFilter.Linear, TextureMagFilter.Linear);
        
        string path = Utils.GetBaseEngineDir() + "\\Images\\Icons\\";
        
        GetFileNames(new DirectoryInfo(path), true);
    }

    private static void GetFileNames(DirectoryInfo dir, bool recursive)
    {
        foreach (var fInfo in dir.GetFiles())
        {
            if (fInfo.Name.Contains("icon") && (fInfo.Extension == ".png" || fInfo.Extension == ".jfp"))
            {
                AddIcons(fInfo);
            }
        }

        if(recursive)
        {
            foreach (var childDir in dir.GetDirectories())
            {
                GetFileNames(childDir, recursive);
            }
        }
    }
    
    private static void AddIcons(FileInfo fInfo)
    {
        Texture tex = new Texture(fInfo.FullName, false, TextureMinFilter.Linear, TextureMagFilter.Linear);
        var name = Path.GetFileNameWithoutExtension(fInfo.FullName);

        if (_icons.ContainsKey(name))
        {
            Log.Error("Already icon with same name, make sure there are no duplicates! Also not in sub dirs");
            return;
        }
        _icons.Add(name, tex);
    }

    private static void RemoveIcon()
    {
        
    }

    public static Texture GetIcon(string iconName)
    {
        if (_icons.TryGetValue(iconName, out Texture tex))
        {
            if (tex == null) return _textureNotFoundIcon;
            return tex;
        }
        else
            return _textureNotFoundIcon;
    }
}