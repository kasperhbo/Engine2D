﻿using Engine2D.Components;
using Engine2D.Core;
using Engine2D.Rendering;
using ImGuiNET;
using Newtonsoft.Json;
using System.Numerics;

namespace Engine2D.GameObjects
{
    [JsonConverter(typeof(ComponentSerializer))]
    public class SpriteRenderer : Component
    {
        public Vector4 Color
        {
            get => _color;
            set
            {
                _color = value;
                IsDirty = true;
            }
        }

//        public Sprite? sprite = null;
        private Vector2 SpriteSize = new Vector2(32, 32);

        private Transform _lastTransform = new();
        private Vector4 _color = new(1,1,1,1);
        
        internal bool IsDirty = true;

        [JsonIgnore]internal Texture texture;
        
        public TextureData? textureData;

        internal Vector2[] TextureCoords =
        {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(0, 1)
        };

        public override void Init(Gameobject parent)
        {
            base.Init(parent);
            if(this.textureData != null)
            {
                Console.WriteLine("has texture data, loading texture...." + textureData.texturePath);
                texture = ResourceManager.GetTexture(textureData);
            }
            GameRenderer.AddSpriteRenderer(this);
        }

        public override  void Start()
        {
        }

        public override void EditorUpdate(double dt)
        {
            //Console.WriteLine(this.texture?.TexID);
            if (!_lastTransform.Equals(Parent.transform))
            {
                IsDirty = true;
                Parent.transform.Copy(_lastTransform);
            }
        }

        public override void GameUpdate(double dt)
        {
            if (!_lastTransform.Equals(Parent.transform))
            {
                IsDirty = true;
                Parent.transform.Copy(_lastTransform);
            }
        }

        public override string GetItemType()
        {
            return "SpriteRenderer";
        }

        public override System.Numerics.Vector2 WindowSize()
        {
            return new System.Numerics.Vector2(0, 200);
        }

        public override void ImGuiFields()
        {

            
                if (this.texture != null)
                    ImGui.ImageButton("Sprite: ", (IntPtr)texture.TexID, new Vector2(128, 128), new Vector2(0, 1),
                new Vector2(1, 0));
                //ImGui.ImageButton("##sprite", (IntPtr)Texture.TexID, new Vector2(56, 56));
                else
                    ImGui.ImageButton("Sprite: ", IntPtr.Zero, new Vector2(128, 128));
                ImGui.TreePop();

            //if(ImGui.CollapsingHeader("Sprite Renderer"))
            //{
            //    if (ImGui.ColorPicker4("Color: ", ref _color))
            //    {
            //        IsDirty = true;
            //    }

            //ImGui.ImageButton("##sprite", IntPtr.Zero, new Vector2(56, 56));
        }
  
    }
}
