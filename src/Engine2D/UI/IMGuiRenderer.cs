using Engine2D.Rendering;
using Engine2D.UI;
using ImGuiNET;
using KDBEngine.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KDBEngine.UI
{
    public  class IMGuiRenderers
    {
         ImGuiController _controller = null;
         bool _initialized = false;
         List<ImGuiWindow> _windows = new List<ImGuiWindow>();

        private GameViewport _gameViewport = new GameViewport();
        private FrameBuffer _frameBuffer;

        public  void Init(Vector2i clientSize)
        {
            _controller = new ImGuiController(clientSize.X, clientSize.Y);
            _gameViewport = new GameViewport();
            _frameBuffer = new(clientSize.X, clientSize.Y);
            
            _initialized = true;
        }

        public  void Update()
        {
            if(!_initialized) { return; }
        }


        internal void Render(GameRenderer renderer, Core.Window window, double dt)
        {
            if (!_initialized) { return; }

            {
                _frameBuffer.Bind();
                renderer.Render();
                _frameBuffer.UnBind();
            }

            _controller.Update(window, dt);
            ImGui.DockSpaceOverViewport();      

            foreach(ImGuiWindow windows in _windows)
            {
                windows.Render();
            }            
                        

            _controller.Render();
            ImGuiController.CheckGLError("End of frame");
        }

        public  void WindowResized(Vector2i size)
        {
            if (!_initialized) { return; }

            // Tell ImGui of the new size
            _controller.WindowResized(size.X, size.Y);
            _frameBuffer = new FrameBuffer(size.X, size.Y);
        }

        public void OnTextInput(TextInputEventArgs c) {
            if (!_initialized) { return; }

            _controller.PressChar((char)c.Unicode);

        }

        public  void OnMouseWheel(MouseWheelEventArgs e) {
            if (!_initialized) { return; }

            _controller.MouseScroll(e.Offset);
        }

        public  void OnClose()
        {
            if (!_initialized) { return; }
        }


        public  void AddWindow(ImGuiWindow window)
        {
            _windows.Add(window);
        }

        public  void CreateDemoWindow(string name)
        {
            Console.WriteLine("Create demo window");
            var simpleWindow = new ImGuiWindow(name, 
                ImGuiWindowFlags.None |
                ImGuiWindowFlags.AlwaysAutoResize,
                () =>
                {
                    ImGui.Text("this is an demo window");
                    ImGui.Button("demo button");
                });

            AddWindow(simpleWindow);
        }
    }
}
