#region

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace Engine2D.Components;

public static class ComponentGameSerializer
{
    public static object? GetCustomGameComponent(JObject jo, JsonSerializerSettings? converters)
    {
        return null;
    }
}