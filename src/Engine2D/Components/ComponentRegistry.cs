using Engine2D.Logging;

namespace Engine2D.Components;

public class ComponentRegistry
{
    private static readonly Dictionary<string, Type> types = new();

    public static void Register(string path, Type type)
    {
        types.Add(path, type);
    }

    public static Type? Get(string path)
    {
        if (!types.TryGetValue(path, out var type))
        {
            Log.Error(path + " not found in component registry make sure to add it on boot");
            return null;
        }

        return type;
    }
}