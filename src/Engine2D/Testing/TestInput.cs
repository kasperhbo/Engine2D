﻿using KDBEngine.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Testing
{
    internal class TestInput
    {

        private static Vector2 lastPos;
        private static double worldX;
        private static double worldY;
        private static Vector2 world;
        private static double lastWorldX;
        private static double lastWorldY;
        private static Vector2 lastWorld;

        private static Vector2 viewportPos = new Vector2();
        private static Vector2 viewportSize = new Vector2();

        private static double xPos;
        private static double yPos;
        private static Vector2 pos;

        private static double lastX;
        private static double lastY;

        public static void Init()
        {
            xPos = 0.0;
            yPos = 0.0;
            pos = new Vector2(0.0f, 0.0f);

            lastX = 0.0;
            lastY = 0.0;
            lastPos = new Vector2(0.0f, 0.0f);

            worldX = 0.0;
            worldY = 0.0;
            world = new Vector2(0.0f, 0.0f);

            lastWorldX = 0.0;
            lastWorldY = 0.0;
            lastWorld = new Vector2(0.0f, 0.0f);

            viewportPos = new Vector2();
            viewportSize = new Vector2(Engine.Get().getWidth(), Engine.Get().getHeight());
        }

        public static void mousePosCallback(double xpos, double ypos)
        {
            //if (.mouseButtonDown > 0)
            //{
            //    isDragging = true;
            //}

            lastPos = (pos); // Useless since endFrame() ??
            lastX = xPos; // Useless since endFrame() ??
            lastY = yPos; // Useless since endFrame() ??

            lastPos = (pos);  // Useless since endFrame() ??
            lastWorldX = worldX;  // Useless since endFrame() ??
            lastWorldY = worldY;  // Useless since endFrame() ??

            pos = new((float)xpos, (float)ypos);
            xPos = xpos;// Delete
            yPos = ypos;// Delete

            calcOrtho();
        }

        public static void endFrame()
        {
            lastPos = (pos);
            lastX = xPos; // Delete
            lastY = yPos; // Delete

            lastWorld = (world);
            lastWorldX = worldX; // Delete
            lastWorldY = worldY; // Delete
        }

        public static float getX()
        {
            //return (float) get().xPos;
            return pos.X;
        }
        public static float getY()
        {
            //return (float) get().yPos;
            return pos.Y;
        }

        private static void calcOrtho()
        {
            float currentX = getX() - viewportPos.X;
            float currentY = getY() - viewportPos.Y;
            //System.out.println(get().viewportSize.x);
            currentX = (currentX / viewportSize.X) * 2.0f - 1.0f;
            currentY = -((currentY / viewportSize.Y) * 2.0f - 1.0f);

            // vec.w must be 1! (Vector multiplication)
            Vector4 tmp = new Vector4(currentX, currentY, 0.0f, 1.0f);

            TestCamera camera = Engine.Get().testCamera;
            Matrix4 viewProjection = new Matrix4();

            viewProjection = Matrix4.Mult(camera.getInverseViewMatrix(), camera.getInverseProjection());
            Vector4 t = MathHelper.Multiply(viewProjection, tmp);
            world = new(t.X, t.Y);
        }

        public static void setViewportPos(Vector2 gameViewportPos)
        {
            viewportPos = (gameViewportPos);
        }

        public static void setViewportSize(Vector2 gameViewportSize)
        {
            viewportSize = (gameViewportSize);
        }

        public static Vector2 getWorld()
        {
            return world;
        }

    }
}