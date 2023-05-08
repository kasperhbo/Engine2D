﻿using Engine2D.Core;
using Engine2D.GameObjects;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Rendering
{
    /// <summary>
    /// Old code that I got from following GAMES WITH GABE'S How to create a game engine in Java Series!
    /// Thanks Gabe, your code is now my property;)
    /// </summary>
    internal class RenderBatch 
    {
        #region fields
        private const int c_MaxBatchSize = 20000;

        private const int c_PosSize = 2;
        private const int c_ColorSize = 4;

        private const int c_PosOffset = 0;
        private const int c_ColorOffset = c_PosOffset + c_PosSize * sizeof(float);
        private const int c_VertexSize = 6;
        private const int c_VertexSizeInBytes = c_VertexSize* sizeof(float);

        private SpriteRenderer[] sprites;
        private int spriteCount = 0;
        public bool HasRoom => spriteCount < c_MaxBatchSize;

        private float[] _vertices = new float[c_MaxBatchSize * c_VertexSize];
        private float[] _vertices2 = new float[c_VertexSize];
        private int _vaoID, _vboID;

        private Shader _shader;
        #endregion
        
        internal RenderBatch()
        {
            _shader = new Shader(Utils.GetBaseEngineDir() + "/Shaders/default.vert", Utils.GetBaseEngineDir() + "/Shaders/default.frag");
            sprites = new SpriteRenderer[c_MaxBatchSize];

            _vertices = new float[c_MaxBatchSize*c_VertexSize*4];            
        }

        internal void Init()
        {
            _vaoID = GL.GenVertexArray();
            GL.BindVertexArray(_vaoID);

            _vboID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.DynamicDraw);

            int eboID = GL.GenBuffer();
            int[] indices = GenerateIndices();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

            // Enable the buffer attribute pointers
            GL.VertexAttribPointer(0, c_PosSize, VertexAttribPointerType.Float, false, c_VertexSizeInBytes, c_PosOffset);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, c_ColorSize, VertexAttribPointerType.Float, false, c_VertexSizeInBytes, c_ColorOffset);
            GL.EnableVertexAttribArray(1);
        }

        public void AddSprite(SpriteRenderer spr)
        {
            int index = spriteCount;
            sprites[index] = spr;
            spriteCount++;

            LoadVertexProperties(index);
        }

        public void Render(Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            // For now, we will rebuffer all data every frame
            bool rebufferData = false; 
            
            for (int i = 0; i < spriteCount; i++)
            {
                if (sprites[i].IsDirty)
                {
                    LoadVertexProperties(i);
                    rebufferData = true;
                }
            }
            
            if(rebufferData)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vboID);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, _vertices.Length * sizeof(float), _vertices);
            }
            
            // Use shader
            _shader.use();
            _shader.uploadMat4f("uProjection", projectionMatrix);
            _shader.uploadMat4f("uView", viewMatrix);

            GL.BindVertexArray(_vaoID);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.DrawElements(PrimitiveType.Triangles, this.spriteCount * 6, DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.BindVertexArray(0);

            _shader.detach();
        }


        private void LoadVertexProperties(int index)
        {
            SpriteRenderer sprite = this.sprites[index];

            // Find offset within array (4 vertices per sprite)
            int offset = index * 4 * c_VertexSize;

            Vector4 color = new Vector4(sprite.Color.X, sprite.Color.Y, sprite.Color.Z, sprite.Color.W);


            // Add vertices with the appropriate properties
            float xAdd = 0.5f;
            float yAdd = 0.5f;
            
            for (int i=0; i < 4; i++) {
                    
                if (i == 1) {
                    yAdd = -0.5f;
                } else if (i == 2) {
                    xAdd = -0.5f;
                } else if (i == 3) {
                    yAdd = 0.5f;
                }

                // Load position
                _vertices[offset] = sprite.Parent.transform.position.X + 
                    xAdd * (sprite.Parent.transform.size.X);
                _vertices[offset + 1] = sprite.Parent.transform.position.Y + 
                    yAdd * (sprite.Parent.transform.size.Y);

                // Load color
                _vertices[offset + 2] = color.X;
                _vertices[offset + 3] = color.Y;
                _vertices[offset + 4] = color.Z;
                _vertices[offset + 5] = color.W;
                
                offset += c_VertexSize;
            }
        }

        private int[] GenerateIndices()
        {
            // 6 indices per quad (3 per triangle)
            int[] elements = new int[6 * c_MaxBatchSize];

            for (int i = 0; i < c_MaxBatchSize; i++)
            {
                loadElementIndices(elements, i);
            }

            return elements;
        }

        private void loadElementIndices(int[] elements, int index)
        {
            int offsetArrayIndex = 6 * index;
            int offset = 4 * index;

            // 3, 2, 0, 0, 2, 1        7, 6, 4, 4, 6, 5
            // Triangle 1
            elements[offsetArrayIndex] =     offset + 3;
            elements[offsetArrayIndex + 1] = offset + 2;
            elements[offsetArrayIndex + 2] = offset + 0;

            // Triangle 2
            elements[offsetArrayIndex + 3] = offset + 0;
            elements[offsetArrayIndex + 4] = offset + 2;
            elements[offsetArrayIndex + 5] = offset + 1;
        }

        
        public void RemoveSprite(SpriteRenderer spr)
        {
            for (int i = 0; i < spriteCount; i++)
            {
                if (sprites[i] == spr)
                {
                    for (int j = i; j < spriteCount - 1; j++)
                    {
                        sprites[j] = sprites[j + 1];
                        sprites[j].IsDirty = true;
                    }
                    spriteCount--;
                }
            }
        }
    }
}
