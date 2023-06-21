#region

using System.Numerics;
using System.Runtime.InteropServices;
using Engine2D.Components;
using Engine2D.Components.Sprites;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.Flags;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.Rendering;
using ImGuiNET;
using Newtonsoft.Json;

#endregion

namespace Engine2D.GameObjects;

internal class KDBColor
{
    [JsonRequired] internal float a;
    [JsonRequired] internal float b;
    [JsonRequired] internal float g;
    [JsonRequired] internal float r;

    internal KDBColor()
    {
        r = 255;
        g = 255;
        b = 255;
        a = 255;
    }

    internal KDBColor(KDBColor other)
    {
        r = other.r;
        g = other.g;
        b = other.b;
        a = other.a;
    }

    internal KDBColor(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    [JsonIgnore] internal float RNormalized => r / 255;
    [JsonIgnore] internal float GNormalized => g / 255;
    [JsonIgnore] internal float BNormalized => b / 255;
    [JsonIgnore] internal float ANormalized => a / 255;


    internal bool Equals(KDBColor? obj)
    {
        if (obj == null)
        {
            Log.Error("Obj to check against not set  'KDB COLOR' ");
            return false;
        }

        return r == obj.r && g == obj.g && b == obj.b && a == obj.a;
    }

    internal static void Copy(KDBColor from, KDBColor to)
    {
        from.r = to.r;
        from.g = to.g;
        from.b = to.b;
        from.a = to.a;
    }
}

[JsonConverter(typeof(ComponentSerializer))]
internal class SpriteRenderer : Component
{
    [ShowUI(show = false)][JsonIgnore] private readonly Vector2[] _defaultTextureCoords =
    {
        new(1.0f, 1.0f),
        new(1.0f, 0.0f),
        new(0.0f, 0.0f),
        new(0.0f, 1.0f)
    };

    [ShowUI(show = false)][JsonIgnore] private readonly KDBColor _lastColor = new();

    // 0.5f,   0.5f, 0.0f,    1.0f, 1.0f,   // top right
    // 0.5f,  -0.5f, 0.0f,    1.0f, 0.0f,   // bottom right
    // -0.5f, -0.5f, 0.0f,    0.0f, 0.0f,   // bottom left
    // -0.5f,  0.5f, 0.0f,    0.0f, 1.0f    // top left 
    [ShowUI(show = false)][JsonIgnore] private readonly Transform _lastTransform = new();
    [ShowUI(show = false)][JsonProperty] private string _spriteSaveFile = "";
    [JsonProperty] internal KDBColor Color = new();
    [JsonProperty] internal int ZIndex;
    

    [JsonConstructor]
    internal SpriteRenderer()
    {
        IsDirty = true;
    }

    [JsonIgnore] internal Sprite? Sprite { get; private set; }
    [JsonIgnore] internal bool IsDirty { get; set; }

    internal Vector2[] TextureCoords
    {
        get
        {
            if (Sprite == null) return _defaultTextureCoords;

            return Sprite.TextureCoords;
        }
    }

    internal override void Init(Gameobject parent, Renderer? renderer)
    {
        ResourceManager.SpriteRenderers.Add(this);
        base.Init(parent, renderer);

        if (_spriteSaveFile == "")
            Engine.Get().CurrentScene.Renderer.AddSpriteRenderer(this);
        else
            SetSprite(_spriteSaveFile);
    }

    public override void EditorUpdate(double dt)
    {
        base.EditorUpdate(dt);

        if (!_lastColor.Equals(Color) || !_lastTransform.Equals(Parent.GetComponent<Transform>()))
        {
            IsDirty = true;
            KDBColor.Copy(_lastColor, Color);
            Transform.Copy(_lastTransform, Parent.GetComponent<Transform>());
        }
    }

    internal void RefreshSprite()
    {
        SetSprite(_spriteSaveFile);
    }

    internal void SetSprite(string newSprite)
    {
        Engine.Get().CurrentScene?.Renderer?.RemoveSprite(this);

        IsDirty = true;
        var oldSprite = _spriteSaveFile;
        _spriteSaveFile = newSprite;

        Sprite = ResourceManager.GetItem<Sprite>(_spriteSaveFile);
        Engine.Get().CurrentScene?.Renderer?.AddSpriteRenderer(this);
    }

    private void SetSprite(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine("Folder changed");
        SetSprite(e.FullPath);
    }

    public override void ImGuiFields()
    {
        base.ImGuiFields();
       
        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGui.AcceptDragDropPayload("sprite_drop");
            if (payload.IsValidPayload())
            {
                var filename = (string)GCHandle.FromIntPtr(payload.Data).Target;
                SetSprite(filename);
            }

            ImGui.EndDragDropTarget();
        }
        //
        //
        // if (Sprite != null)
        // {
        //     ImGui.InputFloat2("0", ref Sprite.TextureCoords[0]);
        //     ImGui.InputFloat2("1", ref Sprite.TextureCoords[1]);
        //     ImGui.InputFloat2("2", ref Sprite.TextureCoords[2]);
        //     ImGui.InputFloat2("3", ref Sprite.TextureCoords[3]);
        // }
    }
}