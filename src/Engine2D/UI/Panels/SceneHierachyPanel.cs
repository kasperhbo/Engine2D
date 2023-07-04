#region

using System.Runtime.InteropServices;
using Engine2D.Components.SpriteAnimations;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.SavingLoading;
using ImGuiNET;

#endregion

namespace Engine2D.UI;

internal class SceneHierachyPanel : UIElement
{
    private bool _currentlyDragging;

    private Gameobject? _currentlyDraggingOBJ;

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

    private void DrawGameobjectNode(Gameobject? go)
    {
        var selected = go == Engine.Get().CurrentSelectedAsset;

        var flags = ImGuiTreeNodeFlags.FramePadding
                    | ImGuiTreeNodeFlags.DefaultOpen
                    | (selected ? ImGuiTreeNodeFlags.Selected : 0)
                    | ImGuiTreeNodeFlags.Leaf
                    | ImGuiTreeNodeFlags.SpanAvailWidth
                    | ImGuiTreeNodeFlags.OpenOnArrow;

        var open = ImGui.TreeNodeEx(go.UID.ToString(), flags, go.Name);

        //Handle Drag Drop
        HierachyItemDragDropped(go);

        //Handle Item Clicked
        HierachyItemClicked(go);

        if (open)
        {
            // for (var i = 0;
            //      i < go.Childs.Count;
            //      i++)
            //     DrawGameobjectNode(Engine.Get().CurrentScene.FindObjectByUID(go.Childs[i]));

            ImGui.TreePop();
        }
    }
    private GCHandle? _currentlyDraggedHandle;
    private unsafe void HierachyItemDragDropped(Gameobject? draggingObject)
    {
        // if (ImGui.BeginDragDropTarget())
        // {
        //     var payload = ImGui.AcceptDragDropPayload("GAMEOBJECT_DROP_Hierachy");
        //     if (payload.IsValidPayload())
        //     {
        //         Log.Message(string.Format("Dropping: {0}, onto: {1}", _currentlyDraggingOBJ.UID, draggingObject.UID));
        //         // _currentlyDraggingOBJ.SetParent(draggingObject.UID);
        //     }
        //
        //     ImGui.EndDragDropTarget();
        // }
        //
        // if (ImGui.BeginDragDropSource())
        // {
        //     _currentlyDragging = true;
        //     ImGui.SetDragDropPayload("GAMEOBJECT_DROP_Hierachy", IntPtr.Zero, 0);
        //     _currentlyDraggingOBJ = draggingObject;
        //     ImGui.EndDragDropSource();
        // }

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

    private void HierachyItemClicked(Gameobject? clicked)
    {
        if (ImGui.IsItemClicked())
        {
            Engine.Get().CurrentSelectedAsset = clicked;
            if (clicked.GetComponent<SpriteAnimator>() != null)
            {
                Engine.Get().CurrentSelectedAnimationAssetBrowserAsset = clicked.GetComponent<SpriteAnimator>().Animation;
            }
        }
        
    }

    private List<Gameobject?> GetGameObjects()
    {
        var gameobjectsWithoutParents = new List<Gameobject?>();
        for (var i = 0; i < Engine.Get().CurrentScene.GameObjects.Count; i++)
        {
            var go = Engine.Get().CurrentScene.GameObjects[i];
            
            if (go.ParentUid == -1 && go.Serialize) gameobjectsWithoutParents.Add(go);
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
                    if (ImGui.MenuItem("New Empty"))
                        Engine.Get().CurrentScene.AddGameObjectToScene(new Gameobject("Empty"));
                    if (ImGui.MenuItem("New SpriteRenderer"))
                        Engine.Get().CurrentScene.AddGameObjectToScene(new SpriteRendererGo("SpriteRenderer"));

                    if (ImGui.MenuItem("New Game Camera"))
                        Engine.Get().CurrentScene.AddGameObjectToScene(new CameraGO("Camera"));

                    if (ImGui.BeginMenu("Lighting"))
                    {
                        if (ImGui.MenuItem("Global Light"))
                            Engine.Get().CurrentScene.AddGameObjectToScene(new GlobalLightGO("Global Light"));

                        if (ImGui.MenuItem("PointLight"))
                            Engine.Get().CurrentScene.AddGameObjectToScene(new PointLightGO("Point Light"));

                        ImGui.EndMenu();
                    }
                }
            }
            ImGui.EndChild();
            if (ImGui.BeginDragDropTarget())
            {
                var payload = ImGui.AcceptDragDropPayload("prefab_drop");
                if (payload.IsValidPayload())
                {
                    var filename = (string)GCHandle.FromIntPtr(payload.Data).Target;
                    var prefab = SaveLoad.LoadGameobject(filename);
                    Engine.Get().CurrentScene.AddGameObjectToScene(prefab);
                }
                ImGui.EndDragDropTarget();
            }
        }
        ;
    }
}