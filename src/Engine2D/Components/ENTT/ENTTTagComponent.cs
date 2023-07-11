using ImGuiNET;

namespace Engine2D.Components.ENTT;

    internal struct ENTTTagComponent : IENTTComponent
    {
        internal string Tag = "";

        internal ENTTTagComponent(string tag)
        {
            Tag = tag;
        }

        public void OnGui()
        {
            ImGui.Text("Tag: " + Tag);
        }
    };

