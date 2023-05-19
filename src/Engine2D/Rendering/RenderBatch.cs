using OpenTK.Graphics.OpenGL;

namespace Engine2D.Rendering;

public abstract class RenderBatch : IComparable<RenderBatch>
{
    protected int m_vertexCount;
    protected int m_vertexSize;

    protected List<Texture> m_textures = new List<Texture>();

    protected float[] m_data;
    protected int m_spriteCount = 0;

    protected readonly int m_maxBatchSize;
    private   readonly ShaderDatatype[] m_shaderAttributes;
    public    readonly int ZIndex;

    private bool m_shouldRebufferData = true;
    
    private int m_textureIndex;
    
    private int m_vao;
    private int m_vbo;
    private int m_ebo;

    public bool HasRoom = true;
    public bool HasTextureRoom => this.m_textures.Count < 8;

    public bool HasTexture(Texture tex)
    {
        return this.m_textures.Contains(tex);
    } 

    //TODO:RESET THIS
    public int VertexCount => m_spriteCount *  _primitive.ElementCount;
    private Primitive _primitive;
	
    public RenderBatch(int maxBatchSize, int zIndex, PrimitiveTypes primitiveType,  ShaderDatatype[] attributes) {
        this.m_maxBatchSize = maxBatchSize;
        this.ZIndex = zIndex;
        this.m_shaderAttributes = attributes;
        
        _primitive = Primitive.valueOf(primitiveType);
        
        m_spriteCount = 0;
        HasRoom = true;
        m_textureIndex = 0;
        m_textures = new List<Texture>();
        foreach (ShaderDatatype type in m_shaderAttributes) {
            m_vertexCount += type.count;
            m_vertexSize  += type.size;
        }
        
        m_data = new float
        [  
	        maxBatchSize * _primitive.VertexCount * m_vertexCount
        ];
    }

    protected abstract void LoadVertexProperties(int index, int offset);
    
    public void Start()
    {
        m_vao= GL.GenVertexArray();
        GL.BindVertexArray(m_vao);
        
        m_vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, m_data.Length * sizeof(float), m_data,BufferUsageHint.DynamicDraw);
        
        m_ebo =GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_ebo);
        var elements = GenerateIndices();
        GL.BufferData(BufferTarget.ElementArrayBuffer, elements.Length * sizeof(int), elements,
            BufferUsageHint.StaticDraw);
        
        int currentOffset = 0;
        for (int i = 0; i < m_shaderAttributes.Length; i++) {
            
	        ShaderDatatype attrib = m_shaderAttributes[i];
            
            GL.VertexAttribPointer(i, attrib.count, attrib.openglType, 
                false, m_vertexSize, currentOffset);
            GL.EnableVertexAttribArray(i);
            currentOffset += attrib.size;
        }
    }
    
    private int GetOffset(int index) {
	    return index * _primitive.VertexCount * m_vertexCount;
    }
    
    protected void Load(int index) {
        m_shouldRebufferData = true;
        m_spriteCount++;
        int offset = GetOffset(index);
        LoadVertexProperties(index, offset);
    }
    
    protected int AddTexture(Texture texture) {
        int texIndex;
        if (m_textures.Contains(texture)) {
            texIndex = m_textures.IndexOf(texture) + 1;
        } else {
            m_textures.Add(texture);
            texIndex = ++m_textureIndex;
        }
        return texIndex;
    }
    
    public virtual void UpdateBuffer()
    {
        if (!m_shouldRebufferData) return;
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbo);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, m_data.Length * sizeof(float), m_data);
        m_shouldRebufferData = false;
    }
    
    public void Bind() {
        GL.BindVertexArray(m_vao);
        
        for (int i = 0; i < m_textures.Count; i++)
        {
	        m_textures[i].Use(i + 1);
        }
    }
    
    public void Unbind() {
        foreach (Texture texture in m_textures)
        {
	        if (texture == null)
		        return;
	        texture.unbind();
        }
        // Unbind everything
        GL.BindVertexArray(0);
    }
    
    public void Delete() {
        GL.DeleteBuffer(m_vbo);
        GL.DeleteBuffer(m_ebo);
        GL.DeleteVertexArray(m_vao);
    }

    private int[] GenerateIndices()
    {
	    var elements = new int[_primitive.ElementCount * m_maxBatchSize];

	    for (int i = 0; i < m_maxBatchSize; i++)
	    {
		    _primitive.elementCreation.Invoke(elements, i);
	    }

	    return elements;
    }
    

    
    public int CompareTo(RenderBatch? other)
    {
        if (other != null && this.ZIndex < other.ZIndex)
            return -1;
            
        if (other != null && this.ZIndex == other.ZIndex)
            return 0;
        
        return 1;
    }
}

