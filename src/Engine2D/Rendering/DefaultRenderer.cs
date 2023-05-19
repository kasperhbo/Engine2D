using System.Numerics;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Rendering.Buffers;
using KDBEngine.Core;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.Rendering;

public class DefaultRenderer : Renderer<DefaultRenderBatch>
{
    private static int MAX_BATCH_SIZE = 1000;

    // The light data
    private List<PointLight> lights = new();
    private int numberOfLights = 0;

    protected override Shader CreateShader()
    {
        ShaderData shaderData = new ShaderData();
        shaderData.FragPath = Utils.GetBaseEngineDir() + "/Shaders/default.frag";
        shaderData.VertexPath = Utils.GetBaseEngineDir() + "/Shaders/default.vert";
        return ResourceManager.GetShader(shaderData);
    }

    protected override Framebuffer CreateFramebuffer()
    {
        return Framebuffer.CreateDefault();
    }

    protected override void Prepare()
    {
        GL.ClearColor(1, 0, 0, 1);
    }

    protected override void UploadUniforms(Shader shader)
    {
        shader.UploadIntArray("uTextures", m_textureSlots);

        // This is here so that all renderers can have different cameras OR no cameras at all
        shader.uploadMat4f("uProjection", TempEngine.Get.camera.getProjectionMatrix());
        shader.uploadMat4f("uView", TempEngine.Get.camera.getViewMatrix());

        // Set lighting uniforms
        OpenTK.Mathematics.Vector2[] lightPositions = new OpenTK.Mathematics.Vector2[numberOfLights];
        OpenTK.Mathematics.Vector3[] lightColors = new OpenTK.Mathematics.Vector3[numberOfLights];
        float[] lightIntensities = new float[numberOfLights];

        for (int i = 0; i < numberOfLights; i++) {
            PointLight light = lights[i];
            lightPositions[i] = new(light.LastTransform.position.X, light.LastTransform.position.Y);
            lightColors[i] = new(light.Color.Color.X, light.Color.Color.Y, light.Color.Color.Z);
            lightIntensities[i] = light.Intensity;
        }

        shader.uploadVec2fArray("uLightPosition", lightPositions);
        shader.uploadVec3fArray("uLightColor", lightColors);
        shader.uploadFloatArray("uIntensity", lightIntensities);
        shader.uploadFloat("uMinLighting", .3f);
        shader.uploadInt("uNumLights", numberOfLights);
    }

    public override void Add(Gameobject gameobject)
    {
        SpriteRenderer spr = gameobject.GetComponent<SpriteRenderer>();
        if (spr != null) {
            AddSpriteRenderer(spr);
        }

        PointLight light = gameobject.GetComponent<PointLight>();
        if (light != null) {
            AddPointLight(light);
        }
    }
    
    protected void AddSpriteRenderer (SpriteRenderer sprite) {
        foreach (DefaultRenderBatch batch in _batches) {
            if (batch.AddSprite(sprite)) {
                return;
            }
        }
        // If unable to add to previous batch, create a new one
        DefaultRenderBatch newBatch = new DefaultRenderBatch(MAX_BATCH_SIZE, sprite.ZIndex);
        
        newBatch.Start();
        _batches.Add(newBatch);
        newBatch.AddSprite(sprite);
        _batches.Sort();
    }
    
    private void AddPointLight(PointLight light) {
        numberOfLights++;
        lights.Add(light);
    }
}