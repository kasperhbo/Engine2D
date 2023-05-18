using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleGame.Registers
{
    internal class CustomDeserializer
    {
        internal static void Deserialize()
        {
            Console.WriteLine("Deserializing from main");
            JObject jo = ComponentSerializer.currentType;
            JsonSerializerSettings? _specifiedSubclassConversion = ComponentSerializer.specifiedSubclassConversion;

            switch (jo["Type"].Value<string>())
            {
                case "Component":
                    break;
                case "TestComponent":
                    ComponentSerializer.obj = JsonConvert.DeserializeObject<TestComponent>(jo.ToString(), _specifiedSubclassConversion);
                    break;
                case "SpriteRenderer":
                    ComponentSerializer.obj = JsonConvert.DeserializeObject<SpriteRenderer>(jo.ToString(), _specifiedSubclassConversion);
                    break;
                case "Rigidbody":
                    ComponentSerializer.obj = JsonConvert.DeserializeObject<RigidBody>(jo.ToString(), _specifiedSubclassConversion);
                    break;
                case "BoxCollider2D":
                    ComponentSerializer.obj = JsonConvert.DeserializeObject<BoxCollider2D>(jo.ToString(), _specifiedSubclassConversion);
                    break;
                case "TextureData":
                    ComponentSerializer.obj = JsonConvert.DeserializeObject<TextureData>(jo.ToString(), _specifiedSubclassConversion);
                    break;
                case "ScriptHolderComponent":
                    ComponentSerializer.obj = JsonConvert.DeserializeObject<ScriptHolderComponent>(jo.ToString(), _specifiedSubclassConversion);
                    break;
                case "NewComponent":
                    ComponentSerializer.obj = JsonConvert.DeserializeObject<NewComponent>(jo.ToString(), _specifiedSubclassConversion);
                    break;
                //LAST LINE
                default:
                    ComponentSerializer.obj = null;
                    break;
            }
        }
    }
}
