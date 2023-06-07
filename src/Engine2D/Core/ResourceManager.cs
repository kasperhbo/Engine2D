using Engine2D.Components;
using Engine2D.Rendering;
using KDBEngine.Shaders;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp.Processing.Processors.Dithering;

namespace Engine2D.Core;

internal struct ShaderData
{
    internal string FragPath;
    internal string VertexPath;
}

internal static class ResourceManager
{
    private static readonly Dictionary<ShaderData, Shader> _shaders = new();

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
}