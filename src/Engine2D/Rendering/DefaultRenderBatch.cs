using System.Numerics;
using Engine2D.Components;
using Engine2D.GameObjects;

namespace Engine2D.Rendering;

public class DefaultRenderBatch : RenderBatch
{
    private SpriteRenderer[] m_sprites;
    private int m_numberOfSprites;
    
    public DefaultRenderBatch(int maxBatchSize, int zIndex) : 
        base(maxBatchSize, zIndex,PrimitiveTypes.QUAD ,new []
        {
            ShaderDatatype.FLOAT2, ShaderDatatype.FLOAT4,
            ShaderDatatype.FLOAT2, ShaderDatatype.FLOAT    
        })
    {
        
        this.m_sprites = new SpriteRenderer[maxBatchSize];
        this.m_numberOfSprites = 0;
    }

    protected override void LoadVertexProperties(int index, int offset)
    {
        SpriteRenderer sprite = this.m_sprites[index];

        Vector4 color = sprite.Color.Color;
        Vector2[] textureCoordinates = sprite.TextureCoords;
        // int textureID = AddTexture(sprite.texture);
        int textureID = AddTexture(sprite.texture);


        // Add vertex with the appropriate properties
        float xAdd = 1.0f;
        float yAdd = 1.0f;
        for (int i = 0; i < 4; i++) {
            switch (i) {
                case 1:
                    yAdd = 0.0f;
                    break;
                case 2:
                    xAdd = 0.0f;
                    break;
                case 3:
                    yAdd = 1.0f;
                    break;
            }
            
            // Load position
            // Transform spr = sprite.Parent.transform;
            
            // Load position
            Transform spr = sprite.Parent.transform;
            m_data[offset] = spr.position.X + (xAdd * spr.size.X);
            m_data[offset + 1] = spr.position.Y + (yAdd * spr.size.Y);

            // Load color
            m_data[offset + 2] = color.X; // Red
            m_data[offset + 3] = color.Y; // Green
            m_data[offset + 4] = color.Z; // Blue
            m_data[offset + 5] = color.W; // Alpha

            // Load texture coordinates
            m_data[offset + 6] = textureCoordinates[i].X;
            m_data[offset + 7] = textureCoordinates[i].Y;

            // Load texture ID
            m_data[offset + 8] = textureID;

            offset += m_vertexCount;
        }
    }

    public override void UpdateBuffer()
    {
        for (int i = 0; i < m_numberOfSprites; i ++) {
            SpriteRenderer spr = m_sprites[i];
            if (spr.IsDirty) {
                Load(i);
                spr.IsDirty = false;
            }
        }
        base.UpdateBuffer();
    }
    
    public bool AddSprite(SpriteRenderer sprite) {
        // If the batch still has room, and is at the same z index as the sprite, then add it to the batch
        if (HasRoom && ZIndex == sprite.ZIndex) {
            Texture tex = sprite.texture;
            if (tex == null || (HasTexture(tex) || HasTextureRoom)) {
                // Get the index and add the renderObject
                int index = this.m_numberOfSprites;
                this.m_sprites[index] = sprite;
                this.m_numberOfSprites++;

                // Add properties to local vertices array
                Load(index);

                if (this.m_numberOfSprites >= this.m_maxBatchSize) {
                    this.HasRoom = false;
                }
                return true;
            }
        }
        return false;
    }
}