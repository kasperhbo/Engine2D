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
                        var go = new GameObject("Gameobject: " + Engine.Get()._currentScene?.Gameobjects.Count + 1,
                            new Transform());
                        Engine.Get()._currentScene?.AddGameObjectToScene(go);
                    }

                    if (ImGui.MenuItem("New Empty Sprite Renderer"))
                    {
                        var go = new SpriteRendererGamObject( "GameObject: " + Engine.Get()._currentScene.Gameobjects.Count + 1);
                        Engine.Get()._currentScene?.AddGameObjectToScene(go);
                    }

                    if (ImGui.MenuItem("New Mario"))
                    {
                        var textureData = new TextureData(
                            "testImage",
                            "D:\\dev\\EngineDev\\Engine2D\\src\\ExampleGame\\Images\\TestImage.png",
                            true,
                            TextureMinFilter.Linear,
                            TextureMagFilter.Linear
                        );
                        var go = new SpriteRendererGamObject("Mario: " + Engine.Get()._currentScene?.Gameobjects.Count + 1,
                            textureData);
                        Engine.Get()._currentScene?.AddGameObjectToScene(go);
                    }

                    if (ImGui.MenuItem("New RB"))
                    {
                        var components = new List<Component>
                        {
                            new SpriteRenderer(),
                            new RigidBody(BodyType.DynamicBody)
                        };

                        var go = new GameObject("Rigidbody: " + Engine.Get()._currentScene?.Gameobjects.Count + 1,
                            new Transform(), components);
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
                            var go = new GameObject(
                                "Point Light: " + Engine.Get()._currentScene?.Gameobjects.Count + 1,
                                new Transform(),components);

                            Engine.Get()._currentScene?.AddGameObjectToScene(go);
                        }

                        if (ImGui.MenuItem("New Global Light"))
                        {
                            var comp = new GlobalLight();

                            var components = new List<Component>
                            {
                                comp
                            };
                            var go = new GameObject(
                                "GlobalLight: " + Engine.Get()._currentScene?.Gameobjects.Count + 1,
                                new Transform(), components);

                            Engine.Get()._currentScene?.AddGameObjectToScene(go);
                        }

                        ImGui.EndMenu();
                    }
                    
                    if(ImGui.BeginMenu("Camera's"))
                    {
                        if (ImGui.MenuItem("Main Camera"))
                        {
                            CameraGameObject go =
                                new CameraGameObject("Main Camera " + Engine.Get()._currentScene.Gameobjects.Count + 1);
                            
                            Engine.Get()._currentScene?.AddGameObjectToScene(go);
                        }
                        ImGui.EndMenu();
                    }
                    ImGui.EndPopup();
                }

                ImGui.EndChild();
            }
        };
    }
}