using System.Numerics;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.Rendering.NewRenderer;

public class Renderer
{
    public static void BeginScene()
    {
        
    }

    public static void EndScene()
    {
        
    }
    
    public static void Submit(OpenGLVertexArray vertexAray)
    {
        vertexAray.Bind();
        OpenGLRenderApi.DrawIndexed(vertexAray);
    }
}

internal static class OpenGLRenderApi
{
    internal static void SetClearColor(Vector4 color)
    {
        GL.ClearColor(color.X, color.Y, color.Z, color.W);
    }

    internal static void Clear()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    internal static void DrawIndexed(OpenGLVertexArray vertexArray)
    {
        var indexBuffer = vertexArray.GetIndexBuffer();
        var count = indexBuffer.GetCount();
        
        GL.DrawElements(BeginMode.Triangles, count, DrawElementsType.UnsignedInt, 0);
        // GL.DrawElements(PrimitiveType.Triangles, .GetCount(), DrawElementsType.UnsignedInt, 0);
    }
}