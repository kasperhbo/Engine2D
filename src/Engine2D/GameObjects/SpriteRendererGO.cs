using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.UI.ImGuiExtension;
using Engine2D.Utilities;
using ImGuiNET;

namespace Engine2D.GameObjects;

public class SpriteRendererGO : Gameobject
{
    public SpriteRendererGO(string name) : base(name)
    {
        var currentScene = Engine.Get().CurrentScene;
        SpriteRenderer spr = new SpriteRenderer();
        spr.Parent = this;
        components.Add(spr);
        
        // components.Add(AssemblyUtils.GetComponent("ExampleGame.Assets.TestClass"));
        
        if (currentScene != null)
        {
            Name = "SpriteRenderer: " + currentScene.GameObjects.Count + 1;
        }

    }

    public override void OnGui()
    {
        // base.OnGui();
        // Console.WriteLine("OnGui");
        ImGui.InputText("##name", ref Name, 256);
        ImGui.SameLine();
        ImGui.Text(" UID: " + UID);
        ImGui.Separator();

        OpenTkuiHelper.DrawComponentWindow("Transform", "Transform",
            () =>
            {
                GetComponent<Transform>()?.ImGuiFields();
                
            }, GetComponent<Transform>().GetFieldSize()
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