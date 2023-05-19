using Engine2D.GameObjects;

namespace Engine2D.Rendering;

public enum PrimitiveTypes
{
    QUAD
}

public class Primitive
{
    private static readonly List<Primitive> valueList = new List<Primitive>();

    //SAMPLE OF CREATING THE QUAD ELEMENETS
    /*************************************************
    private int[] GenerateIndices()
    {
        Primitive primitive = Primitive.valueOf("QUAD");
        // 6 indices per quad (3 per triangle)
        var elements = new int[6 * c_MaxBatchSize];

        for (var i = 0; i < c_MaxBatchSize; i++) primitive.elementCreation.Invoke(elements, i);

        return elements;
    }
    **************************************************/
    
    static Primitive()
    {
        valueList.Add(new Primitive(PrimitiveTypes.QUAD, 4,6, (elements, i) =>
            {
                var offsetArrayIndex = 6 *i;
                int offset = 4 * i;

                // 3, 2, 0,
                // 0, 2, 1,
                
                // 7, 6, 4,
                // 4, 6, 5

                // Triangle 1
                elements[offsetArrayIndex] = offset + 3;
                elements[offsetArrayIndex+1] = offset + 2;
                elements[offsetArrayIndex+2] = offset + 0;

                // Triangle 2
                elements[offsetArrayIndex+3]  = offset + 0;
                elements[offsetArrayIndex+4]  = offset + 2;
                elements[offsetArrayIndex+5]  = offset + 1;
            }));
    }



    public readonly PrimitiveTypes innerEnumValue;
    private readonly int ordinalValue;
    private static int nextOrdinal = 0;

    private Primitive(PrimitiveTypes innerEnum)
    {
        ordinalValue = nextOrdinal++;
        innerEnumValue = innerEnum;
    }

    /// <summary>
    /// Puts index data in the provided int buffer </summary>
    public readonly System.Action<int[], int> elementCreation;

    public int VertexCount { get; private set; }
    public int ElementCount { get; private set; }
    internal Primitive(PrimitiveTypes innerEnum, int vertexCount, int elementCount, System.Action<int[], int> elementCreation)
    {
        this.elementCreation = elementCreation;

        ordinalValue = nextOrdinal++;
        innerEnumValue = innerEnum;

        VertexCount = vertexCount;
        ElementCount = elementCount;
    }

    public static Primitive valueOf(PrimitiveTypes name)
    {
        foreach (Primitive enumInstance in Primitive.valueList)
        {
            if (enumInstance.innerEnumValue == name)
            {
                return enumInstance;
            }
        }
        throw new System.ArgumentException(name.ToString());
    }
}
