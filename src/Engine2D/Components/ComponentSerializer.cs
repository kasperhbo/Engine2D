using Engine2D.GameObjects;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine2D.Core;
using Box2DSharp.Common;
using System.Reflection;

namespace Engine2D.Components
{
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
                    typeof(Gameobject).IsAssignableFrom(objectType) ||
                    typeof(TextureData).IsAssignableFrom(objectType))
                    && !objectType.IsAbstract)
                    
            {
                return null;
            }
            return base.ResolveContractConverter(objectType);
        }
    }

    public class ComponentSerializer : JsonConverter
    {
        static JsonSerializerSettings _specifiedSubclassConversion =
            new JsonSerializerSettings()
            {
                ContractResolver = new BaseSpecifiedConcreteClassConverter()
            };

        public override bool CanConvert(Type objectType)
        {
            return 
                objectType == typeof(Component)||
                //objectType == typeof(SpriteRenderer)||
                //objectType == typeof(RigidBody) ||
                //objectType == typeof(BoxCollider2D) ||
                //objectType == typeof(TextureData) ||
                //objectType == typeof(ScriptHolderComponent) ||
                objectType == typeof(Gameobject) ||
                objectType == typeof(TextureData);
        }

        static Action actions;

        public static void AddAction(Action action)
        {
            actions = action;
        }
        
        public static JObject currentType = null;
        public static JsonSerializerSettings specifiedSubclassConversion = null;
        public static object obj = null;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            currentType = jo;
            specifiedSubclassConversion = _specifiedSubclassConversion;
            
            actions.Invoke();
            
            if (obj != null) return obj;

             //return (object)info.ReturnParameter;

            //return (object)actions.Invoke();

            //Component instance = (Component)Activator.CreateInstance(ComponentRegistry.Get(jo["Type"].Value<string>()));

            //return JsonConvert.DeserializeObject<>(jo.ToString(), _specifiedSubclassConversion);

            switch (jo["Type"].Value<string>())
            {
                case "Component":
                    Logging.Log.Error("is component");
                    return null;
                case "SpriteRenderer":
                    return JsonConvert.DeserializeObject<SpriteRenderer>(jo.ToString(), _specifiedSubclassConversion);
                case "Rigidbody":
                    return JsonConvert.DeserializeObject<RigidBody>(jo.ToString(), _specifiedSubclassConversion);
                case "BoxCollider2D":
                    return JsonConvert.DeserializeObject<BoxCollider2D>(jo.ToString(), _specifiedSubclassConversion);
                case "TextureData":
                    return JsonConvert.DeserializeObject<TextureData>(jo.ToString(), _specifiedSubclassConversion);
                case "ScriptHolderComponent":
                    return JsonConvert.DeserializeObject<ScriptHolderComponent>(jo.ToString(), _specifiedSubclassConversion);
                default:
                    {
                        Logging.Log.Error("Not Found" + jo["Type"].Value<string>());
                        return null;
                    }
                    //}
            }
        }
        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // won't be called because CanWrite returns false
        }



    }
}
