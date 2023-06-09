﻿using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Engine2D.Components.ENTT;
using Engine2D.Components.Sprites;
using Engine2D.Core;
using Engine2D.Managers;
using Engine2D.Rendering;
using Engine2D.Rendering.NewRenderer;
using Engine2D.SavingLoading;
using Engine2D.Scenes;
using Engine2D.UI.ImGuiExtension;
using EnTTSharp.Entities;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;
using Serilog;
using Vortice.DXGI;

public class Entity : Asset
{
    //Serialized
    [JsonProperty]public int UUID = -1;
    [JsonProperty]public bool IsStatic = true;
    
    //Runtime
    [JsonIgnore]public EntityKey m_EntityHandle;
    [JsonIgnore]public bool IsDirty = true;
    
    [JsonIgnore]private Scene m_Scene = null;
    [JsonIgnore]private Vector2 _lastPosition = Vector2.Zero;
    
    [JsonIgnore]private Vector2 _lastScale = Vector2.One;
    [JsonIgnore]private Quaternion _lastRotation = new();

    [JsonIgnore] public int AddedToBatch = -1;
    
    
    public Entity(EntityKey handle, Scene scene, int uuid, bool isStatic = false)
    {
        m_EntityHandle = handle;
        m_Scene = scene;
        UUID = uuid;
        IsStatic = isStatic;
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

    /// <summary>
    /// Runs every frame, not depended on play mode or not
    /// <param name="dt">delta time</param>
    /// </summary>
    public void Update(double dt)
    {
        if(IsDead)
        {
            Engine.Get().CurrentScene.RemoveEntity(this);
            Engine.Get().CurrentSelectedAsset = null;
        }
        
        CheckForDirty();
    }

    /// <summary>
    /// Check if location changed since last frame
    /// Only used for non static entities
    /// </summary>
    [JsonIgnore] private bool _firstRun = true;
    private void CheckForDirty()
    {
        if (_firstRun)
        {
            _firstRun = false;
            
            IsDirty = true;
        }
        // if (IsStatic) return;
        var pos = this.GetComponent<ENTTTransformComponent>().Position;
        if(pos.X != _lastPosition.X || pos.Y != _lastPosition.Y)
        {
            IsDirty = true;
            _lastPosition = this.GetComponent<ENTTTransformComponent>().Position;
        }
        if(this.GetComponent<ENTTTransformComponent>().Rotation != _lastRotation)
        {
            IsDirty = true;
            _lastRotation = this.GetComponent<ENTTTransformComponent>().Rotation;
        }
        if(this.GetComponent<ENTTTransformComponent>().Scale != _lastScale)
        {
            IsDirty = true;
            _lastScale = this.GetComponent<ENTTTransformComponent>().Scale;
        }
    }

    
    public void AddComponent<T>(T component)
    {
        if (m_Scene == null)
        {
            m_Scene = Engine.Get().CurrentScene;
        }
        
        if (HasComponent<T>())
        {
            Log.Warning("Entity already has component!");
            return;
        }

        if (component is ENTTSpriteRenderer spriteRenderer)
        {
            spriteRenderer.ParentUUID = this.UUID;
          
            if (spriteRenderer.SpritePath != "" && spriteRenderer.SpritePath != null)
            {
                var sprite = ResourceManager.GetItem<Sprite>(spriteRenderer.SpritePath);
                if(sprite != null)
                {
                    spriteRenderer.Sprite = sprite;
                }else
                    Log.Error(String.Format("Sprite {0} is not found in the resource manager", spriteRenderer.SpritePath));
            }
            
            m_Scene.EntityRegistry.AssignComponent(m_EntityHandle, spriteRenderer);
            Renderer.AddSprite(this);
        }
        else
        {
            m_Scene.EntityRegistry.AssignComponent(m_EntityHandle, component);
        }
        
        IsDirty = true;
    }


    public void SetComponent<T>(T component)
    {
        IsDirty = true;
        
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
                ENTTSpriteRenderer comp = new ENTTSpriteRenderer();
                  
                AddComponent(comp);
                
                ImGui.CloseCurrentPopup();
                
            }
            
            ImGui.EndPopup();
        }
        
        ImGui.Text($"Added to batch: {AddedToBatch}");
        ImGui.Text($"UUID: {UUID}");
        ImGui.Text($"Entity Handle: {m_EntityHandle}");
        ImGui.Text("IsStatic: ");
        
        ImGui.SameLine();
        ImGui.Checkbox("##IsStatic", ref IsStatic);

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
            });
            ImGui.PopID();
        }
        
        //Transform component  
        if (HasComponent<ENTTTransformComponent>())
        {
            ImGui.PushID("##transformcomponent");

            var transform = GetComponent<ENTTTransformComponent>();
            Gui.DrawTable("Transform", () =>
            {
                if(Gui.DrawProperty("Position", ref transform.Position))
                {
                    SetComponent(transform);
                }
                
                // if(Gui.DrawProperty("Rotation", ref transform.Rotation)){
                //     SetComponent(transform);
                // }
                if(Gui.DrawProperty("Scale", ref transform.Scale)){
                    {
                        SetComponent(transform);
                    }
                }
            });

            ImGui.PopID();
        }
        
        //Sprite Renderer component
        if (HasComponent<ENTTSpriteRenderer>())
        {
            ImGui.PushID("##spriterenderercomponent" + UUID);
            
            var spriteRenderer = GetComponent<ENTTSpriteRenderer>();
            
            Gui.DrawTable("Sprite Renderer", () =>
            {
                //TODO: REMOVE THIS THIS IS FOR DEBUGGING
                Gui.DrawProperty("texture id: " + spriteRenderer.Sprite?.Texture?.TexID);
                // Gui.DrawProperty("Parent UUID: " + spriteRenderer.Parent?.UUID.ToString());
                
                
                var tex = spriteRenderer.Sprite?.Texture;
                
                if (tex == null)
                    ImGui.Button("Sprite", new Vector2(64, 64));
                else
                    ImGui.ImageButton((IntPtr)tex.TexID, new Vector2(64, 64), new Vector2(0, 1), new Vector2(1, 0));
          
                if (ImGui.BeginDragDropTarget())
                {
                    var payload = ImGui.AcceptDragDropPayload("sprite_drop");
                    if (payload.IsValidPayload())
                    {
                        var filename = (string)GCHandle.FromIntPtr(payload.Data).Target;
                    
                        var sprite = ResourceManager.GetItem<Sprite>(filename);
                        if (sprite != null)
                        {
                            spriteRenderer.Sprite = sprite;
                            SetComponent(spriteRenderer);
                        }
                        else
                        {
                            Log.Error("Something went wrong when trying to load sprite " + filename);
                        }
                    }

                    ImGui.EndDragDropTarget();
                }
                
                
                if (Gui.DrawProperty("Color", ref spriteRenderer.Color, isColor:true))
                {
                    SetComponent(spriteRenderer);
                }
                
            });
            ImGui.PopID();
        }

    }
    
}
