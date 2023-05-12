using Box2DSharp.Collision.Shapes;
using Box2DSharp.Common;
using Box2DSharp.Dynamics;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
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
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.Scenes
{
    internal class LightSettings
    {
        public float GlobalLightIntensity = 1;
        public SpriteColor ClearColor = new SpriteColor(.19f,.19f,.19f,1f);
        // public Vector4 ClearColor = new(.19f,.19f,.19f,1f);
    }
    internal class Scene
    {
        internal string ScenePath { get; private set; } = "NoScene";
        internal List<Gameobject> Gameobjects { get; private set; } = new List<Gameobject>();

        public LightSettings LightSettings = new();
        
        
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
                    SaveLoad.SaveScene(this);
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

        internal void GameUpdate(double dt)
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
            //SaveLoad.LoadScene(this.ScenePath);
        }

        internal virtual void Init(Engine engine, string scenePath, int width, int height)
        {
            ScenePath = scenePath;

            GameRenderer.Init();
        }

        internal virtual void EditorUpdate(double dt) 
        {

            if(TestInput.KeyDown(Keys.LeftControl)){
                if (TestInput.KeyPress(Keys.S))
                {
                    if(IsPlaying) { return;  }
                    SaveLoad.SaveScene(this);
                }
            }            

            foreach (Gameobject obj in Gameobjects) { obj.EditorUpdate(dt); }
            if (IsPlaying) GameUpdate(dt);
            //if (IsPlaying) { foreach (Gameobject obj in Gameobjects) { obj.GameUpdate(dt); } }
        }

        internal virtual void Render(double dt) {
            GameRenderer.Render();
        }

        internal void AddGameObjectToScene(Gameobject go)
        {
            Gameobjects.Add(go);
            go.Init();
            go.Start();
            Engine.Get().CurrentSelectedAsset = go;
        }

        internal virtual void OnClose() {
            SaveLoad.SaveScene(this);
            GameRenderer.OnClose();
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

        internal void OnGui()
        {
            ImGui.Begin("Scene Settings");
            
            OpenTKUIHelper.DrawComponentWindow("Light Settings", "Light Settings", () =>
            {
                OpenTKUIHelper.DrawProperty("Global Light Intensity", ref LightSettings.GlobalLightIntensity, 0, 1,
                    dragSpeed: 0.01f);
                OpenTKUIHelper.DrawProperty("Enviroment Color", ref LightSettings.ClearColor);
            });
            
            ImGui.End();
        }
    }
}
