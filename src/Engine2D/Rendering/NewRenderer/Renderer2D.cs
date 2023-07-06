using Engine2D.Cameras;
using Octokit;
using OpenTK.Mathematics;

namespace Engine2D.Rendering;

internal static class Renderer2D
{
    internal struct Statistics
    {
        internal uint DrawCalls;
        internal uint QuadCount;
        internal uint GetTotalIndexCount()  { return QuadCount * 6; }
        internal uint GetTotalVertexCount() { return QuadCount * 4; }

        public Statistics()
        {
            DrawCalls = 0;
            QuadCount = 0;
        }
    }
    
    struct QuadVertex
    {
        internal Vector3 Position;
        internal Vector4 Color;
        internal Vector2 TexCoord;
        
        internal float TexIndex;
        internal float TilingFactor;
        
        internal int EntityID;
    }

    struct CircleVertex
    {
        internal Vector3 Position;
        internal Vector4 Color;
        internal Vector2 TexCoord;
        
        internal float Thickness;
        internal float Fade;
        
        internal int EntityID;
    }
    
    struct LineVertex
    {
        internal Vector3 Position;
        internal Vector4 Color;
        
        internal int EntityID;
    }
    
    struct Renderer2DData
    {
        const int MaxQuads = 10000;
        const int MaxVertices = 4 * MaxQuads;
        const int MaxIndices = 6 * MaxQuads;
        const int MaxTextureSlots = 32; // TODO: RenderCaps

        VertexArray QuadVertexArray;
        VertexBuffer QuadVertexBuffer;
        Shader QuadShader;
        Texture2D WhiteTexture;
        
        VertexArray CircleVertexArray;
        VertexBuffer CircleVertexBuffer;
        Shader CircleShader;
        
        VertexArray LineVertexArray;
        VertexBuffer LineVertexBuffer;
        Shader LineShader;
        
        uint QuadIndexCount = 0;
        QuadVertex QuadVertexBufferBase;
        QuadVertex QuadVertexBufferPtr;
        
        uint CircleIndexCount = 0;
        CircleVertex CircleVertexBufferBase;
        CircleVertex CircleVertexBufferPtr;
        
        uint LineIndexCount = 0;
        LineVertex LineVertexBufferBase;
        LineVertex LineVertexBufferPtr;
        
        float LineWidth = 2.0f;
        
        internal Texture2D[] TextureSlots = new Texture2D[MaxTextureSlots];
        internal int TextureSlotIndex = 1; // 0 = white texture
        
        Vector4[] QuadVertexPosition = new Vector4[4];

        private Renderer2D.Statistics Stats;

        struct CameraData
        {
            Matrix4 viewProjectionMatrix;
        }

        CameraData CameraBuffer;
        UniformBuffer CameraUniformBuffer;

        public Renderer2DData()
        {
        }
    }
    
    static Renderer2DData _data = new();
    
    private static Statistics _stats = new();
    
    internal static void ResetStats()
    {
        _stats.DrawCalls = 0;
        _stats.QuadCount = 0;
    }
    
    internal static Statistics GetStats()
    {
        return _stats;
    }
    
    
    internal static void Init()
    {

    }

    internal static void Shutdown()
    {

    }

    internal static void BeginScene(Camera camera, Matrix4 transform)
    {

    }

    internal static void BeginScene(Camera editorCamera)
    {

    }

    internal static void EndScene()
    {

    }

    internal static void Flush()
    {

    }


    // Primitives
    internal static void DrawQuad(Vector2 position, Vector2 size, Vector4 color)
    {

    }

    internal static void DrawQuad(Vector3 position, Vector2 size, Vector4 color)
    {

    }

    internal static void DrawQuad(Vector2 position, Vector2 size, Texture
        texture, float tilingFactor = 1.0f, Vector4 tintColor = default(Vector4))
    {

    }

    internal static void DrawQuad(Vector3 position, Vector2 size, Texture
        texture, float tilingFactor = 1.0f, Vector4 tintColor = default(Vector4))
    {

    }

    internal static void DrawQuad(Matrix4 transform, Vector4 color, int entityID = -1)
    {

    }

    internal static void DrawQuad(Matrix4 transform, Texture texture, float
        tilingFactor = 1.0f, Vector4 tintColor = default(Vector4), int entityID = -1)
    {

    }


    internal static void DrawRotatedQuad(Vector2 position, Vector2 size, float rotation, Vector4 color)
    {
    }

    internal static void DrawRotatedQuad(Vector3 position, Vector2 size, float rotation, Vector4 color)
    {
    }

    internal static void DrawRotatedQuad(Vector2 position, Vector2 size, float rotation, Texture texture,
        float tilingFactor = 1.0f, Vector4 tintColor = default(Vector4))
    {
    }

    internal static void DrawRotatedQuad(Vector3 position, Vector2 size, float rotation, Texture texture,
        float tilingFactor = 1.0f, Vector4 tintColor = default(Vector4))
    {
    }

    internal static void DrawCircle(Matrix4 transform, Vector4 color, float thickness = 1.0f, float fade = 0.005f,
        int entityID = -1)
    {
    }

    internal static void DrawLine(Vector3 p0, Vector3 p1, Vector4 color, int entityID = -1)
    {
        
    }

    internal static void DrawRect(Vector3 position, Vector2 size, Vector4 color, int entityID = -1)
    {
        
    }

    internal static void DrawRect(Matrix4 transform, Vector4 color, int entityID = -1)
    {
    }

    internal static void DrawSprite(Matrix4 transform, SpriteRendererComponent src, int entityID)
    {

    }

    internal static float GetLineWidth()
    {
        return 0;
    }

    internal static void SetLineWidth(float width)
    {

    }

    private static void StartBatch()
    {
        
    }
    
    private static void NextBatch()
    {
        
    }
}

internal struct SpriteRendererComponent
{
    
}