#region

using Engine2D.Cameras;
using Engine2D.Components.TransformComponents;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

#endregion

namespace Engine2D.Components;

public class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
{
    protected override JsonConverter? ResolveContractConverter(Type objectType)
    {
        if ((
                typeof(Component).IsAssignableFrom(objectType) ||
                //typeof(SpriteRenderer).IsAssignableFrom(objectType) ||
                //typeof(RigidBody).IsAssignableFrom(objectType) ||
                //typeof(BoxCollider2D).IsAssignableFrom(objectType) ||
                //typeof(TextureData).IsAssignableFrom(objectType) ||
                //typeof(ScriptHolderComponent).IsAssignableFrom(objectType) ||
                typeof(Gameobject).IsAssignableFrom(objectType))
            && !objectType.IsAbstract)
            return null;
        return base.ResolveContractConverter(objectType);
    }
}

public class ComponentSerializer : JsonConverter
{
    private static readonly JsonSerializerSettings? _specifiedSubclassConversion =
        new()
        {
            ContractResolver = new BaseSpecifiedConcreteClassConverter()
        };


    public static JObject currentType;
    public static JsonSerializerSettings? specifiedSubclassConversion;

    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType)
    {
        return
            objectType == typeof(Component) ||
            objectType == typeof(Gameobject);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        var jo = JObject.Load(reader);
        currentType = jo;
        specifiedSubclassConversion = _specifiedSubclassConversion;

        string componentType = jo["Type"].Value<string>();
        
        string dynamicClassName = (componentType);
        Type dynamicType = Type.GetType(dynamicClassName);
        
        //Try converting to a component type that is in the Engine assembly
        if (dynamicType != null)
        {
            return JsonConvert.DeserializeObject(jo.ToString(), dynamicType, _specifiedSubclassConversion);
        }

        //Try converting to a component type that is in the Game assembly
        if (AssemblyUtils.GetComponent(componentType) != null)
        {
            return JsonConvert.DeserializeObject(jo.ToString(), AssemblyUtils.GetComponent(componentType).GetType(),
                _specifiedSubclassConversion);
        }
        
        //If all else fails, log an error and return null
        Log.Error($"Unknown component type: {componentType}");
        
        
        
        return null;
        
        switch (jo["Type"].Value<string>())
        {
            // case "Component":
            //     Log.Error("is component");
            //     return null;
            // case "SpriteRenderer":
            //     return JsonConvert.DeserializeObject<SpriteRenderer>(jo.ToString(), _specifiedSubclassConversion);
            // case "Rigidbody":
            //     return JsonConvert.DeserializeObject<RigidBody>(jo.ToString(), _specifiedSubclassConversion);
            // case "BoxCollider2D":
            //     return JsonConvert.DeserializeObject<BoxCollider2D>(jo.ToString(), _specifiedSubclassConversion);
            // case "PointLight":
            //     return JsonConvert.DeserializeObject<PointLightComponent>(jo.ToString(), _specifiedSubclassConversion);
            // case "GlobalLight":
            //     return JsonConvert.DeserializeObject<GlobalLight>(jo.ToString(), _specifiedSubclassConversion);
            // case "ScriptHolderComponent":
            //     return JsonConvert.DeserializeObject<ScriptHolderComponent>(jo.ToString(),
            //         _specifiedSubclassConversion);
            // case "Transform":
            //     return JsonConvert.DeserializeObject<Transform>(jo.ToString(),
            //         _specifiedSubclassConversion);
            // case "Camera":
            //     return JsonConvert.DeserializeObject<Camera>(jo.ToString(),
            //         _specifiedSubclassConversion);
            // case "Texture":
            //     return JsonConvert.DeserializeObject<Texture>(jo.ToString(),
            //         _specifiedSubclassConversion);
            // default:
            // {
            //     Log.Error("Not Found" + jo["Type"].Value<string>());
            //     return null;
            // }
        }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException(); // won't be called because CanWrite returns false
    }
    
}