#region

using System.Runtime.InteropServices;
using Engine2D.Components.ENTT;
using Engine2D.Core;
using Engine2D.SavingLoading;
using ImGuiNET;

#endregion

namespace Engine2D.UI;

internal class SceneHierachyPanel : UIElement
{
    private bool _currentlyDragging;

    private Entity? _currentlyDraggingOBJ;

    internal SceneHierachyPanel(string title) : base(title)
    {
        IsVisible = true;
    }

    private void CreateHierachy()
    {
        var gameobjectsWithoutParents = GetGameObjects();

        for (var i = 0; i < gameobjectsWithoutParents.Count; i++)
        {
            var go = gameobjectsWithoutParents[i];

            DrawGameobjectNode(go);
        }
    }

    private void DrawGameobjectNode(Entity? entity)
    {
        var selected = entity == Engine.Get().CurrentSelectedAsset;

        if (entity == null) return;
        
        var flags = ImGuiTreeNodeFlags.FramePadding
                    | ImGuiTreeNodeFlags.DefaultOpen
                    | (selected ? ImGuiTreeNodeFlags.Selected : 0)
                    | ImGuiTreeNodeFlags.Leaf
                    | ImGuiTreeNodeFlags.SpanAvailWidth
                    | ImGuiTreeNodeFlags.OpenOnArrow;
        
        var open = ImGui.TreeNodeEx(entity.UUID, flags, entity.GetComponent<ENTTTagComponent>().Tag);

        //Handle Drag Drop
        HierachyItemDragDropped(entity);

        //Handle Item Clicked
        HierachyItemClicked(entity);

        if (open)
        {
            // for (var i = 0;
            //      i < entity.Children.Count;
            //      i++)
            // {
            //     // var item = Engine.Get().CurrentScene.FindObjectByUID(go.Children[i]);
            //     // DrawGameobjectNode(item);
            // }

            ImGui.TreePop();
        }
    }
    private GCHandle? _currentlyDraggedHandle;
    private unsafe void HierachyItemDragDropped(Entity? draggingObject)
    {
        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGui.AcceptDragDropPayload("GAMEOBJECT_DROP_Hierachy");
            if (payload.IsValidPayload())
            {
                // _currentlyDraggingOBJ.SetParent(draggingObject.UID);
            }

            ImGui.EndDragDropTarget();
        }
        
        if (ImGui.BeginDragDropSource())
        {
            _currentlyDraggedHandle ??= GCHandle.Alloc(draggingObject);

            ImGui.SetDragDropPayload("gameobject_drop_hierachy", GCHandle.ToIntPtr(_currentlyDraggedHandle.Value),
                (uint)sizeof(IntPtr));
            ImGui.EndDragDropSource();
        }
        
        // if (ImGui.BeginDragDropTarget())
        // {
        //     var payload = ImGui.AcceptDragDropPayload("gameobject_drop_hierachy");
        //     if (payload.IsValidPayload())
        //     {
        //         var handle = GCHandle.FromIntPtr(new IntPtr(payload.Data));
        //         var draggedObject = (Gameobject?)handle.Target;
        //         
        //         if (draggedObject != null)
        //         {
        //             draggedObject.SetParent(draggingObject.UID);
        //         }
        //     }
        //     ImGui.EndDragDropTarget();
        // }
    }

    private void HierachyItemClicked(Entity? clicked)
    {
        if (ImGui.IsItemClicked())
        {
            Engine.Get().CurrentSelectedAsset = clicked;
        }
    }

    private List<Entity> GetGameObjects()
    {
        var gameobjectsWithoutParents = new List<Entity>();
        
        for (var i = 0; i < Engine.Get().CurrentScene.Entities.Count; i++)
        {
            var ent = Engine.Get().CurrentScene.Entities[i];
            gameobjectsWithoutParents.Add(ent);   
            // if (go.ParentUid == -1 && go.Serialize) gameobjectsWithoutParents.Add(go);
        }
    
        return gameobjectsWithoutParents;
    }

    internal override void Render()
    {
        {
            CreateHierachy();
            ImGui.BeginChild("Scrolling");
            {
                if (ImGui.BeginPopupContextWindow("p"))
                {
                    // if (ImGui.MenuItem("New Empty"))
                    //     Engine.Get().CurrentScene.AddGameObjectToScene(new Gameobject("Empty"));
                    //
                    // if (ImGui.MenuItem("New SpriteRenderer"))
                    //     Engine.Get().CurrentScene.AddGameObjectToScene(new SpriteRendererGo("SpriteRenderer"));
                    //
                    // if (ImGui.MenuItem("New Game Camera"))
                    //     Engine.Get().CurrentScene.AddGameObjectToScene(new CameraGO("Camera"));
                }
            }
            ImGui.EndChild();
            if (ImGui.BeginDragDropTarget())
            {
                var payload = ImGui.AcceptDragDropPayload("prefab_drop");
                if (payload.IsValidPayload())
                {
                    var filename = (string)GCHandle.FromIntPtr(payload.Data).Target;
                    // var prefab = SaveLoad.LoadGameobject(filename);
                    // Engine.Get().CurrentScene.AddGameObjectToScene(prefab);
                }
                ImGui.EndDragDropTarget();
            }
        }
        ;
    }
}