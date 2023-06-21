#region

using System.Numerics;
using Engine2D.Components;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.Rendering;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;
using Newtonsoft.Json;

#endregion

namespace Engine2D.GameObjects;

internal class Gameobject : Asset
{
    [JsonProperty]internal List<Component?> components = new();
    [JsonProperty]internal string Name = "";
    [JsonProperty]internal int PARENT_UID = -1;
    
    private readonly Type typeToRemove = null;
    [JsonIgnore] protected Gameobject _parent;
    [JsonIgnore] internal List<Gameobject> Childs = new();
    [JsonIgnore] internal bool Serialize = true;

    //UIDS
    internal int UID = -1;

    internal Gameobject(string name)
    {
        Name = name;
        components = new List<Component?>();
        GetUID();
    }

    internal Gameobject(string name, List<Component?> components)
    {
        Name = name;
        this.components = components;
        GetUID();
    }

    [JsonConstructor]
    internal Gameobject(string name, List<Component?> components, int uid, int parentUid)
    {
        Name = name;
        this.components = components;
        UID = uid;
        PARENT_UID = parentUid;
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

        foreach (var component in components) component.Init(this, renderer);
    }

    internal void Start()
    {
        foreach (var component in components) component.Start();
    }

    internal virtual void EditorUpdate(double dt)
    {
        foreach (var component in components) component.EditorUpdate(dt);
    }

    internal void GameUpdate(double dt)
    {
        foreach (var component in components) component.GameUpdate(dt);
    }

    internal void Destroy()
    {
        foreach (var component in components) component.Destroy();
    }

    internal void AddComponent(Component? component, Renderer? renderer)
    {
        component.Init(this, renderer);
        components.Add(component);
    }

    internal Component? AddComponent(Component? component)
    {
        if (component == null) return null;
        component.Init(this);
        components.Add(component);
        return component;
    }

    internal T? GetComponent<T>() where T : Component
    {
        foreach (var component in components)
        {
            if (component == null)
            {
                components.Remove(component);
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
        components.Remove(comp);
    }

    internal void SetParent(int parentUID)
    {
        //TODO: DETACH THIS FROM PREV PARENT AND MAKE THIS SELF OWNED OBJ
        if (parentUID == -1) return;

        var currentScene = Engine.Get().CurrentScene;

        if (_parent != null)
            _parent.Childs.Remove(this);

        PARENT_UID = parentUID;
        var parent = currentScene.FindObjectByUID(parentUID);

        parent.AddGameObjectChild(this);

        Log.Succes(string.Format("Succesfully attached uid: {0} to uid: {1}", UID, PARENT_UID));
    }

    private void AddGameObjectChild(Gameobject gameObject)
    {
        gameObject._parent = this;
        Childs.Add(gameObject);
    }

    internal void RemoveComponent<T>() where T : Component
    {
        components.RemoveAll(IsRightComponent);
    }

    private bool IsRightComponent(Component obj)
    {
        if (obj.GetType() == typeToRemove) return true;

        return false;
    }

    internal override void OnGui()
    {
        ImGui.InputText("##name", ref Name, 256);
        ImGui.SameLine();
        ImGui.Text(" UID: " + UID);
        ImGui.Separator();

        OpenTkuiHelper.DrawComponentWindow("Transform", "Transform",
            () => { GetComponent<Transform>().ImGuiFields(); }, GetComponent<Transform>().GetFieldSize()
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