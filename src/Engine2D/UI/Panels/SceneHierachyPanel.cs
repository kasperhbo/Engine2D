#region

using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using ImGuiNET;

#endregion

namespace Engine2D.UI;

internal class SceneHierachyPanel : UIElement
{
    private bool _currentlyDragging;

    private Gameobject _currentlyDraggingOBJ;

    internal SceneHierachyPanel(string title) : base(title)
    {
    }

    private void CreateHierachy()
    {
        var _gameobjectsWithoutParents = GetGameObjects();

        for (var i = 0; i < _gameobjectsWithoutParents.Count; i++)
        {
            var go = _gameobjectsWithoutParents[i];

            DrawGameobjectNode(go);
        }
    }

    private void DrawGameobjectNode(Gameobject go)
    {
        var selected = go == Engine.Get().CurrentSelectedAsset;

        var flags = ImGuiTreeNodeFlags.FramePadding
                    | ImGuiTreeNodeFlags.DefaultOpen
                    | (selected ? ImGuiTreeNodeFlags.Selected : 0)
                    | ImGuiTreeNodeFlags.SpanAvailWidth
                    | ImGuiTreeNodeFlags.OpenOnArrow;

        var open = ImGui.TreeNodeEx(go.UID.ToString(), flags, go.Name);

        //Handle Drag Drop
        HierachyItemDragDropped(go);

        //Handle Item Clicked
        HierachyItemClicked(go);


        if (open)
        {
            for (var i = 0;
                 i < go.Childs.Count;
                 i++)
                DrawGameobjectNode(go.Childs[i]);

            ImGui.TreePop();
        }
    }

    private void HierachyItemDragDropped(Gameobject draggingObject)
    {
        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGui.AcceptDragDropPayload("GAMEOBJECT_DROP");
            if (payload.IsValidPayload())
            {
                Log.Message(string.Format("Dropping: {0}, onto: {1}", _currentlyDraggingOBJ.UID, draggingObject.UID));
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

    private void HierachyItemClicked(Gameobject clicked)
    {
        if (ImGui.IsItemClicked()) Engine.Get().CurrentSelectedAsset = clicked;
    }

    private List<Gameobject> GetGameObjects()
    {
        var _gameobjectsWithoutParents = new List<Gameobject>();
        for (var i = 0; i < Engine.Get().CurrentScene.GameObjects.Count; i++)
        {
            var go = Engine.Get().CurrentScene.GameObjects[i];

            if (go.PARENT_UID == -1) _gameobjectsWithoutParents.Add(go);
        }

        return _gameobjectsWithoutParents;
    }

    internal override void Render()
    {
        {
            CreateHierachy();
            ImGui.BeginChild("Scrolling");
            {
                if (ImGui.BeginPopupContextWindow("p"))
                {
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
        }
        ;
    }
}