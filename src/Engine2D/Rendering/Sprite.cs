using Engine2D.Core;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Rendering
{
    internal class Sprite : Asset
    {
        [JsonIgnore]public Texture? Texture = null;

        public Vector2 TextureCoords = new Vector2(1,1);        

        public TextureData TextureData = new(); 
        public bool IsDirty = false;

        public string Type = "Sprite";

        public void Init(Vector2 textureCoords, TextureData textureData)
        {
            this.TextureCoords = textureCoords; 
            this.TextureData = textureData;
            Texture = ResourceManager.GetTexture(TextureData);
        }

        internal override void OnGui()
        {
               
        }
    }
}
