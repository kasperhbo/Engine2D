using OpenTK.Graphics.OpenGL4;
using static OpenTK.Graphics.OpenGL4.All;

namespace Engine2D.Rendering.Buffers;

public class FramebufferTextureSpec
{
	public sealed class FramebufferTextureFormat
	{
		public static readonly FramebufferTextureFormat NONE = new FramebufferTextureFormat( 
			InnerEnum.NONE, false,(int)None, None);
		
		public static readonly FramebufferTextureFormat RGBA8 = new FramebufferTextureFormat( 
			InnerEnum.RGBA8, false, (int)Rgba8, Rgba);
		
		public static readonly FramebufferTextureFormat RED_INTEGER = new FramebufferTextureFormat(
			InnerEnum.RED_INTEGER, false,  (int)R32i, RedInteger);
		 
		public static readonly FramebufferTextureFormat RED_UNSIGNED_INTEGER = new FramebufferTextureFormat( 
			InnerEnum.RED_UNSIGNED_INTEGER, false, (int)R32ui, RedInteger);
		
		public static readonly FramebufferTextureFormat DEPTH = new FramebufferTextureFormat( 
			InnerEnum.DEPTH, true, (int)Depth24Stencil8, DepthStencilAttachment);
		
		public static readonly FramebufferTextureFormat DEPTH24STENCIL8 = new FramebufferTextureFormat( 
			InnerEnum.DEPTH24STENCIL8, true, (int)Depth24Stencil8, DepthStencilAttachment);
		

		private static readonly List<FramebufferTextureFormat> valueList = new List<FramebufferTextureFormat>();

		static FramebufferTextureFormat()
		{
			valueList.Add(NONE);
			valueList.Add(RGBA8);
			valueList.Add(RED_INTEGER);
			valueList.Add(RED_UNSIGNED_INTEGER);
			valueList.Add(DEPTH);
			valueList.Add(DEPTH24STENCIL8);
		}

		public enum InnerEnum
		{
			NONE,
			RGBA8,
			RED_INTEGER,
			RED_UNSIGNED_INTEGER,
			DEPTH,
			DEPTH24STENCIL8
		}

		public readonly InnerEnum innerEnumValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		public bool isDepth;
		public int internalFormat;
		public All format;
		
