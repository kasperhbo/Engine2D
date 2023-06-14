using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Soap;
using Engine2D.Logging;
using ImGuiNET;
using Microsoft.Win32;

namespace Engine2D.Core;

public static class Utils
{
    internal static string GetBaseEngineDir()
    {
        var engineDir = Environment.GetEnvironmentVariable("KDBENGINE_DIR");

        if (engineDir == null) throw new Exception("Engine Directory Not Set!");
        return engineDir;
    }

    internal static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        var dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (var file in dir.GetFiles())
        {
            var targetFilePath = Path.Combine(destinationDir, file.Name);
            
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
            foreach (var subDir in dirs)
            {
                var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
    }

    internal static bool TryOpenUrl(string p_url)
    {
        // try use default browser [registry: HKEY_CURRENT_USER\Software\Classes\http\shell\open\command]
        try
        {
            var keyValue =
                Registry.GetValue(@"HKEY_CURRENT_USER\Software\Classes\http\shell\open\command", "", null) as string;
            if (string.IsNullOrEmpty(keyValue) == false)
            {
                var browserPath = keyValue.Replace("%1", p_url);
                Process.Start(browserPath);
                return true;
            }
        }
        catch
        {
        }

        // try open browser as default command
        try
        {
            Process.Start(p_url); //browserPath, argUrl);
            return true;
        }
        catch
        {
        }

        // try open through 'explorer.exe'
        try
        {
            var browserPath = GetWindowsPath("explorer.exe");
            var argUrl = "\"" + p_url + "\"";

            Process.Start(browserPath, argUrl);
            return true;
        }
        catch
        {
        }

        // return false, all failed
        return false;
    }

    internal static string GetWindowsPath(string p_fileName)
    {
        string path = null;
        string sysdir;

        for (var i = 0; i < 3; i++)
            try
            {
                if (i == 0)
                {
                    path = Environment.GetEnvironmentVariable("SystemRoot");
                }
                else if (i == 1)
                {
                    path = Environment.GetEnvironmentVariable("windir");
                }
                else if (i == 2)
                {
                    sysdir = Environment.GetFolderPath(Environment.SpecialFolder.System);
                    path = Directory.GetParent(sysdir).FullName;
                }

                if (path != null)
                {
                    path = Path.Combine(path, p_fileName);
                    if (File.Exists(path)) return path;
                }
            }
            catch
            {
            }

        // not found
        return null;
    }

    public static bool SaveWithSoapStaticClass(Type static_class, string filename)
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

    public static bool LoadWithSoapStaticClass(Type static_class, string filename)
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

    public static unsafe bool IsValidPayload(this ImGuiPayloadPtr payload)
    {
        return payload.NativePtr != null;
    }

    public static string[] GetAllScriptFiles()
    {
        return Directory.GetFiles(ProjectSettings.FullProjectPath, "*.cs", SearchOption.AllDirectories);
    }

    public static string GetFilePath(string file)
    {
        var res = Directory.GetFiles(ProjectSettings.FullProjectPath, file, SearchOption.AllDirectories);
        if (res.Length == 0)
        {
            Log.Error(file + " not found");
            return null;
        }

        var path = res[0];
        return path;
    }

    public static void CreateEntry(string fileName, string lineToWriteTo, string lineToAdd) //npcName = "item1"
    {
        var endTag = string.Format("{0}", lineToWriteTo);

        var txtLines = File.ReadAllLines(fileName).ToList(); //Fill a list with the lines from the txt file.

        txtLines.Insert(txtLines.IndexOf(endTag),
            lineToAdd); //Insert the line you want to add last under the tag 'item1'.

        File.WriteAllLines(fileName, txtLines); //Add the lines including the new one.
    }
    
    
}