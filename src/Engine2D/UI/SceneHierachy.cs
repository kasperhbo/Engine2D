﻿using Engine2D.Components;
using Engine2D.GameObjects;
using ImGuiNET;
using KDBEngine.Core;
using KDBEngine.UI;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.UI
{
    internal class SceneHierachy : UIElemenet
    {
        internal SceneHierachy(Inspector inspector)
        {
            this.Title = "Hierachy";
            this._flags = ImGuiNET.ImGuiWindowFlags.None;
            this._windowContents = () =>
            {
                for (int i = 0; i < Engine.Get()._currentScene?.Gameobjects.Count; i++)
                {
                    ImGui.PushID(i);

                    bool treeOpen = ImGuiNET.ImGui.TreeNodeEx(
                        Engine.Get()._currentScene?.Gameobjects[i].Name,

                        ImGuiTreeNodeFlags.DefaultOpen |
                        ImGuiTreeNodeFlags.FramePadding |
                        ImGuiTreeNodeFlags.OpenOnArrow |
                        ImGuiTreeNodeFlags.SpanAvailWidth,

                        Engine.Get()._currentScene?.Gameobjects[i].Name
                    );
                    
                    
                    if (ImGui.BeginPopupContextWindow("t"))
                    {
                        ImGui.MenuItem("New Child");
                    }

                    if (ImGui.IsItemClicked())
                    {
                        Engine.Get().CurrentSelectedAsset = Engine.Get()._currentScene.Gameobjects[i];
                        //inspector.CurrentSelectedGameObject = Engine.Get()._currentScene?.Gameobjects[i];
                    }

                    ImGuiNET.ImGui.PopID();

                    if (treeOpen)
                    {
                        ImGuiNET.ImGui.TreePop();
                    }
                }

                ImGui.BeginChild("Scrolling");
                {
                    if (ImGui.BeginPopupContextWindow("p"))
                    {
                        if (ImGui.MenuItem("New GameObject"))
                        {
                            Gameobject go = new Gameobject((Engine.Get()._currentScene?.Gameobjects.Count + 1).ToString(), new Components.Transform());
                            Engine.Get()._currentScene?.AddGameObjectToScene(go);
                        }
                        if (ImGui.MenuItem("New SpriteRenderer"))
                        {
                            SpriteRenderer spriteRenderer = new SpriteRenderer();
                            spriteRenderer.textureData = new(
                                "C:\\Users\\Kasper\\Documents\\GAMEPROJECTS\\helloworld-01\\Images\\TestImage.png",
                                false,
                                OpenTK.Graphics.OpenGL4.TextureMinFilter.Nearest,
                                OpenTK.Graphics.OpenGL4.TextureMagFilter.Nearest
                                );
                            List<Component> components = new List<Component>
                            {
                                spriteRenderer
                            };
                            
                            Gameobject go = new Gameobject((Engine.Get()._currentScene?.Gameobjects.Count + 1).ToString(), components, new Components.Transform());
                            Engine.Get()._currentScene?.AddGameObjectToScene(go);
                        }

                        if (ImGui.MenuItem("New RB"))
                        {
                            List<Component> components = new List<Component>
                            {
                                new SpriteRenderer(),
                                new RigidBody(Box2DSharp.Dynamics.BodyType.DynamicBody)
                            };

                            Gameobject go = new Gameobject((Engine.Get()._currentScene?.Gameobjects.Count + 1).ToString(), components, new Components.Transform());
                            Engine.Get()._currentScene?.AddGameObjectToScene(go);
                        }

                        ImGui.EndPopup();
                    }
                }
                ImGui.EndChild();
            };
        }
    }
}
