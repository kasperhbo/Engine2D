using Engine2D.Components;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.Rendering;
using Engine2D.Scenes;
using Engine2D.UI;
using ImGuiNET;
using Newtonsoft.Json;
using Engine2D.Components.TransformComponents;
using Engine2D.UI.ImGuiExtension;

namespace Engine2D.GameObjects;

public class Gameobject : Asset
{
    [JsonIgnore] public bool Serialize = true;
    
    //UIDS
    public int UID = -1;
    public string Name = "";
    
    public int PARENT_UID = -1;

    //
    public List<Component> components = new();
    
    [JsonIgnore] public List<Gameobject> Childs = new List<Gameobject>();
    [JsonIgnore] protected Gameobject _parent = null;
    
    public Gameobject(string name)
    {
        Name = name;
        components = new List<Component>();
        GetUID();
    }
    
    public Gameobject(string name, List<Component> components)
    {
        Name = name;
        this.components = components;
        GetUID();
    }
    
    [JsonConstructor]
    public Gameobject(string name, List<Component> components, int uid, int parentUid)
    {
        Name = name;
        this.components = components;
        this.UID = uid;
        this.PARENT_UID = parentUid;
        GetUID();
    }

    private void GetUID()
    {
        if (UID == -1) UID = UIDManager.GetUID();
        else UIDManager.TakenUIDS.Add(UID);
    }


    public void Init(Renderer? renderer)
    {
        if(this.GetComponent<Transform>() == null)
        {
            Log.Warning(this.Name + " Has no Transform Component, Adding one");
            
            var t = new Transform();

            t.Position = new System.Numerics.Vector2(0, 0);
            AddComponent(t);
        }
        
        foreach (var component in components) component.Init(this, renderer);
    }

    public void Start()
    {
        foreach (var component in components) component.Start();
    }

    public virtual void EditorUpdate(double dt)
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

    public void AddComponent(Component component, Renderer? renderer)
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
        ImGui.Text(" UID: " + UID);
        ImGui.Separator();

        OpenTkuiHelper.DrawComponentWindow("Transform", "Transform",
            () =>
            {
                this.GetComponent<Transform>().ImGuiFields();
                
            }, this.GetComponent<Transform>().GetFieldSize()
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

    public T? GetComponent<T>() where T : Component
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

        Scene currentScene = Engine.Get().CurrentScene;

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