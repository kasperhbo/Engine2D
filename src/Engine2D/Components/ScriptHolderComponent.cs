#region

using Engine2D.GameObjects;
using Engine2D.Rendering;
using ImGuiNET;
using Newtonsoft.Json;

#endregion

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
internal class ScriptHolderComponent : Component
{
    //internal string refFile = "";
    [JsonProperty]internal Component component = null;

    internal override void Init(Gameobject parent, Renderer? renderer)
    {
        base.Init(parent, renderer);
    }
    
    public override string GetItemType()
    {
        return this.GetType().FullName;
    }
}