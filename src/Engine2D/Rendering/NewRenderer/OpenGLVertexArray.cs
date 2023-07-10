using Engine2D.Logging;
using OpenTK.Graphics.OpenGL;

namespace Engine2D.Rendering.NewRenderer;


internal enum ShaderDataType
{
    None = 0, Float, Float2, Float3, Float4, Mat3, Mat4, Int, Int2, Int3, Int4, Bool
};


internal static class RenderHelpers
{
    
    internal static int ShaderDataTypeSize(ShaderDataType type)
    {
        switch (type)
        {
            case ShaderDataType.Float:    return 4;
            case ShaderDataType.Float2:   return 4 * 2;
            case ShaderDataType.Float3:   return 4 * 3;
            case ShaderDataType.Float4:   return 4 * 4;
            case ShaderDataType.Mat3:     return 4 * 3 * 3;
            case ShaderDataType.Mat4:     return 4 * 4 * 4;
            case ShaderDataType.Int:      return 4;
            case ShaderDataType.Int2:     return 4 * 2;
            case ShaderDataType.Int3:     return 4 * 3;
            case ShaderDataType.Int4:     return 4 * 4;
            case ShaderDataType.Bool:     return 1;
        }
        
        Log.Error("Unknown ShaderDataType!");
        return 0;
    }
    
        
    internal static VertexAttribType ShaderDataTypeToOpenGLBaseType(ShaderDataType type)
    {
        switch (type)
        {
            case ShaderDataType.Float:    return VertexAttribType.Float;
            case ShaderDataType.Float2:   return VertexAttribType.Float;
            case ShaderDataType.Float3:   return VertexAttribType.Float;
            case ShaderDataType.Float4:   return VertexAttribType.Float;
            case ShaderDataType.Mat3:     return VertexAttribType.Float;
            case ShaderDataType.Mat4:     return VertexAttribType.Float;
            case ShaderDataType.Int:      return VertexAttribType.Int;
            case ShaderDataType.Int2:     return VertexAttribType.Int;
            case ShaderDataType.Int3:     return VertexAttribType.Int;
            case ShaderDataType.Int4:     return VertexAttribType.Int;
            //case ShaderDataType.Bool:     return VertexAttribType.Float;
        }

        Log.Error("Unknown ShaderDataType!");
        return 0;
    }

}

public class OpenGLVertexArray : IDisposable
{
    int m_RendererID;
    List<OpenGLVertexBuffer> m_VertexBuffers = new();
    OpenGLIndexBuffer m_IndexBuffer;
    
    internal OpenGLVertexArray()
    {
        GL.CreateVertexArrays(1, out m_RendererID);
    }
    
    public void Dispose()
    {
        GL.DeleteVertexArray(m_RendererID);
    }
    
    internal void Bind()
    {
        GL.BindVertexArray(m_RendererID);
    }
    
    internal void Unbind()
    {
        GL.BindVertexArray(0);
    }
    
    internal void AddVertexBuffer(OpenGLVertexBuffer vertexBuffer)
    {
        m_VertexBuffers.Add(vertexBuffer);
    }

    internal void SetIndexBuffer(OpenGLIndexBuffer indexBuffer)
    {
        m_IndexBuffer = indexBuffer;
    }
    
    internal OpenGLIndexBuffer GetIndexBuffer()
    {
        return m_IndexBuffer;
    }

    internal static OpenGLVertexArray Create()
    {
        return new OpenGLVertexArray();
    }

    
    
}