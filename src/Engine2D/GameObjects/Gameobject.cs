#region

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
    [JsonProperty] internal int ParentUid = -1;
    [JsonProperty] internal int UID = -1;
    [JsonProperty] private bool _canBeSelected = true;
    [JsonProperty] internal List<Component> Components = new();
    
    [JsonIgnore] private Gameobject? _parent;
    [JsonIgnore] internal List<Gameobject> Childs = new();
    [JsonIgnore] internal bool Serialize = true;
    [JsonIgnore] bool _isPopupOpen = false;
    
    [JsonIgnore] private List<Component> _toReset = new();
    [JsonIgnore] public Transform? Transform => GetComponent<Transform>();
    
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
        if (UID == -1) UID = UIDManager.GetUID();
        else UIDManager.TakenUIDS.Add(UID);
    }


    internal void Init(Renderer? renderer)
    {
        GetUID();
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
        
        if (ParentUid != -1)
        {
            SetParent(ParentUid);
        }
    }
    
    

    internal virtual void Update(FrameEventArgs args)
    {
        foreach (var component in Components) component.Update(args);
        if (ParentUid != -1)
        {
            if (_parent == null) return;
            Transform.Position = _parent.Transform.Position ;
            
            var thisSize   = Transform.GetFullSize(false);
            var parentSize = _parent.Transform.GetFullSize(false);
            thisSize = parentSize;
            
            Transform.Rotation = _parent.Transform.Rotation ;
        }
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
            
            if (component is RigidBody)
            {
                physics2DWorld.AddRigidbody(component as RigidBody);
            }
        }
    }
    
    internal void StopPlay()
    {
        Destroy();
    }

    internal void Destroy()
    {
        if (_parent != null)
            _parent.RemoveChild(this);

        for (int i = 0; i < Childs.Count; i++)
        {
            var child = Childs[i];
            child.Destroy();
            i--;
        }
        foreach (var component in Components) component.Destroy();
    }

    private void RemoveChild(Gameobject gameobject)
    {
        Childs.Remove(gameobject);
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

    public void SetParent(Gameobject? parent)
    {
        if (parent == null) return;
        SetParent(parent.UID);
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
        
        if(parent == null)
        {
            Log.Error($"Couldn't find to uid: {ParentUid}, to attach {UID} to");
            return;
        }
        
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

        ImGui.Text("Can be selected: ");
        ImGui.SameLine();
        ImGui.Checkbox("##CanBeSelected", ref _canBeSelected);
        
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
        
        if (!_canBeSelected) return false;
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
        
        if(ParentUid != -1)
        {
            pos.X += transform.LocalPosition.X;
            pos.Y += transform.LocalPosition.Y;
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
    
    public object Clone()
    {
        Gameobject clone = (Gameobject)this.MemberwiseClone();
        clone.Components = new();
        foreach (var component in Components)
        {
            clone.Components.Add((Component)component.Clone());
        }
        
        return clone;
    }
}