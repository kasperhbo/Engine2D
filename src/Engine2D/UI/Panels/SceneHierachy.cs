using System.Numerics;
using Box2DSharp.Dynamics;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Testing;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Engine2D.UI;

internal class SceneHierachy : UIElement
{
    public SceneHierachy(string title) : base(title)
    {
    }

    private void CreateHierachy()
    {
        List<Gameobject> _gameobjectsWithoutParents = GetGameObjects();

        for (int i = 0; i < _gameobjectsWithoutParents.Count; i++)
        {
            Gameobject go = _gameobjectsWithoutParents[i];

            DrawGameobjectNode(go);
        }
    }

    private void DrawGameobjectNode(Gameobject go)
    {
        bool selected = go == Engine.Get().CurrentSelectedAsset;

        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.FramePadding
                                   | ImGuiTreeNodeFlags.DefaultOpen
                                   | (selected ? ImGuiTreeNodeFlags.Selected : 0)
                                   | ImGuiTreeNodeFlags.SpanAvailWidth
                                   | ImGuiTreeNodeFlags.OpenOnArrow;

        bool open = ImGui.TreeNodeEx(go.UID.ToString(), flags, go.Name);

        //Handle Drag Drop
        HierachyItemDragDropped(go);

        //Handle Item Clicked
        HierachyItemClicked(go);


        if (open)
        {
            for (int i = 0;
                 i < go.Childs.Count;
                 i++)
            {
                DrawGameobjectNode(go.Childs[i]);
            }

            ImGui.TreePop();
        }

    }

    private Gameobject _currentlyDraggingOBJ = null;
    private bool _currentlyDragging = false;

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
        if (ImGui.IsItemClicked())
        {
            Engine.Get().CurrentSelectedAsset = clicked;
        }
    }

    private List<Gameobject> GetGameObjects()
    {
        List<Gameobject> _gameobjectsWithoutParents = new List<Gameobject>();
        for (int i = 0; i < Engine.Get().CurrentScene.GameObjects.Count; i++)
        {
            Gameobject go = Engine.Get().CurrentScene.GameObjects[i];

            if (go.PARENT_UID == -1)
            {
                _gameobjectsWithoutParents.Add(go);
            }
        }

        return _gameobjectsWithoutParents;
    }

    public override void Render()
    {
        {
            CreateHierachy();
            ImGui.BeginChild("Scrolling");
            {
                if (ImGui.BeginPopupContextWindow("p"))
                {
                    if (ImGui.MenuItem("New SpriteRenderer"))
                    {
                        Engine.Get().CurrentScene.AddGameObjectToScene(new SpriteRendererGO("SpriteRenderer"));
                    }
                    
                    if (ImGui.MenuItem("New Game Camera"))
                    {
                        Engine.Get().CurrentScene.AddGameObjectToScene(new CameraGO("Camera"));
                    }
                    
                    if (ImGui.BeginMenu("Lighting"))
                    {
                        if (ImGui.MenuItem("Global Light"))
                        {
                            Engine.Get().CurrentScene.AddGameObjectToScene(new GlobalLightGO("Global Light"));
                        }
                            
                        if (ImGui.MenuItem("PointLight"))
                        {
                            Engine.Get().CurrentScene.AddGameObjectToScene(new PointLightGO("Point Light"));
                        }
                        
                        ImGui.EndMenu();
                    }

                }
            }
            ImGui.EndChild();
        };
    }
}