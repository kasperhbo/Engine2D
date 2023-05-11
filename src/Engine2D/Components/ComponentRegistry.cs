using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Components
{
    public class ComponentRegistry
    {
        private static Dictionary<string, Type> types = new Dictionary<string, Type>();

        public static void Register(string path, Type type)
        {
            types.Add(path, (Type)type);
        }

        public static Type? Get(string path)
        {
            if (!types.TryGetValue(path, out var type)) {
                Logging.Log.Error(path + " not found in component registry make sure to add it on boot");
                return null;
            }
            else return type;

        }
    }
}
