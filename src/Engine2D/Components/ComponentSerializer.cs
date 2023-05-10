using Engine2D.GameObjects;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Components
{
    public class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
    {
        protected override JsonConverter? ResolveContractConverter(Type objectType)
        {
            if ((
                    typeof(Component).IsAssignableFrom(objectType) ||
                    typeof(Gameobject).IsAssignableFrom(objectType))

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
            return objectType == typeof(Component) || objectType == typeof(Gameobject);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            switch (jo["Type"].Value<string>())
            {
                case "Component":
                    return null;

                case "SpriteRenderer":
                    return JsonConvert.DeserializeObject<SpriteRenderer>(jo.ToString(), _specifiedSubclassConversion);

                case "Rigidbody":
                    return JsonConvert.DeserializeObject<RigidBody>(jo.ToString(), _specifiedSubclassConversion);

                case "BoxCollider2D":
                    return JsonConvert.DeserializeObject<BoxCollider2D>(jo.ToString(), _specifiedSubclassConversion);

                default:
                    throw new Exception();
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
