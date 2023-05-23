﻿using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Testing;
using KDBEngine.Core;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Engine2D.Rendering;

public class LightMapRenderer
{
    private readonly uint[] _indices =
    {
        // Note that indices start at 0!
        0, 1, 3, // The first triangle will be the top-right half of the triangle
        1, 2, 3 // Then the second will be the bottom-left half of the triangle
    };

    private readonly float[] _vertices =
    {
        0.5f, 0.5f, // top right
        0.5f, -0.5f, // bottom right
        -0.5f, -0.5f, // bottom left
        -0.5f, 0.5f // top left
    };

    private int _elementBufferObject;
    private TestFrameBuffer _lightMap;
    private Shader _shader;
    private int _vertexArrayObject;

    private int _vertexBufferObject;

    public void Init()
    {
        var offset = 0;
        _vertices[offset] = Engine.Get().Size.X;
        ;
        _vertices[offset + 1] = Engine.Get().Size.Y;

        _vertices[offset + 2] = Engine.Get().Size.X;
        _vertices[offset + 3] = -Engine.Get().Size.Y;
        ;


        _vertices[offset + 4] = -Engine.Get().Size.X;
        _vertices[offset + 5] = -Engine.Get().Size.Y;

        _vertices[offset + 6] = -Engine.Get().Size.X;
        _vertices[offset + 7] = Engine.Get().Size.Y;

        _lightMap = new TestFrameBuffer(Engine.Get().Size.X, Engine.Get().Size.Y);

        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
            BufferUsageHint.StaticDraw);

        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        // We also upload data to the EBO the same way as we did with VBOs.
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices,
            BufferUsageHint.StaticDraw);
        // The EBO has now been properly setup. Go to the Render function to see how we draw our rectangle now!

        var shaderData = new ShaderData
        {
            FragPath = Utils.GetBaseEngineDir() + "/Shaders/lightmap.frag",
            VertexPath = Utils.GetBaseEngineDir() + "/Shaders/lightmap.vert"
        };

        _shader = ResourceManager.GetShader(shaderData);
        _shader.use();
    }

    public Texture Render()
    {
        _lightMap.Bind();

        GL.ClearColor(1, 1, 1, 1);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        //Draw a fullscreen quad

        _shader.use();
        UploadUniforms();
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

        _lightMap.UnBind();

        return _lightMap.Texture;
    }

    public void Resize()
    {
        _lightMap = new TestFrameBuffer(Engine.Get().Size.X, Engine.Get().Size.Y);
    }

    private void UploadUniforms()
    {
        _shader.uploadMat4f("uProjection", Engine.Get().testCamera.getProjectionMatrix());
        _shader.uploadVec2f("uCameraOffset", Engine.Get().testCamera.getPosition());

        var lightsToRenderer = Renderer.GetPointLightsToRender();

        var lightPositions = new Vector2[lightsToRenderer.Count];
        var lightColors = new Vector3[lightsToRenderer.Count];

        var lightIntensities = new float[lightsToRenderer.Count];

        for (var i = 0; i < lightsToRenderer.Count; i++)
        {
            var light = lightsToRenderer[i];
            lightPositions[i] = new Vector2(light.LastTransform.position.X, light.LastTransform.position.Y);
            lightColors[i] = new Vector3(light.Color.Color.X, light.Color.Color.Y, light.Color.Color.Z);
            lightIntensities[i] = light.Intensity;
        }

        _shader.uploadVec2fArray("uLightPosition", lightPositions);
        _shader.uploadVec3fArray("uLightColor", lightColors);
        _shader.uploadFloatArray("uIntensity", lightIntensities);
        if (Engine.Get()._currentScene?.GlobalLight != null)
        {
            var currentScene = Engine.Get()._currentScene;
            if (currentScene != null)
                _shader.uploadFloat("uMinLighting", currentScene.GlobalLight.Intensity);
        }
        else
        {
            Log.Warning("No Global Light In Scene");
            _shader.uploadFloat("uMinLighting", .6f);
        }

        _shader.uploadInt("uNumLights", lightsToRenderer.Count);
    }
}