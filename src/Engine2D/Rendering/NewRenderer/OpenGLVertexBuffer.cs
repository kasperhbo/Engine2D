using Engine2D.Logging;
using OpenTK.Graphics.OpenGL;

namespace Engine2D.Rendering.NewRenderer;

internal class OpenGLVertexBuffer : IDisposable
{
    int m_RendererID;
    BufferLayout m_Layout;
    
    internal OpenGLVertexBuffer(float[] vertices, int size)
    {
        GL.CreateBuffers(1, out m_RendererID);
        GL.BindBuffer(BufferTarget.ArrayBuffer, m_RendererID);
        GL.BufferData(BufferTarget.ArrayBuffer, size, vertices, BufferUsageHint.StaticDraw);

    }

    public void Dispose()
    {
        GL.DeleteBuffer(m_RendererID);
    }
    
    internal void Bind()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, m_RendererID);
    }
    
    internal void Unbind()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }
    
    internal BufferLayout GetLayout()
    {
        return m_Layout;
    }
    
    
    internal void SetLayout(BufferLayout layout)
    {
        m_Layout = layout;
    }

    public static OpenGLVertexBuffer Create(float[] squareVertices, int size)
    {
        return new OpenGLVertexBuffer(squareVertices, size);
    }
}

internal class OpenGLIndexBuffer : IDisposable
{
    int m_RendererID;
    int m_Count;
    
    internal OpenGLIndexBuffer(int[] indices, int count)
    {
        m_Count = count;
        GL.CreateBuffers(1, out m_RendererID);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_RendererID);
        GL.BufferData(BufferTarget.ElementArrayBuffer, count * sizeof(uint), indices, BufferUsageHint.StaticDraw);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(m_RendererID);
    }
    
    internal void Bind()
    {
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_RendererID);
    }
    
    internal void Unbind()
    {
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }
    
    internal int GetCount()
    {
        return m_Count;
    }
}

internal class BufferLayout
{
    private List<BufferElement> m_Elements;
    private int m_Stride;

    internal BufferLayout()
    {
        m_Elements = new List<BufferElement>();
        m_Stride = 0;
    }

    internal BufferLayout(List<BufferElement> elements)
    {
        m_Elements = elements;
        CalculateOffsetsAndStride();
    }
    
    internal int GetStride()
    {
        return m_Stride;
    }
    
    internal List<BufferElement> GetElements()
    {
        return m_Elements;
    }

    internal List<BufferElement>.Enumerator Begin()
    {
        return m_Elements.GetEnumerator();
    }
    
    internal List<BufferElement>.Enumerator End()
    {
        return m_Elements.GetEnumerator();
    }
    
    
    private void CalculateOffsetsAndStride()
    {
        int offset = 0;
        m_Stride = 0;
        
        foreach (BufferElement element in m_Elements)
        {
            var bufferElement = element;
            bufferElement.Offset = offset;
            offset += bufferElement.Size;
            m_Stride += bufferElement.Size;
        }
    }
}

struct BufferElement
{
    public string Name;
    public ShaderDataType Type;
    public int Size;
    public int Offset;
    public bool Normalized;

    internal BufferElement(ShaderDataType type, string name, bool normalized = false)
    {
        Name = name;
        Type = type;
        Size = RenderHelpers.ShaderDataTypeSize(type);
        Offset = 0;
        Normalized = normalized;
    }

    internal int GetComponentCount()
    {
        switch (Type)
        {
            case ShaderDataType.Float: return 1;
            case ShaderDataType.Float2: return 2;
            case ShaderDataType.Float3: return 3;
            case ShaderDataType.Float4: return 4;
            case ShaderDataType.Mat3: return 3 * 3;
            case ShaderDataType.Mat4: return 4 * 4;
            case ShaderDataType.Int: return 1;
            case ShaderDataType.Int2: return 2;
            case ShaderDataType.Int3: return 3;
            case ShaderDataType.Int4: return 4;
            case ShaderDataType.Bool: return 1;
        }
        Log.Error("Unknown ShaderDataType!");
        return 0;
    }
}

