﻿#region

using System.Numerics;
using Engine2D.Components;
using Engine2D.Components.SpriteAnimations;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.Physics;
using Engine2D.Rendering;
using Engine2D.UI.ImGuiExtension;
using Engine2D.Utilities;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Windowing.Common;

#endregion

namespace Engine2D.GameObjects;

public class Gameobject : Asset, ICloneable
{
    [JsonProperty] internal string Name = "";
    [JsonProperty] internal bool CanBeSelected = true;
    [JsonProperty] internal List<Component> Components = new();
    [JsonProperty] internal int UID = -1;
    [JsonProperty] internal int ParentUid = -1;
    
    [JsonIgnore] internal List<int> Children = new();
    [JsonIgnore] internal bool Serialize = true;
    [JsonIgnore] bool _isPopupOpen = false;
    [JsonIgnore]   public Transform Transform => GetComponent<Transform>();
    
    public Gameobject(string name)
    {
        Name = name;
        Components = new List<Component?>();
        GetUID();
    }

    public Gameobject(string name, List<Component?> components)
    {
        Name = name;
        Components = components;
        GetUID();
    }

    [JsonConstructor]
    internal Gameobject(string name, List<Component?> components, int uid, int parentUid)
    {
        Name = name;
        Components = components;
        UID = uid;
        ParentUid = parentUid;
    }

    private void GetUID()
    {
        if (UID == -1)
        {
            UID = UIDManager.GetUID();
        }
        else
        {
            UIDManager.TakenUids.Add(UID);
        }
    }


    internal void Init(Renderer? renderer)
    {
        GetUID();
        if (GetComponent<Transform>() == null)
        {
            var t = new Transform();

            t.Position = new Vector2(0, 0);
            AddComponent(t);
        }
        
        foreach (var component in Components) component.Init(this);
    }

    internal void Start()
    {
        if(ParentUid != -1)SetParent(ParentUid);
        foreach (var component in Components) component.Start();
    }
    
    internal virtual void Update(FrameEventArgs args)
    {
        foreach (var component in Components) component.Update(args);
    }

    internal virtual void EditorUpdate(double dt)
    {
        foreach (var component in Components) component.EditorUpdate(dt);
    }

    internal void GameUpdate(double dt)
    {
        foreach (var component in Components) component.GameUpdate(dt);
    }
    
    
    internal void StartPlay(Physics2DWorld physics2DWorld)
    {
        foreach (var component in Components)
        {
            component.StartPlay();
            
            if (component is RigidBody)
            {
                physics2DWorld.AddRigidbody(component as RigidBody);
            }
        }
    }
    
    internal void StopPlay()
    {
        foreach (var component in Components)
        {
            component.StopPlay();
        }
    }

    internal void Destroy()
    {
        for (int i = 0; i < Children.Count; i++)
        {
            var child = Children[i];
            Engine.Get().CurrentScene.FindObjectByUID(child)?.Destroy();
            i--;
        }

        for (int i = 0; i < Components.Count; i++)
        {
            var comp = Components[i];
            comp?.Destroy();
            i--;
        }

        Engine.Get().CurrentScene.RemoveGameObject(this);
    }

    public Component? AddComponent(Component? component)
    {
        if (component == null) return null;
        component.Init(this);
        Components.Add(component);
        return component;
    }

    public T? GetComponent<T>() where T : Component
    {
        foreach (var component in Components)
        {
            if (typeof(T) == component.GetType())
                return
                    (component as T)!;
        }
        return null;
    }

    private void RemoveComponents(Component? comp)
    {
        Components.Remove(comp);
    }

    internal override void OnGui()
    {
        ImGui.InputText("##name", ref Name, 256);

        ImGui.SameLine();
        ImGui.Text(" UID: " + UID);
        ImGui.Separator();

        ImGui.Text("Can be selected: ");
        ImGui.SameLine();
        ImGui.Checkbox("##CanBeSelected", ref CanBeSelected);
        
        if (ImGui.Button("Add Component", new Vector2(ImGui.GetContentRegionAvail().X - 30, 30)))
        {
            _isPopupOpen = true;
        }
        
        if (_isPopupOpen)
        {
            ImGui.OpenPopup("Components Popup");
        }

        if (ImGui.BeginPopup("Components Popup"))
        {
            //List all components in a menu
            foreach (var component in AssemblyUtils.GetComponents())
            {
                if (ImGui.MenuItem(component.Name))
                {
                    AddComponent(AssemblyUtils.GetComponent(component.FullName));
                    _isPopupOpen = false;
                }
            }
            if(ImGui.MenuItem("Rigidbody"))
            {
                AddComponent(new RigidBody());
                AddComponent(new BoxCollider2D());
                _isPopupOpen = false;
            }
            
            if(ImGui.MenuItem("Sprite renderer"))
            {
                AddComponent(new SpriteRenderer());
                _isPopupOpen = false;
            }
            
            if(ImGui.MenuItem("Sprite Animator"))
            {
                AddComponent(new SpriteAnimator());
                _isPopupOpen = false;
            }
            
            ImGui.EndPopup();
        }

        if (ImGui.IsAnyMouseDown())
        {
            _isPopupOpen = false;
        }

        for (var i = 0; i < Components.Count; i++)
        {
            if (Components[i] == null)
            {
                Components.RemoveAt(i);
                continue;
            }
            
            ImGui.PushID(i);

            Gui.DrawTable(Components[i].GetItemType(), Components[i].ImGuiFields);
            
            ImGui.PopID();
        }
        

    }


    public bool AABB(float pX, float pY)
    {
        
        if (!CanBeSelected) return false;
        //Check if the point is inside the gameobject
        var transform = GetComponent<Transform>();
        if (transform == null) return false;
        var pos = transform.Position;
        var size = transform.GetFullSize(true);
        var spr = GetComponent<SpriteRenderer>();
        if (spr != null)
        {
            if (spr.Sprite != null)
            {
                size += new Vector2(spr.Sprite.Width, spr.Sprite.Height);
            }
        }
        
        var halfSize = size / 2;
        var x1 = pos.X - halfSize.X;
        var x2 = pos.X + halfSize.X;
        var y1 = pos.Y - halfSize.Y;
        var y2 = pos.Y + halfSize.Y;
        
        bool inBox1 = pX >= x1;
        bool inBox2 = pX <= x2;
        bool inBox3 = pY >= y1;
        bool inBox4 = pY <= y2;
        
        return inBox1 && inBox2 && inBox3 && inBox4;
    }
    
    public void SetParent(int draggingObjectUid)
    {
        if (UID == draggingObjectUid) return;
        var parent = Engine.Get().CurrentScene.FindObjectByUID(draggingObjectUid);
        
        if (parent == null) return;
        if (parent.Children.Contains(draggingObjectUid)) return;
        
        ParentUid = parent.UID;
        
        parent.Children.Add(UID);
    }

    public object Clone(bool getNewUID = false)
    {
        Gameobject clone = (Gameobject)new Gameobject(this.Name);

        clone.Name           = this.Name          ;
        
        clone.UID = getNewUID ? UIDManager.GetUID() : this.UID;
        
        clone.UID            = this.UID           ;
        clone.CanBeSelected = this.CanBeSelected;
        clone.Serialize      = this.Serialize     ;
        clone._isPopupOpen   = this._isPopupOpen  ;
        
        clone.Components = new();
        // clone.Childs = new();
        // clone._toReset = new();
        //
        foreach (var component in Components)
        {
            clone.Components.Add((Component)component.Clone());
        }

        return clone;

    }

    public object Clone()
    {
        return Clone(false);
    }
}