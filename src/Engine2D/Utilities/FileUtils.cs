namespace Engine2D.Utilities;

internal static class FileUtils
{
    internal static FileInfo GetFileInfo(string path)
    {
        return new FileInfo(path);
    }

    internal static List<FileInfo> GetFileInfos(string[] paths)
    {
        var files = new List<FileInfo>();

        for (var i = 0; i < paths.Length; i++) files.Add(new FileInfo(paths[i]));

        return files;
    }
}