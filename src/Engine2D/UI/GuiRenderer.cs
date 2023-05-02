using ImGuiNET;
using KDBEngine.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KDBEngine.UI
{
    internal static class GuiRenderer
    {
        static ImGuiController _controller = null;
        static bool _initialized = false;
        static List<ImGuiWindow> _windows = new List<ImGuiWindow>();

        public static void Init(Vector2i clientSize)
        {
            _controller = new ImGuiController(clientSize.X, clientSize.Y);
            _initialized = true;

            CreateDemoWindow();
        }

        public static void Update(Window window, float dt)
        {
            if(!_initialized) { return; }

            _controller.Update(window, dt);
        }

        public static void Render()
        {
            if (!_initialized) { return; }

            ImGui.DockSpaceOverViewport();
            ImGui.ShowDemoWindow();

            foreach(ImGuiWindow window in _windows)
            {
                window.Render();
            }

            _controller.Render();
        }

        public static void WindowResized(Vector2i size)
        {
            if (!_initialized) { return; }

            // Tell ImGui of the new size
            _controller.WindowResized(size.X, size.Y);
        }

        public static void CharacterPressed(TextInputEventArgs c) {
            if (!_initialized) { return; }

            _controller.PressChar((char)c.Unicode);

            if((char)c.Unicode == 'c')
            {
                _windows[0].SetWindowContent(() =>
                {
                    ImGui.Text("new");
                });
            }
        }

        public static void OnMouseWheel(MouseWheelEventArgs e) {
            if (!_initialized) { return; }

            _controller.MouseScroll(e.Offset);
        }

        public static void OnClose()
        {
            if (!_initialized) { return; }
        }


        public static void AddWindow(ImGuiWindow window)
        {
            _windows.Add(window);
        }

        public static void CreateDemoWindow()
        {
            var simpleWindow = new ImGuiWindow("demo window", 
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
