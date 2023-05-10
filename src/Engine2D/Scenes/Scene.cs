using Box2DSharp.Collision.Shapes;
using Box2DSharp.Common;
using Box2DSharp.Dynamics;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Engine2D.Testing;
using Engine2D.UI;
using ImGuiNET;
using KDBEngine.Core;
using KDBEngine.UI;
using NativeFileDialogExtendedSharp;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Engine2D.Scenes
{
    internal class Scene
    {
        internal Gameobject SelectedGameobject;

        internal string ScenePath { get; private set; } = "NoScene";
        
        internal List<Gameobject> Gameobjects { get; private set; } = new List<Gameobject>();

        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            set
            {
                if (value)
                {
                    StartPlay();
                }
                else
                {
                    StopPlay();
                }
                _isPlaying = value;
            }
        }

        #region onplay
        World physicsWorld = null;
        #endregion  

        private void StartPlay()
        {
            physicsWorld = new World(new System.Numerics.Vector2(0, -9.8f));
            foreach (Gameobject gameobject in Gameobjects)
            {
                foreach (Component item in gameobject.components)
                {
                    if(item.Type == "Rigidbody")
                    {
                        BodyDef bodyDef = new BodyDef();
                        RigidBody rb = (RigidBody)item;
                        bodyDef.BodyType = rb.BodyType;
                        bodyDef.Position = rb.Parent.transform.position;

                        Body body = physicsWorld.CreateBody(bodyDef);
                        body.IsFixedRotation = rb.FixedRotation;

                        rb.runtimeBody = body;


                        PolygonShape shape = new PolygonShape();

                        shape.SetAsBox(gameobject.transform.size.X * .5f, gameobject.transform.size.Y * .5f);
                        FixtureDef fixtureDef = new FixtureDef();
                        fixtureDef.Shape = shape;
                        fixtureDef.Density = 1.0f;
                        fixtureDef.Friction = 0.5f;
                        fixtureDef.Restitution = .0f;
                        fixtureDef.RestitutionThreshold = 0.5f;

                        body.CreateFixture(fixtureDef);
                    }
                }
            }
        }

        private void GameUpdate(double dt)
        {
            int velocityItterations = 6;
            int positionItterations = 2;

            physicsWorld.Step((float)dt, velocityItterations, positionItterations);

            foreach (Gameobject obj in Gameobjects)
            {
                obj.GameUpdate(dt);
            }
        }

        private void StopPlay()
        {            
            Engine.LoadScene(this.ScenePath);
        }

        internal virtual void Init(Engine engine, string scenePath, int width, int height)
        {
            ScenePath = scenePath;

            GameRenderer.Init();
        }

        internal virtual void EditorUpdate(double dt) 
        {

            if(Input.KeyDown(Keys.LeftControl)){
                if (Input.KeyPress(Keys.S))
                {
                    Engine.SaveScene(this);
                }
            }            

            foreach (Gameobject obj in Gameobjects) { obj.EditorUpdate(dt); }
            if (IsPlaying) GameUpdate(dt);
            //if (IsPlaying) { foreach (Gameobject obj in Gameobjects) { obj.GameUpdate(dt); } }
        }

        internal virtual void Render(double dt) {
            GameRenderer.Render();
        }

        internal virtual void OnClose() {

            GameRenderer.OnClose();
        }

        internal void AddGameObjectToScene(Gameobject go)
        {
            Gameobjects.Add(go);
            go.Init();
            go.Start();
            SelectedGameobject = go;
        }

        private string _tempProjectPath = "";
        private string _tempProjectName = "";

        //TODO: ABSTRACT THIS
        private void TEMP_CREATE_AND_SAVE_MENU()
        {
            ImGui.Begin("test");
            if (ImGui.Button("save scene"))
            {
                Engine.SaveScene(this);
            }

            if (ImGui.Button("New Project"))
            {
                ImGui.OpenPopup("Create New Project");
                _tempProjectPath = ProjectSettings.s_ProjectLocation;
                _tempProjectName = ProjectSettings.s_ProjectName;
            }

            if (ImGui.BeginPopupModal("Create New Project"))
            {

                ImGui.Text("Enter project details:");
                ImGui.InputText("Project File Path", ref _tempProjectPath, 256);
                ImGui.SameLine();

                if (ImGui.Button("..."))
                {
                    NfdDialogResult res = Nfd.PickFolder();
                    if (res.Path != null)
                        _tempProjectPath = res.Path;
                }

                ImGui.InputText("Project Name", ref _tempProjectName, 256);
                if (ImGui.Button("OK"))
                {
                    ProjectSettings.s_ProjectLocation = _tempProjectPath;
                    ProjectSettings.s_ProjectName = _tempProjectName;
                    ProjectSettings.s_FullProjectPath = ProjectSettings.s_ProjectLocation + "/" + ProjectSettings.s_ProjectName;

                    Console.WriteLine("Create new project" + ProjectSettings.s_ProjectLocation);

                    if (!Directory.Exists(ProjectSettings.s_FullProjectPath))
                    {
                        Directory.CreateDirectory(ProjectSettings.s_FullProjectPath);
                    }
                    else
                    {
                        throw new Exception("Project dir not empty");
                    }

                    Utils.CopyDirectory(Utils.GetBaseEngineDir() + "\\ExampleProject\\", ProjectSettings.s_FullProjectPath, true);

                    //Engine.CreateNewProject(ProjectSettings.s_FullProjectPath + "\\DefaultScenes\\example.kdbscene");
                    Engine.CreateNewProject(_tempProjectPath, _tempProjectName);

                    ImGui.CloseCurrentPopup();
                }

                ImGui.SameLine();

                if (ImGui.Button("Cancel"))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
            
            ImGui.End();
        }

        #region inputs
        internal virtual void OnResized(ResizeEventArgs newSize)
        {
            GameRenderer.OnResize(newSize);
            Engine.Get().ImGuiController.WindowResized(newSize.Size.X, newSize.Size.Y);
        }

        internal virtual void OnMouseWheel(MouseWheelEventArgs mouseWheelEventArgs)
        {
            Engine.Get().ImGuiController.MouseScroll(mouseWheelEventArgs.Offset);
        }

        internal virtual void OnTextInput(TextInputEventArgs inputEventArgs)
        {
            Engine.Get().ImGuiController.PressChar((char)inputEventArgs.Unicode);
        }
        #endregion
    }
}
