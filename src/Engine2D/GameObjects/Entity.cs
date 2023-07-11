using Engine2D.Components.ENTT;
using Engine2D.Core;
using Engine2D.Managers;
using Engine2D.Scenes;
using Engine2D.UI.ImGuiExtension;
using EnTTSharp.Entities;
using ImGuiNET;

public class Entity : Asset
{
    public EntityKey m_EntityHandle;
    public int UUID = -1;

    private Scene m_Scene = null;

    public Entity()
    {
        Init();
    }

    public Entity(EntityKey handle, Scene scene)
    {
        m_Scene = scene;
        m_EntityHandle = handle;
        Init();
    }

    private void Init()
    {
        if (UUID == -1)
        {
            UUID = UIDManager.GetUID();
        }
    }

    public T AddComponent<T>(T component)
    {
        if (HasComponent<T>())
        {
            throw
                new Exception
                    ("Entity already has component!");
        }

        m_Scene.EntityRegistry.AssignComponent(m_EntityHandle, component);
        return component;
    }


    public void SetComponent<T>(T component)
    {

        RemoveComponent<T>();

        m_Scene.EntityRegistry.AssignComponent(m_EntityHandle, component);
    }


    public T? GetComponent<T>()
    {
        if (!HasComponent<T>())
        {
            throw new Exception
                ("Entity does not have component!");
        }

        m_Scene.EntityRegistry.GetComponent<T>(m_EntityHandle, out T? component);

        return component;
    }

    public bool HasComponent<T>()
    {
        return m_Scene.EntityRegistry.HasComponent<T>(m_EntityHandle);
    }

    public void RemoveComponent<T>()
    {
        if (!HasComponent<T>())
        {
            throw new Exception("Entity does not have component!");
        }

        m_Scene.EntityRegistry.RemoveComponent<T>(m_EntityHandle);
    }

    internal override void OnGui()
    {
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.Button("Add Component"))
        {
            ImGui.OpenPopup("Add Component");
        }
        
        if (ImGui.BeginPopup("Add Component"))
        {
            if (ImGui.MenuItem("Sprite Renderer"))
            {
                AddComponent(new ENTTSpriteRenderer());
                ImGui.CloseCurrentPopup();
            }
            
            ImGui.EndPopup();
        }
        
        ImGui.Text($"UUID: {UUID}");
        ImGui.Text($"Entity Handle: {m_EntityHandle}");
      
        ImGui.Separator();
        //Tag component ui
        if (HasComponent<ENTTTagComponent>())
        {
            var tag = GetComponent<ENTTTagComponent>();
            Gui.DrawTable("Tag", () =>
            {
                if(Gui.DrawProperty("Tag", ref tag.Tag)){
                    SetComponent(tag);
                }
                // ImGui.Text("Tag: ");
                // ImGui.SameLine();
                // if(ImGui.InputText("##TagCompo", ref tag.Tag, 64)){
                //     SetComponent(tag);
                // }
            });
            ImGui.Separator();
        }
        
        //Transform component  
        if (HasComponent<ENTTTransformComponent>())
        {
            var transform = GetComponent<ENTTTransformComponent>();
            Gui.DrawTable("Transform", () =>
            {
                if(Gui.DrawProperty("Position", ref transform.Position)){
                    SetComponent(transform);
                }
                if(Gui.DrawProperty("Rotation", ref transform.Rotation)){
                    SetComponent(transform);
                }
                if(Gui.DrawProperty("Scale", ref transform.Scale)){
                    SetComponent(transform);
                }
            });

            ImGui.Separator();
        }

    }

}
