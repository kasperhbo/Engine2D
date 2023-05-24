using System.Numerics;
using System.Runtime.InteropServices;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Testing;
using ImGuiNET;
using KDBEngine.Core;
using KDBEngine.UI;

namespace Engine2D.UI;

internal class SceneHierachy : UIElemenet
{
    private GCHandle? _currentlyDraggedHandle;
    private bool _currentlyDragging;
    
    protected override Action SetWindowContent()
    {
        return () =>
        {
            if (ImGui.BeginMenu("Create"))
            {
                bool create = false;
                List<Component> componentsToAdd = null;
                if (ImGui.MenuItem("New Gameobject"))
                {
                    create = true;
                }
                
                if (ImGui.MenuItem("New Sprite Rendenderer"))
                {
                    componentsToAdd = new List<Component>();
                    SpriteRenderer spr = new SpriteRenderer();
                    componentsToAdd.Add(spr);
                    create = true;
                }
                
                if (ImGui.MenuItem("New Rigidbody"))
                {
                    componentsToAdd = new List<Component>();
                    SpriteRenderer spr = new SpriteRenderer();
                    componentsToAdd.Add(spr);
                    RigidBody rb = new RigidBody();
                    componentsToAdd.Add(rb);
                    create = true;
                }
                
                if (ImGui.MenuItem("New Camera"))
                {
                    componentsToAdd = new List<Component>();
                    TestCamera camera = new TestCamera();
                    componentsToAdd.Add(camera);
                    create = true;
                }
                
                if (ImGui.BeginMenu("Lighting"))
                {
                    if (ImGui.MenuItem("Global Light"))
                    {
                        componentsToAdd = new List<Component>();
                        GlobalLight glbl = new GlobalLight();
                        componentsToAdd.Add(glbl);
                        create = true;
                    }

                    if (ImGui.MenuItem("Point Light"))
                    {
                        componentsToAdd = new List<Component>();
                        PointLight pl = new PointLight();
                        componentsToAdd.Add(pl);
                        create = true;
                    }
                    ImGui.EndMenu();
                }

                if (create)
                {
                    GameObject go = new GameObject("GameObject " + (Engine.Get()._currentScene.GameObjectsHierachy.Count() + 1));
                    if (componentsToAdd != null)
                    {
                        foreach (var c in componentsToAdd)
                        {
                            go.AddComponent(c);
                        }
                    }
                    Engine.Get()._currentScene.AddGameObjectToScene(go);
                }
                ImGui.EndMenu();
            }
            
            for (var i = 0; i < Engine.Get()._currentScene?.GameObjectsHierachy.Count; i++)
            {
                GameObject gameObject = Engine.Get()._currentScene?.GameObjectsHierachy[i];
                DrawGameObjectNode(gameObject);
            }
        };
    }
    
    protected override string SetWindowTitle()
    {
        return "Hierachy";
    }

    protected override ImGuiWindowFlags SetWindowFlags()
    {
        return ImGuiWindowFlags.None;
    }

    private void DrawGameObjectNode(GameObject gameObject)
    {
        
        bool selected = gameObject == Engine.Get().CurrentSelectedAsset;
        
        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.FramePadding 
                    | ImGuiTreeNodeFlags.DefaultOpen 
                    | (selected ? ImGuiTreeNodeFlags.Selected : 0)
                    | ImGuiTreeNodeFlags.SpanAvailWidth
                    | ImGuiTreeNodeFlags.OpenOnArrow;
        
        
        bool open = ImGui.TreeNodeEx(gameObject.UID.ToString(), flags, gameObject.Name);
            
        HandleDragDrop(gameObject);
        HandleItemClicked(gameObject);
        
        List<GameObject> childs = gameObject.GetChildren();
        
        if (open) {
            for (int i = 0;
                 i < childs.Count;
                 i++)
            {
                DrawGameObjectNode(childs[i]);
            }
            ImGui.TreePop();
        }
    }
    
    private void HandleDragDrop(GameObject draggingObject) {
        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGui.AcceptDragDropPayload("GAMEOBJECT_DROP");
            if (payload.IsValidPayload())
            {
                Log.Message(string.Format("Dropping: {0}, onto: {1}", _currentlyDraggingOBJ.UID, draggingObject.UID));
                GameObject other = draggingObject;
                _currentlyDraggingOBJ.SetParent(draggingObject.UID);
            }

            ImGui.EndDragDropTarget();
        }
        
        if (ImGui.BeginDragDropSource())
        {
            _currentlyDragging = true;
            ImGui.SetDragDropPayload("GAMEOBJECT_DROP", IntPtr.Zero, 0);
            _currentlyDraggingOBJ = draggingObject;    
            ImGui.EndDragDropSource();
        }
    }

    private void HandleItemClicked(GameObject clicked)
    {
        if(ImGui.IsItemClicked())
        {
            Log.Message(string.Format("Clicked {0} with uid: {1}", clicked.Name, clicked.UID));
            Engine.Get().CurrentSelectedAsset = clicked;
        }
    }
    
    public GameObject _currentlyDraggingOBJ { get; set; }


}