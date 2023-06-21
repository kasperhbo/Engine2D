#region

using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;

#endregion

namespace Engine2D.GameObjects;

internal class SpriteRendererGo : Gameobject
{
    internal SpriteRendererGo(string name) : base(name)
    {
        var currentScene = Engine.Get().CurrentScene;
        var spr = new SpriteRenderer();
        spr.Parent = this;
        components.Add(spr);

        if (currentScene != null) Name = "SpriteRenderer: " + currentScene.GameObjects.Count + 1;
    }

    internal override void OnGui()
    {
        ImGui.InputText("##name", ref Name, 256);
        ImGui.SameLine();
        ImGui.Text(" UID: " + UID);
        ImGui.Separator();

        OpenTkuiHelper.DrawComponentWindow("Transform", "Transform",
            () => { GetComponent<Transform>()?.ImGuiFields(); }, GetComponent<Transform>().GetFieldSize()
        );

        for (var i = 0; i < components.Count; i++)
        {
            if (components[i].GetType() == typeof(Transform)) return;

            ImGui.PushID(i);

            OpenTkuiHelper.DrawComponentWindow(i.ToString(), components[i].GetItemType(),
                () => { components[i].ImGuiFields(); }, components[i].GetFieldSize()
            );

            ImGui.PopID();
        }
    }
}