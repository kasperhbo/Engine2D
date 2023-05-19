using System.Transactions;
using Engine2D.GameObjects;
using Engine2D.Rendering.Buffers;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.Rendering;

public abstract class Renderer<T> where T : RenderBatch
{
    protected int[] m_textureSlots = {0, 1, 2, 3, 4, 5, 6, 7};

    protected List<T> _batches = new List<T>();
    private Shader _shader;
    
    public Framebuffer Framebuffer;
    
    protected abstract Shader CreateShader();
    protected abstract Framebuffer CreateFramebuffer();
    protected abstract void Prepare();
    protected abstract void UploadUniforms(Shader shader);
    
    public virtual void Add(Gameobject gameobject){}

    public void Init()
    {
        _shader = CreateShader();
        Framebuffer = CreateFramebuffer();
    }

    public int FetchColorAttachment(int index)
    {
        return Framebuffer.FetchColorAttachment(index);
    }

    public void Render()
    {
        Framebuffer.Bind();
        Prepare();
        GL.Clear(ClearBufferMask.ColorBufferBit);
        _shader.use();
        UploadUniforms(_shader);
        foreach(T batch in _batches )
        {
            batch.UpdateBuffer();
            batch.Bind();
            GL.DrawElements(PrimitiveType.Triangles, batch.VertexCount, DrawElementsType.UnsignedInt, 0);
            batch.Unbind();
        }
        _shader.detach();
        Framebuffer.Unbind();
    }

    public void Clean()
    {
        foreach (T batch in _batches)
        {
            batch.Delete();
        }
    }
}