		internal FramebufferTextureFormat(InnerEnum innerEnum, bool isDepth, int internalFormat, All format)
		{
			this.isDepth = isDepth;
			this.internalFormat = internalFormat;
			this.format = format;

			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public static FramebufferTextureFormat[] values()
		{
			return valueList.ToArray();
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public static FramebufferTextureFormat valueOf(InnerEnum name)
		{
			foreach (FramebufferTextureFormat enumInstance in FramebufferTextureFormat.valueList)
			{
				if (enumInstance.innerEnumValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name.ToString());
		}
	}
	public sealed class TextureResizeFilterType
	{
		public static readonly TextureResizeFilterType LINEAR = new TextureResizeFilterType(                 
			InnerEnum.LINEAR, true,    Linear);
		
		public static readonly TextureResizeFilterType NEAREST = new TextureResizeFilterType(                
			InnerEnum.NEAREST, true, Nearest);
		
		public static readonly TextureResizeFilterType NEAREST_MIPMAP_NEAREST = new TextureResizeFilterType( 
			InnerEnum.NEAREST_MIPMAP_NEAREST, false, NearestMipmapNearest);
		
		public static readonly TextureResizeFilterType NEAREST_MIPMAP_LINEAR = new TextureResizeFilterType(  
			InnerEnum.NEAREST_MIPMAP_LINEAR, false, NearestMipmapLinear);
		
		public static readonly TextureResizeFilterType LINEAR_MIPMAP_NEAREST = new TextureResizeFilterType(  
			InnerEnum.LINEAR_MIPMAP_NEAREST, false, LinearMipmapNearest);
		
		public static readonly TextureResizeFilterType LINEAR_MIPMAP_LINEAR = new TextureResizeFilterType(   
			InnerEnum.LINEAR_MIPMAP_LINEAR, false, LinearMipmapLinear);
		

		private static readonly List<TextureResizeFilterType> valueList = new List<TextureResizeFilterType>();

		static TextureResizeFilterType()
		{
			valueList.Add(LINEAR);
			valueList.Add(NEAREST);
			valueList.Add(NEAREST_MIPMAP_NEAREST);
			valueList.Add(NEAREST_MIPMAP_LINEAR);
			valueList.Add(LINEAR_MIPMAP_NEAREST);
			valueList.Add(LINEAR_MIPMAP_LINEAR);
		}

		public enum InnerEnum
		{
			LINEAR,
			NEAREST,
			NEAREST_MIPMAP_NEAREST,
			NEAREST_MIPMAP_LINEAR,
			LINEAR_MIPMAP_NEAREST,
			LINEAR_MIPMAP_LINEAR
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		public readonly bool appliesToMagFilter;
		public readonly All glType;

		/// <summary>
		/// A basic property constructor
		/// </summary>
		/// <param name="appliesToMagFilter"> boolean: is this type applicable for magnification filters </param>
		/// <param name="glType"> int: the type that OpenGL wants. </param>
		internal TextureResizeFilterType(InnerEnum innerEnum, bool appliesToMagFilter, All glType)
		{
			this.appliesToMagFilter = appliesToMagFilter;
			this.glType = glType;

			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public static TextureResizeFilterType[] values()
		{
			return valueList.ToArray();
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static TextureResizeFilterType valueOf(InnerEnum type)
		{
			foreach (TextureResizeFilterType enumInstance in valueList)
			{
				if (enumInstance.innerEnumValue == type)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(type.ToString());
		}
	}
	public sealed class TextureWrapFilterType
	{
		public static readonly TextureWrapFilterType CLAMP_TO_EDGE = new TextureWrapFilterType(         
			Type.CLAMP_TO_EDGE,   ClampToEdge);
		public static readonly TextureWrapFilterType CLAMP_TO_BORDER = new TextureWrapFilterType(   	
			Type.CLAMP_TO_BORDER, ClampToBorder);
		public static readonly TextureWrapFilterType REPEAT = new TextureWrapFilterType(	        	
			Type.REPEAT,          Repeat);
		public static readonly TextureWrapFilterType MIRRORED_REPEAT = new TextureWrapFilterType(		
			Type.MIRRORED_REPEAT, MirroredRepeat);

		private static readonly List<TextureWrapFilterType> valueList = new List<TextureWrapFilterType>();

		static TextureWrapFilterType()
		{
			valueList.Add(CLAMP_TO_EDGE);
			valueList.Add(CLAMP_TO_BORDER);
			valueList.Add(REPEAT);
			valueList.Add(MIRRORED_REPEAT);
		}

		public enum Type
		{
			CLAMP_TO_EDGE,
			CLAMP_TO_BORDER,
			REPEAT,
			MIRRORED_REPEAT
		}

		private readonly Type innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		public readonly All GLType;

		internal TextureWrapFilterType(Type innerEnum, All glType)
		{
			this.GLType = glType;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public static TextureWrapFilterType[] Values()
		{
			return valueList.ToArray();
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static TextureWrapFilterType ValueOf(Type name)
		{
			foreach (TextureWrapFilterType enumInstance in TextureWrapFilterType.valueList)
			{
				if (enumInstance.innerEnumValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name.ToString());
		}
	}

	public readonly FramebufferTextureFormat format;
	public readonly TextureResizeFilterType minificationFilter;
	public readonly TextureResizeFilterType magnificationFilter;
	public readonly TextureWrapFilterType rFilter;
	public readonly TextureWrapFilterType sFilter;
	public readonly TextureWrapFilterType tFilter;

	public FramebufferTextureSpec() : this(FramebufferTextureFormat.NONE)
	{
	}
	public FramebufferTextureSpec(FramebufferTextureFormat format) : this(format, TextureResizeFilterType.LINEAR, TextureResizeFilterType.LINEAR)
	{
	}
	public FramebufferTextureSpec(FramebufferTextureFormat format, TextureResizeFilterType minificationFilter, TextureResizeFilterType magnificationFilter) : this(format, minificationFilter, magnificationFilter, TextureWrapFilterType.CLAMP_TO_EDGE)
	{
	}
	public FramebufferTextureSpec(FramebufferTextureFormat format, TextureResizeFilterType minificationFilter, TextureResizeFilterType magnificationFilter, TextureWrapFilterType wrapFilters) : this(format, minificationFilter, magnificationFilter, wrapFilters, wrapFilters, wrapFilters)
	{
	}
	public FramebufferTextureSpec(FramebufferTextureFormat format, TextureResizeFilterType minificationFilter, TextureResizeFilterType magnificationFilter, TextureWrapFilterType rFilter, TextureWrapFilterType sFilter, TextureWrapFilterType tFilter)
	{
		this.format = format;
		this.minificationFilter = minificationFilter;
		this.magnificationFilter = changeForMagnification(magnificationFilter);
		this.rFilter = rFilter;
		this.sFilter = sFilter;
		this.tFilter = tFilter;
	}
	private static TextureResizeFilterType changeForMagnification(TextureResizeFilterType t)
	{
		if (t.appliesToMagFilter)
		{
			return t;
		}
		else
		{
			return TextureResizeFilterType.LINEAR;
		}
	}
}