public enum ShaderTypes
{
	INT,
	INT2,
	INT3,
	INT4,
	FLOAT,
	FLOAT2,
	FLOAT3,
	FLOAT4,
	MAT3,
	MAT4
}

public class ShaderDatatype
{
    public static readonly ShaderDatatype INT =   new ShaderDatatype(ShaderTypes.INT, 1,  1 * sizeof(int),        VertexAttribPointerType.Int);
	public static readonly ShaderDatatype INT2 =  new ShaderDatatype(ShaderTypes.INT2, 2, 2 * sizeof(int),        VertexAttribPointerType.Int);
	public static readonly ShaderDatatype INT3 =  new ShaderDatatype(ShaderTypes.INT3, 3, 3 * sizeof(int),        VertexAttribPointerType.Int);
	public static readonly ShaderDatatype INT4 =  new ShaderDatatype(ShaderTypes.INT4, 4, 4 * sizeof(int),        VertexAttribPointerType.Int);
	public static readonly ShaderDatatype FLOAT = new ShaderDatatype(ShaderTypes.FLOAT, 1, (1*1) * sizeof(float), VertexAttribPointerType.Float);
	public static readonly ShaderDatatype FLOAT2 =new ShaderDatatype(ShaderTypes.FLOAT2, 2,(1*2) * sizeof(float), VertexAttribPointerType.Float);
	public static readonly ShaderDatatype FLOAT3 =new ShaderDatatype(ShaderTypes.FLOAT3, 3,(1*3) * sizeof(float), VertexAttribPointerType.Float);
	public static readonly ShaderDatatype FLOAT4 =new ShaderDatatype(ShaderTypes.FLOAT4, 4,(1*4) * sizeof(float), VertexAttribPointerType.Float);
	public static readonly ShaderDatatype MAT3 =  new ShaderDatatype(ShaderTypes.MAT3, 9,  3 * 3 * sizeof(float), VertexAttribPointerType.Float);
	public static readonly ShaderDatatype MAT4 =  new ShaderDatatype(ShaderTypes.MAT4, 16, 4 * 4 * sizeof(float), VertexAttribPointerType.Float);

	private static readonly List<ShaderDatatype> valueList = new List<ShaderDatatype>();

	static ShaderDatatype()
	{
		valueList.Add(INT);
		valueList.Add(INT2);
		valueList.Add(INT3);
		valueList.Add(INT4);
		valueList.Add(FLOAT);
		valueList.Add(FLOAT2);
		valueList.Add(FLOAT3);
		valueList.Add(FLOAT4);
		valueList.Add(MAT3);
		valueList.Add(MAT4);
	}



	public readonly ShaderTypes shaderType;
	private readonly string nameValue;
	private readonly int ordinalValue;
	private static int nextOrdinal = 0;

	/// <summary>
	/// Number of FLOATS or INTS </summary>
	public readonly int count;
	/// <summary>
	/// Number of bytes </summary>
	public readonly int size;
	/// <summary>
	/// OpenGL expected type </summary>
	public readonly VertexAttribPointerType openglType;

	internal ShaderDatatype(ShaderTypes shaderType, int count, int bytes, VertexAttribPointerType openglType)
	{
		this.count = count;
		this.size = bytes;
		this.openglType = openglType;
		this.shaderType = shaderType;

		ordinalValue = nextOrdinal++;
	}

	public static ShaderDatatype[] values()
	{
		return valueList.ToArray();
	}

	public override string ToString()
	{
		return nameValue;
	}

	public static ShaderDatatype ValueOf(ShaderTypes type)
	{
		foreach (ShaderDatatype enumInstance in ShaderDatatype.valueList)
		{
			if (enumInstance.shaderType == type)
			{
				return enumInstance;
			}
		}
		throw new System.ArgumentException(type.ToString());
	}

}