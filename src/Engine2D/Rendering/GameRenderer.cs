using Engine2D.GameObjects;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

namespace Engine2D.Rendering
{
    internal static class GameRenderer {

        internal static OrthographicCamera S_CurrentCamera = new(0,0);
        private static List<RenderBatch> _renderBatches = new();

        private static Dictionary<SpriteRenderer, RenderBatch> _spriteBatchDict = new();
        
        internal static void Init()
        {
            Flush();

            _renderBatches = new();
            
            // Calculate the aspect ratio
            float aspectRatio = (float)RenderSettings.s_DefaultRenderResolution.X /
                                (float)RenderSettings.s_DefaultRenderResolution.Y;

            // Calculate the size of the camera
            float size = RenderSettings.s_DefaultRenderResolution.Y / 10f;

            S_CurrentCamera = new OrthographicCamera(aspectRatio, size);
            
            foreach (var rb in _renderBatches) rb.Init();
        }

        internal static void Flush()
        {
            _renderBatches.Clear();
            _spriteBatchDict.Clear();
            S_CurrentCamera = null;
        }

        internal static void Render()
        {            
            GL.ClearColor(S_CurrentCamera.ClearColor);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            foreach (var rb in _renderBatches) rb.Render(S_CurrentCamera.ProjectionMatrix, S_CurrentCamera.ViewMatrix);
        }

        internal static void Update(double dt)
        {
            
        }

        internal static void OnClose() { }

        internal static void OnResize(ResizeEventArgs size) { }

        internal static void AddSpriteRenderer(SpriteRenderer spr)
        {
            bool added = false;
            RenderBatch addedToBatch = null;
            Console.WriteLine("add");
            foreach (var batch in _renderBatches)
            {
                if(batch.HasRoom)
                {
                    added = true;
                    batch.AddSprite(spr);
                    addedToBatch = batch;
                }
            }

            if (!added)
            {
                RenderBatch batch = new RenderBatch();
                batch.Init();
                batch.AddSprite(spr);
                _renderBatches.Add(batch);
                addedToBatch = batch;
            }
           
        }

        internal static void RemoveSprite(SpriteRenderer spr)
        {
            _spriteBatchDict[spr].RemoveSprite(spr);
            _spriteBatchDict.Remove(spr);
        }
    }
}
