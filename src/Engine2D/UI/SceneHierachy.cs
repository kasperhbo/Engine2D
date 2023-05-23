using Box2DSharp.Dynamics;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
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

    protected override Action SetWindowContent()
    {
        return () =>
        {
            for (var i = 0; i < Engine.Get()._currentScene?.Gameobjects.Count; i++)
            {
                ImGuiTreeNodeFlags flags = new();

                if (Engine.Get().CurrentSelectedAsset == Engine.Get()._currentScene?.Gameobjects[i])
                    flags |= ImGuiTreeNodeFlags.Selected;
                else
                    flags |= ImGuiTreeNodeFlags.None;


                flags |= ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.OpenOnArrow;

                ImGui.PushID(i);

                var treeOpen = ImGui.TreeNodeEx(
                    Engine.Get()._currentScene?.Gameobjects[i].Name,
                    flags,
                    Engine.Get()._currentScene?.Gameobjects[i].Name
                );


                if (ImGui.BeginPopupContextWindow("t"))
                {
                    if (ImGui.MenuItem("New Child"))
                    {
                    }

                    ImGui.EndPopup();
                }

                if (ImGui.IsItemClicked())
                    Engine.Get().CurrentSelectedAsset = Engine.Get()._currentScene.Gameobjects[i];
                //inspector.CurrentSelectedGameObject = Engine.Get()._currentScene?.Gameobjects[i];
                ImGui.PopID();

                if (treeOpen) ImGui.TreePop();
            }

            ImGui.BeginChild("Scrolling");
            {
                if (ImGui.BeginPopupContextWindow("p"))
                {
                    if (ImGui.MenuItem("New GameObject"))
                    {
                        var go = new Gameobject("Gameobject: " + Engine.Get()._currentScene?.Gameobjects.Count + 1,
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

                        var go = new Gameobject("Empty Sprite: " + Engine.Get()._currentScene?.Gameobjects.Count + 1,
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

                        var go = new Gameobject("Mario: " + Engine.Get()._currentScene?.Gameobjects.Count + 1,
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

                        var go = new Gameobject("Rigidbody: " + Engine.Get()._currentScene?.Gameobjects.Count + 1,
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
                                "Point Light: " + Engine.Get()._currentScene?.Gameobjects.Count + 1,
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
                                "GlobalLight: " + Engine.Get()._currentScene?.Gameobjects.Count + 1,
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