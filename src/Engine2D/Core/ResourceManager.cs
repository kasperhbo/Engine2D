using Engine2D.Components;
using Engine2D.Rendering;
using KDBEngine.Shaders;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Core
{
    internal struct ShaderData
    {
        internal string FragPath;
        internal string VertexPath;
    }

    [JsonConverter(typeof(ComponentSerializer))]
    public class TextureData
    {
        public string texturePath;
        public bool flipped;
        public TextureMinFilter minFilter;
        public TextureMagFilter magFilter;

        public string Type = "TextureData";

        public TextureData()
        {
        }

        public TextureData(string texturePath, bool flipped, TextureMinFilter minFilter, TextureMagFilter magFilter)
        {
            this.texturePath = texturePath;
            this.flipped = flipped;
            this.minFilter = minFilter;
            this.magFilter = magFilter;
        }
    }


    internal static class ResourceManager
    {
        private static Dictionary<ShaderData, Shader> _shaders = new Dictionary<ShaderData, Shader>();
        private static Dictionary<TextureData, Texture> _textures = new Dictionary<TextureData, Texture>();

        internal static Shader GetShader(ShaderData shaderLocations)
        {
            Shader shader;
            if (!_shaders.TryGetValue(shaderLocations, out shader))
            {
                shader = new Shader(shaderLocations.VertexPath, shaderLocations.FragPath);
                _shaders.Add(shaderLocations, shader);
            }

            return shader;
        }

        internal static Texture GetTexture(TextureData textureData)
        {
            Texture tex;
            if (!_textures.TryGetValue(textureData, out tex))
            {
                tex = new Texture(textureData.texturePath, textureData.flipped, textureData.minFilter, textureData.magFilter);
                _textures.Add(textureData, tex);
            }

            return tex;
        }


    }
}
