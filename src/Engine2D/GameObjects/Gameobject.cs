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
using Engine2D.Components.TransformComponents;

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
    [JsonIgnore] public Transform Transform { get; set; }
    
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

            t.Position = new System.Numerics.Vector2(0, 0);

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
            () =>
            {
                Transform.ImGuiFields();
            }, Transform.GetFieldSize()
        );
        
        for (var i = 0; i < components.Count; i++)
        {
            if (components[i].GetType() == typeof(Transform)) return;
            
            ImGui.PushID(i);

            OpenTKUIHelper.DrawComponentWindow(i.ToString(), components[i].GetItemType(),
                () => { components[i].ImGuiFields(); }, components[i].GetFieldSize() 
            );

            ImGui.PopID();
        }

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