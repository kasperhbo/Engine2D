using Box2DSharp.Dynamics;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using ImGuiNET;
using KDBEngine.Core;
using KDBEngine.UI;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.UI;

internal class SceneHierachy : UIElemenet
{
    protected override string SetWindowTitle()
    {
        return "Hierachy";
    }

    protected override ImGuiWindowFlags SetWindowFlags()
    {
        return ImGuiWindowFlags.None;
    }

    private void CreateHierachyItem()
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
        
        
        if (open) {
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
        if(ImGui.IsItemClicked())
        {
            Engine.Get().CurrentSelectedAsset = clicked;
        }
    }

    private List<Gameobject> GetGameObjects()
    {
        List<Gameobject>_gameobjectsWithoutParents = new List<Gameobject>();
        for (int i = 0; i < Engine.Get()._currentScene.Gameobjects.Count; i++)
        {
            Gameobject go = Engine.Get()._currentScene.Gameobjects[i];
            
            if (go.PARENT_UID == -1)
            {
                _gameobjectsWithoutParents.Add(go);
            }
        }

        return _gameobjectsWithoutParents;
    }

    protected override Action SetWindowContent()
    {
        return () =>
        {
            CreateHierachy();
            // for (var i = 0; i < Engine.Get()._currentScene?.Gameobjects.Count; i++)
            // {
            //     ImGuiTreeNodeFlags flags = new();
            //
            //     if (Engine.Get().CurrentSelectedAsset == Engine.Get()._currentScene?.Gameobjects[i])
            //         flags |= ImGuiTreeNodeFlags.Selected;
            //     else
            //         flags |= ImGuiTreeNodeFlags.None;
            //
            //
            //     flags |= ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.OpenOnArrow;
            //
            //     ImGui.PushID(i);
            //
            //     var treeOpen = ImGui.TreeNodeEx(
            //         Engine.Get()._currentScene?.Gameobjects[i].Name,
            //         flags,
            //         Engine.Get()._currentScene?.Gameobjects[i].Name
            //     );
            //
            //
            //     if (ImGui.BeginPopupContextWindow("t"))
            //     {
            //         if (ImGui.MenuItem("New Child"))
            //         {
            //         }
            //
            //         ImGui.EndPopup();
            //     }
            //
            //     if (ImGui.IsItemClicked())
            //         Engine.Get().CurrentSelectedAsset = Engine.Get()._currentScene.Gameobjects[i];
            //     //inspector.CurrentSelectedGameObject = Engine.Get()._currentScene?.Gameobjects[i];
            //     ImGui.PopID();
            //
            //     if (treeOpen) ImGui.TreePop();
            // }

            ImGui.BeginChild("Scrolling");
            {
                if (ImGui.BeginPopupContextWindow("p"))
                {
                    if (ImGui.MenuItem("New GameObject"))
                    {
                        var go = new Gameobject(("Gameobject: " + Engine.Get()._currentScene?.Gameobjects.Count + 1),
                            new Transform());
                        Engine.Get()._currentScene?.AddGameObjectToScene(go);
                    }

                    if (ImGui.MenuItem("New Empty Sprite Renderer"))
                    {
                        var spriteRenderer = new SpriteRenderer();
                        var components = new List<Component>
                        {
                            spriteRenderer
                        };

                        var go = new Gameobject(("Empty Sprite: " + Engine.Get()._currentScene?.Gameobjects.Count + 1),
                            components, new Transform());
                        Engine.Get()._currentScene?.AddGameObjectToScene(go);
                    }

                    if (ImGui.MenuItem("New Mario"))
                    {
                        var spriteRenderer = new SpriteRenderer();
                        spriteRenderer.textureData = new TextureData(
                            "D:\\dev\\EngineDev\\Engine2D\\src\\ExampleGame\\Images\\TestImage.png",
                            true,
                            TextureMinFilter.Nearest,
                            TextureMagFilter.Nearest
                        );
                        var components = new List<Component>
                        {
                            spriteRenderer
                        };

                        var go = new Gameobject(("Mario: " + Engine.Get()._currentScene?.Gameobjects.Count + 1),
                            components, new Transform());
                        Engine.Get()._currentScene?.AddGameObjectToScene(go);
                    }

                    if (ImGui.MenuItem("New RB"))
                    {
                        var components = new List<Component>
                        {
                            new SpriteRenderer(),
                            new RigidBody(BodyType.DynamicBody)
                        };

                        var go = new Gameobject(("Rigidbody: " + Engine.Get()._currentScene?.Gameobjects.Count + 1),
                            components, new Transform());
                        Engine.Get()._currentScene?.AddGameObjectToScene(go);
                    }

                    if (ImGui.BeginMenu("Lighting"))
                    {
                        if (ImGui.MenuItem("New Point Light"))
                        {
                            var pl = new PointLight();

                            var components = new List<Component>
                            {
                                pl
                            };
                            var go = new Gameobject(
                                ("Point Light: " + Engine.Get()._currentScene?.Gameobjects.Count + 1),
                                components, new Transform());

                            Engine.Get()._currentScene?.AddGameObjectToScene(go);
                        }

                        if (ImGui.MenuItem("New Global Light"))
                        {
                            var comp = new GlobalLight();

                            var components = new List<Component>
                            {
                                comp
                            };
                            var go = new Gameobject(
                                ("GlobalLight: " + Engine.Get()._currentScene?.Gameobjects.Count + 1),
                                components, new Transform());

                            Engine.Get()._currentScene?.AddGameObjectToScene(go);
                        }

                        ImGui.EndMenu();
                    }

                    ;

                    ImGui.EndPopup();
                }

                ImGui.EndChild();
            }
        };
    }
}