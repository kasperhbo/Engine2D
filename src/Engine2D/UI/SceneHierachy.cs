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

                //if (Engine.Get().CurrentSelectedAsset == Engine.Get()._currentScene?.Gameobjects[i])
                //    flags |= ImGuiTreeNodeFlags.Selected;

                //? ImGuiTreeNodeFlags.Selected : 0) | ImGuiTreeNodeFlags.OpenOnArrow;

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


                if (ImGui.BeginPopupContextWindow("t")) ImGui.MenuItem("New Child");

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
                        var go = new Gameobject((Engine.Get()._currentScene?.Gameobjects.Count + 1).ToString(),
                            new Transform());
                        Engine.Get()._currentScene?.AddGameObjectToScene(go);
                    }

                    if (ImGui.MenuItem("New SpriteRenderer"))
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

                        var go = new Gameobject((Engine.Get()._currentScene?.Gameobjects.Count + 1).ToString(),
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

                        var go = new Gameobject((Engine.Get()._currentScene?.Gameobjects.Count + 1).ToString(),
                            components, new Transform());
                        Engine.Get()._currentScene?.AddGameObjectToScene(go);
                    }

                    ImGui.EndPopup();
                }
            }
            ImGui.EndChild();
        };
    }
}