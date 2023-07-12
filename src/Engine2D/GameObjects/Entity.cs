using Engine2D.Components.ENTT;
using Engine2D.Core;
using Engine2D.Managers;
using Engine2D.Rendering.NewRenderer;
using Engine2D.Scenes;
using Engine2D.UI.ImGuiExtension;
using EnTTSharp.Entities;
using ImGuiNET;
using Newtonsoft.Json;
using Serilog;
using Vortice.DXGI;

public class Entity : Asset
{
    //Serialized
    [JsonProperty]public int UUID = -1;
    
    //Runtime
    [JsonIgnore]public EntityKey m_EntityHandle;
    [JsonIgnore]public bool IsDirty = true;
    
    [JsonIgnore]private Scene m_Scene = null;

    public Entity(EntityKey handle, Scene scene, int uuid)
    {
        m_EntityHandle = handle;
        m_Scene = scene;
        UUID = uuid;
        
        Init();
    }


    private void Init()
    {
        if (UUID == -1)
        {
            UUID = UIDManager.GetUID();
        }
        else
        {
            UIDManager.TakenUids.Add(UUID);
        }
    }

    public T? AddComponent<T>(T? component)
    {
        if (m_Scene == null)
        {
            m_Scene = Engine.Get().CurrentScene;
        }
        
        if (HasComponent<T>())
        {
            Log.Warning("Entity already has component!");
            return GetComponent<T>();
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
           Log.Warning("Entity does not have component!");
           return default;
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
            return;
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
                var comp = new ENTTSpriteRenderer(this.UUID);
                AddComponent(comp);
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
            ImGui.PushID("##tagcomponent");
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
            ImGui.PopID();
        }
        
        //Transform component  
        if (HasComponent<ENTTTransformComponent>())
        {
            ImGui.PushID("##transformcomponent");
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
            ImGui.PopID();
        }
        
        //Sprite Renderer component
        if (HasComponent<ENTTSpriteRenderer>())
        {
            ImGui.PushID("##spriterenderercomponent");
            var spriteRenderer = GetComponent<ENTTSpriteRenderer>();
            Gui.DrawTable("Sprite Renderer", () =>
            {
                //Just for testing if parenting is working
                Gui.DrawProperty("Parent UUID: " + spriteRenderer.Parent?.UUID.ToString());
                
                if (Gui.DrawProperty("Color", ref spriteRenderer.Color, isColor:true))
                {
                    SetComponent(spriteRenderer);
                }
                
            });
            ImGui.Separator();
            ImGui.PopID();
        }

    }

}
