using Engine2D.GameObjects;
using ImGuiNET;
using Newtonsoft.Json;

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
public class ScriptHolderComponent : Component
{
    //public string refFile = "";
    public Component component = null;

    public override void Init(GameObject parent)
    {
        base.Init(parent);
    }

    public override float ImGuiFields()
    {
        base.ImGuiFields();
        float f = 0;

        if (component != null) f = f + component.ImGuiFields();

        sizeYGUI += f;
        ImGui.TableNextColumn();
        //ImGui.ImageButton("", IntPtr.Zero, new System.Numerics.Vector2(56, 56));

        return sizeYGUI;
    }

    public override string GetItemType()
    {
        return "ScriptHolderComponent";
    }
}