namespace Engine2D.Utilities;

public static class FileUtils
{
    public static FileInfo GetFileInfo(string path)
    {
        return new FileInfo(path);
    }

    public static List<FileInfo> GetFileInfos(string[] paths)
    {
        List<FileInfo> files = new List<FileInfo>();

        for (int i = 0; i < paths.Length; i++)
        {
            files.Add(new FileInfo(paths[i]));
        }

        return files;
    }
}