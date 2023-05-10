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
            float currentY = mouse.Position.Y - GameViewportPos.Y;;


            currentX = (currentX / GameViewportSize.X) *2- 1.0f;           
            currentY = -((currentY / GameViewportSize.Y) *2- 1.0f);
            Console.WriteLine(GameViewportSize.Y);

            Vector4 tmp = new Vector4(currentX, currentY, 0.0f, 1.0f);

            //OrthographicCamera camera = GameRenderer.S_CurrentCamera;
            //Matrix4 viewProjection = Matrix4.Mult(camera.InverseView, camera.InverseProjection);
            //Vector4 t= MathHelper.Multiply(viewProjection, tmp);
            Vector4 d = new();

            //MathHelper.Test(tmp, viewProjection, out d);
                        

            return new(d.X, d.Y);
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
