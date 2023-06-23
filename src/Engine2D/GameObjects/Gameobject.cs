#region

using System.Numerics;
using System.Runtime.InteropServices;
using Engine2D.Components;
using Engine2D.Components.SpriteAnimations;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.Rendering;
using Engine2D.UI.ImGuiExtension;
using Engine2D.Utilities;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Windowing.Common;

#endregion

namespace Engine2D.GameObjects;

public class Gameobject : Asset
{
    [JsonProperty]internal List<Component> Components = new();
    [JsonProperty]internal string Name = "";
    [JsonProperty]internal int ParentUid = -1;
    
    [JsonIgnore] private Gameobject _parent;
    [JsonIgnore] internal List<Gameobject> Childs = new();
    [JsonIgnore] internal bool Serialize = true;
    [JsonIgnore] bool _isPopupOpen = false;
    
    [JsonIgnore] private List<Component> _toReset = new();

    //UIDS
    internal int UID = -1;

    internal Gameobject(string name)
    {
        Name = name;
        Components = new List<Component?>();
        GetUID();
    }

    internal Gameobject(string name, List<Component?> components)
    {
        Name = name;
        this.Components = components;
        GetUID();
    }

    [JsonConstructor]
    internal Gameobject(string name, List<Component?> components, int uid, int parentUid)
    {
        Name = name;
        this.Components = components;
        UID = uid;
        ParentUid = parentUid;
        GetUID();
    }

    private void GetUID()
    {
        if (UID == -1) UID = UIDManager.GetUID();
        else UIDManager.TakenUIDS.Add(UID);
    }


    internal void Init(Renderer? renderer)
    {
        if (GetComponent<Transform>() == null)
        {
            Log.Warning(Name + " Has no Transform Component, Adding one");

            var t = new Transform();

            t.Position = new Vector2(0, 0);
            AddComponent(t);
        }

        foreach (var component in Components) component.Init(this, renderer);
    }

    internal void Start()
    {
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

    public void StartPlay()
    {
        _toReset = new();
        
        foreach (var component in Components)
        {
            //Clone the components
            //This is so that we can reset the components after play
            Component comp = (Component)component.Clone();
            _toReset.Add(comp);
        }
        
        foreach (var component in Components)
        {
            component.StartPlay();
        }
    }
    
    internal void StopPlay()
    {
        foreach (var component in Components)
        {
            component.StopPlay();
            component.Destroy();
        }
        
        this.Components = new();
        
        foreach (var component in _toReset)
        {
            //Add the clones back
            AddComponent(component);
        }
    }

    internal void Destroy()
    {
        foreach (var component in Components) component.Destroy();
    }

    internal void AddComponent(Component? component, Renderer? renderer)
    {
        component.Init(this, renderer);
        Components.Add(component);
    }

    internal Component? AddComponent(Component? component)
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
            if (component == null)
            {
                Components.Remove(component);
                break;
            }

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

    internal void SetParent(int parentUID)
    {
        //TODO: DETACH THIS FROM PREV PARENT AND MAKE THIS SELF OWNED OBJ
        if (parentUID == -1) return;

        var currentScene = Engine.Get().CurrentScene;

        if (_parent != null)
            _parent.Childs.Remove(this);

        ParentUid = parentUID;
        var parent = currentScene.FindObjectByUID(parentUID);

        parent.AddGameObjectChild(this);

        Log.Succes(string.Format("Succesfully attached uid: {0} to uid: {1}", UID, ParentUid));
    }

    private void AddGameObjectChild(Gameobject gameObject)
    {
        gameObject._parent = this;
        Childs.Add(gameObject);
    }

    internal override void OnGui()
    {
        ImGui.InputText("##name", ref Name, 256);

        ImGui.SameLine();
        ImGui.Text(" UID: " + UID);
        ImGui.Separator();
        
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

    
}