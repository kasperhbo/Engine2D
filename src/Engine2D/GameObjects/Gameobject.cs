using System.Runtime.InteropServices;
using System.Transactions;
using Box2DSharp.Dynamics;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.Rendering;
using Engine2D.Scenes;
using Engine2D.UI;
using ImGuiNET;
using KDBEngine.Core;
using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace Engine2D.GameObjects;

public class Gameobject : Asset
{
    //UIDS
    public int UID = -1;
    public int PARENT_UID = -1;

    //
    private readonly List<Component> _componentsToAddEndOfFrame = new();
    public List<Component> components = new();
    public string Name = "";
    public System.Numerics.Vector2 localPosition = new();
    [JsonIgnore] public List<Gameobject> Childs = new List<Gameobject>();
    [JsonIgnore] private Gameobject _parent = null;
    [JsonIgnore] public Transform Transform { get; private set; }
    
    public Gameobject()
    {
    }

    public Gameobject(string name)
    {
        Name = name;
        components = new List<Component>();
    }
    
    public Gameobject(string name, List<Component> components)
    {
        Name = name;
        this.components = components;
    }



    public void Init(Renderer renderer)
    {
        if(Transform == null)
        {
            var t = new Transform();

            t.size = new System.Numerics.Vector2(100, 100);
            t.position = new System.Numerics.Vector2(0, 0);

            AddComponent(t);
            Transform = t;
        }
        Transform.Parent = this;
        
        if (UID == -1) UID = UIDManager.GetUID();
        UIDManager.TakenUIDS.Add(UID);
        foreach (var component in components) component.Init(this, renderer);
    }

    public void Start()
    {
        foreach (var component in components) component.Start();
    }

    public void EditorUpdate(double dt)
    {
        foreach (var component in components) component.EditorUpdate(dt);
    }

    public void GameUpdate(double dt)
    {
        foreach (var component in components) component.GameUpdate(dt);
    }

    public void Destroy()
    {
        foreach (var component in components) component.Destroy();
    }

    public void AddComponent(Component component, Renderer renderer)
    {
        component.Init(this, renderer);
        components.Add(component);
    }
    
    public void AddComponent(Component component)
    {
        component.Init(this);
        components.Add(component);
    }
    
    public override void OnGui()
    {
        ImGui.InputText("##name", ref Name, 256);
        ImGui.SameLine();
        ImGui.Separator();
        
        OpenTKUIHelper.DrawComponentWindow("Transform", "Transform",
            () => { Transform.ImGuiFields(); }, Transform.WindowSize().Y
        );
        
        for (var i = 0; i < components.Count; i++)
        {
            if (components[i].GetType() == typeof(Transform)) return;
            
            ImGui.PushID(i);

            OpenTKUIHelper.DrawComponentWindow(i.ToString(), components[i].GetItemType(),
                () => { components[i].ImGuiFields(); }, components[i].WindowSize().Y
            );

            ImGui.PopID();
        }


        // {
        //     ImGui.Dummy(new System.Numerics.Vector2(0, ImGui.GetContentRegionAvail().Y - 80));
        //     ImGui.Separator();
        //
        //     if (ImGui.Button("Add component", new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, 26)))
        //     {
        //         if (ImGui.BeginDragDropTarget())
        //         {
        //             var payload = ImGui.AcceptDragDropPayload("Script_Drop");
        //             if (payload.IsValidPayload())
        //             {
        //                 var component = (string)GCHandle.FromIntPtr(payload.Data).Target;
        //                 Log.Message("Dropped: " + component);
        //             }
        //
        //             ImGui.EndDragDropTarget();
        //         }
        //
        //         ImGui.OpenPopup("AddComponent");
        //     }
        //
        //     ImGui.GetMousePos();
        //
        //     ImGui.Dummy(new System.Numerics.Vector2(0, ImGui.GetContentRegionAvail().Y));
        //
        //
        //     //TODO: ADD COMPONENT TO GOP
        //     if (ImGui.BeginPopup("AddComponent"))
        //     {
        //         if (ImGui.MenuItem("ScriptComponent"))
        //         {
        //             var rb = new ScriptHolderComponent();
        //             var go = (Gameobject)Engine.Get().CurrentSelectedAsset;
        //             go?.AddComponent(rb);
        //             ImGui.CloseCurrentPopup();
        //         }
        //
        //         if (ImGui.MenuItem("RigidBody"))
        //         {
        //             var rb = new RigidBody(BodyType.DynamicBody);
        //             var go = (Gameobject)Engine.Get().CurrentSelectedAsset;
        //             go?.AddComponent(rb);
        //             ImGui.CloseCurrentPopup();
        //         }
        //
        //         if (ImGui.MenuItem("Sprite Renderer"))
        //         {
        //             var spr = new SpriteRenderer();
        //             var go = (Gameobject)Engine.Get().CurrentSelectedAsset;
        //             go?.AddComponent(spr);
        //             ImGui.CloseCurrentPopup();
        //         }
        //
        //         ImGui.EndPopup();
        //     }
        // }
        _componentsToAddEndOfFrame.Clear();
    }

    public T GetComponent<T>() where T : Component
    {
        foreach (var component in components)
            if (typeof(T) == component.GetType())
                return
                    (component as T)!;

        return null;
    }

    private void RemoveComponents(Component comp)
    {
        components.Remove(comp);
    }

    public void SetParent(int parentUID)
    {
        //TODO: DETACH THIS FROM PREV PARENT AND MAKE THIS SELF OWNED OBJ
        if (parentUID == -1) return;

        Scene currentScene = Engine.Get()._currentScene;

        if (this._parent != null)
            this._parent.Childs.Remove(this);

        this.PARENT_UID = parentUID;
        Gameobject parent = currentScene.FindObjectByUID(parentUID);
        
        parent.AddGameObjectChild(this);
        
        Log.Succes(string.Format("Succesfully attached uid: {0} to uid: {1}",UID, PARENT_UID));
    }
 
    private void AddGameObjectChild(Gameobject gameObject)
    {
        gameObject._parent = this;
        Childs.Add(gameObject);
    }
}