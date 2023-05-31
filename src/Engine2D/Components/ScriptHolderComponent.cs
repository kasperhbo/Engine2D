using Engine2D.GameObjects;
using Engine2D.Rendering;
using ImGuiNET;
using Newtonsoft.Json;

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
public class ScriptHolderComponent : Component
{
    //public string refFile = "";
    public Component component = null;

    public override void Init(Gameobject parent, Renderer? renderer)
    {
        base.Init(parent, renderer);
    }

    public override void ImGuiFields()
    {
        base.ImGuiFields();
        float f = 0;

        // if (component != null) f = f + component.ImGuiFields();

        sizeYGUI += f;
        ImGui.TableNextColumn();
        //ImGui.ImageButton("", IntPtr.Zero, new System.Numerics.Vector2(56, 56));
        throw new NotImplementedException();
    }

    public override string GetItemType()
    {
        return "ScriptHolderComponent";
    }
}