using ImGuiNET;
using Newtonsoft.Json;

namespace Engine2D.Components.ENTT;

    internal struct ENTTTagComponent : IENTTComponent
    {
        [JsonProperty]internal string Tag = "";

        internal ENTTTagComponent(string tag)
        {
            Tag = tag;
        }

        public void OnGui()
        {
            ImGui.Text("Tag: " + Tag);
        }
    };

