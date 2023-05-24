using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using Box2DSharp.Dynamics;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.SavingLoading;
using Engine2D.Scenes;
using Engine2D.UI;
using ImGuiNET;
using KDBEngine.Core;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL;


namespace Engine2D.GameObjects;

[JsonConverter(typeof(ComponentSerializer))]
public class GameObject : Asset
{
    public Int64 UID = -1;
    public Int64 ParentUID = -1;
    
    public readonly string Type = "GameObject";

    public string Name = "";
    public Transform Transform = new Transform();

    public List<Component> Components { get; private set; } = new List<Component>();
    
    [JsonIgnore]public GameObject? Parent = null;
    [JsonIgnore]public List<GameObject> Childs = new List<GameObject>();

    public List<GameObject>  GetChildren()
    {
        return Childs;
    }
    
    public GameObject(string name)
    {
        Name = name;
    }
    
    [JsonConstructor]
    public GameObject(string name, GameObject parent)
    {
        Name = name;
        Parent = parent;
        Init();
    }

    public void Init()
    {
        if (UID == -1) UID = UIDManager.GetNewUID();
        
        foreach (var component in Components)
        {
           component.Init(this);
        }
        
    }
    
    
    public void Start()
    {
        foreach (var component in Components)
        {
            component.Start();
        }
    }


    public void Update(double dt)
    {
        foreach (var component in Components)
        {
            component.Update(dt);
        }
    }

    public void EditorUpdate(double dt)
    {
        foreach (var component in Components)
        {
            component.EditorUpdate(dt);
        }
    }

    public void GameUpdate(double dt)
    {
        foreach (var component in Components)
        {
            component.GameUpdate(dt);
        }
    }
    
    public override void OnGui()
    {
        ImGui.InputText("##name", ref Name, 256);
        ImGui.SameLine();
        ImGui.Separator();

        OpenTKUIHelper.DrawComponentWindow("transform" + Name, "Transform", () =>
        {
            OpenTKUIHelper.DrawProperty("Position: ", ref Transform.position);
            OpenTKUIHelper.DrawProperty("Rotation: ", ref Transform.rotation);
            OpenTKUIHelper.DrawProperty("Scale: ",    ref Transform.size);
        });

        for (var i = 0; i < Components.Count; i++)
        {
            ImGui.PushID(i);


            OpenTKUIHelper.DrawComponentWindow(i.ToString(), Components[i].GetItemType(),
                () => { Components[i].ImGuiFields(); }, Components[i].WindowSize().Y
            );

            ImGui.PopID();
        }


        {
            ImGui.Dummy(new System.Numerics.Vector2(0, ImGui.GetContentRegionAvail().Y - 80));
            ImGui.Separator();

            if (ImGui.Button("Add component", new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, 26)))
            {
                if (ImGui.BeginDragDropTarget())
                {
                    var payload = ImGui.AcceptDragDropPayload("Script_Drop");
                    if (payload.IsValidPayload())
                    {
                        var component = (string)GCHandle.FromIntPtr(payload.Data).Target;
                        Log.Message("Dropped: " + component);
                    }

                    ImGui.EndDragDropTarget();
                }

                ImGui.OpenPopup("AddComponent");
            }

            ImGui.GetMousePos();

            ImGui.Dummy(new System.Numerics.Vector2(0, ImGui.GetContentRegionAvail().Y));

            if (ImGui.BeginPopup("AddComponent"))
            {
                if (ImGui.MenuItem("ScriptComponent"))
                {
                    var rb = new ScriptHolderComponent();
                    var go = (GameObject)Engine.Get().CurrentSelectedAsset;
                    go?.AddComponent(rb);
                    ImGui.CloseCurrentPopup();
                }

                if (ImGui.MenuItem("RigidBody"))
                {
                    var rb = new RigidBody(BodyType.DynamicBody);
                    var go = (GameObject)Engine.Get().CurrentSelectedAsset;
                    go?.AddComponent(rb);
                    ImGui.CloseCurrentPopup();
                }

                if (ImGui.MenuItem("Sprite Renderer"))
                {
                    var spr = new SpriteRenderer();
                    var go = (GameObject)Engine.Get().CurrentSelectedAsset;
                    go?.AddComponent(spr);
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }
    }

    public bool RemoveComponent<T>() where T : Component
    {
        for (int i = 0; i < Components.Count(); i++)
        {
            if (typeof(T) == Components[i].GetType())
            {
                Components.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    public void AddComponent(Component component)
    {
        Components.Add(component);
        component.Init(this);
    }
    
    public T GetComponent<T>() where T : Component
    {
        foreach (var component in Components)
            if (typeof(T) == component.GetType())
                return
                    (component as T)!;

        return null;
    }


    public void SetParent(Int64 parentUID)
    {
        if (parentUID == -1) return;
        Scene currentScene = Engine.Get()._currentScene;
        if(Parent != null)
            Parent.Childs.Remove(this);
        
        ParentUID = parentUID;
        GameObject parent = currentScene.FindObjectByUID(parentUID);
        currentScene.GameObjectsHierachy.Remove(this);
        parent.AddGameObjectChild(this);
    }

    private void AddGameObjectChild(GameObject gameObject)
    {
        gameObject.Parent = this;
        Childs.Add(gameObject);
    }
}