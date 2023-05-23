using System.Numerics;
using System.Runtime.InteropServices;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
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
            for (var i = 0; i < Engine.Get()._currentScene?.GameObjectsHierachy.Count; i++)
            {
                GameObject gameObject = Engine.Get()._currentScene?.GameObjectsHierachy[i];
                DrawGameObjectNode(gameObject);
            }
            
            if (ImGui.BeginMenu("Create"))
            {
                if (ImGui.MenuItem("New Gameobject"))
                {
                    GameObject go = new GameObject(
                        "GameObject " + (Engine.Get()._currentScene.GameObjectsHierachy.Count() + 1) 
                    );
                    
                    Engine.Get()._currentScene.AddGameObjectToScene(go);
                }
                
                ImGui.EndMenu();
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
                    | (selected ? ImGuiTreeNodeFlags.Selected : 0);
        
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(5, 5));
        bool open = ImGui.TreeNodeEx(gameObject.UID.ToString(), flags, gameObject.Name);
        ImGui.PopStyleVar();
            
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