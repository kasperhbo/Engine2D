using Engine2D.Rendering;
using KDBEngine.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Core
{
    internal static class Input
    {
        static KeyboardState keyboardState;
        static MouseState mouse;

        internal static Vector2 GameViewportPos = new();
        internal static Vector2 GameViewportSize = new();
        
        internal static void Update(KeyboardState state, MouseState mouseState)
        {
            keyboardState = state;
            mouse = mouseState;
        }

        public static bool KeyPress(Keys key)
        {
            return keyboardState.IsKeyPressed(key);
        }

        public static bool KeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        public static Vector2 MouseLocation()
        {
            return mouse.Position;
        }

        public static Vector2 GetWorld()
        {
            float currentX = mouse.Position.X - GameViewportPos.X;
            currentX = (2.0f * (currentX / GameViewportSize.X)) - 1f;

            float currentY = mouse.Position.Y - GameViewportPos.Y;
            currentY = -((2.0f * (1f - (currentY / GameViewportSize.Y))) - 1f);

            OrthographicCamera camera = GameRenderer.S_CurrentCamera;
            Vector4 temp = new Vector4(currentX, currentY, 0, 1f);

            Matrix4 inverseView = camera.InverseView;
            Matrix4 inverseProj = camera.InverseProjection;

            Matrix4 viewProjection = Matrix4.Mult(inverseView, inverseProj);

            Vector4 outV = MathHelper.Multiply(viewProjection, temp);

            return new Vector2(outV.X, outV.Y - .1f);
        }
        //internal static void IsKeyUp(KeyboardKeyEventArgs e)
        //{
        //    while (_keysDown.Contains(e.Key))
        //        _keysDown.Remove(e.Key);

        //    while (_keysDownLast.Contains(e.Key))
        //        _keysDownLast.Remove(e.Key);
        //}

        //internal static void IsKeyDown(KeyboardKeyEventArgs e)
        //{
        //    if (!_keysDown.Contains(e.Key))
        //    {             
        //        _keysDown.Add(e.Key);
        //    }
        //    else if (!_keysDownLast.Contains(e.Key))
        //    {
        //        _keysDownLast.Add(e.Key);
        //    }
        //}

        //internal static void MouseEnter()
        //{

        //}

        //internal static void MouseLeave()
        //{

        //}

        //internal static void MouseMove(MouseMoveEventArgs obj)
        //{

        //}

        //internal static void MouseUp(MouseButtonEventArgs obj)
        //{

        //}

        //internal static void MouseWheel(MouseWheelEventArgs obj)
        //{

        //}

        //internal static void MouseDown(MouseButtonEventArgs e)
        //{

        //}

        //public static bool IsKeyPressed(Keys key)
        //    {
        //        return (
        //                _keysDown.Contains(key) 
        //                && 
        //                !_keysDownLast.Contains(key)
        //            );
        //    }

        //public static bool KeyDown(Keys key)
        //    {
        //        return (_keysDown.Contains(key));
        //    }
    }
}
