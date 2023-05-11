using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using ExampleGame;
using KDBEngine.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Program
{
    static void Main()
    {
        ComponentSerializer.AddAction(() => { Deserializer(); });
        ComponentRegistry.Register("D:\\dev\\EngineDev\\Engine2D\\src\\ExampleGame\\TestComponent.cs", typeof(TestComponent));
        Console.WriteLine(Utils.GetAllScriptFiles());
        Settings.s_IsEngine = true;        
        Engine.Get().Run();
    }
    

    static void Deserializer()
    {
        Console.WriteLine("Deserializing from main");
        JObject jo = ComponentSerializer.currentType;
        JsonSerializerSettings _specifiedSubclassConversion = ComponentSerializer.specifiedSubclassConversion;

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
            default:
                ComponentSerializer.obj = null;
                break;
        }
    }
}