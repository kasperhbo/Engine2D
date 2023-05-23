using System.Numerics;
using System.Reflection;
using Engine2D.Flags;
using Engine2D.GameObjects;
using Engine2D.UI;
using ImGuiNET;
using Newtonsoft.Json;

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
public abstract class Component
{
    [JsonIgnore] private bool _initialized;
    [JsonIgnore] public GameObject Parent;

    protected float sizeYGUI;
    public string Type => GetItemType();


    public abstract string GetItemType();


    public virtual void Init(GameObject parent)
    {
        if (_initialized) return;
        _initialized = true;
        Parent = parent;
    }

    public virtual void Start()
    {
    }

    public virtual void EditorUpdate(double dt)
    {
    }

    public virtual void GameUpdate(double dt)
    {
    }

    public virtual void Destroy()
    {
    }

    public virtual Vector2 WindowSize()
    {
        return new Vector2(0, sizeYGUI);
    }


    public virtual float ImGuiFields()
    {
        sizeYGUI = 3;
        var y = ImGui.GetFontSize() + 14;
        var fields = GetType().GetFields(
            BindingFlags.DeclaredOnly
            | BindingFlags.Public
            | BindingFlags.NonPublic
            | BindingFlags.Instance
        );
        foreach (var field in fields)
        {
            var type = field.FieldType;
            var value = field.GetValue(this);
            var name = field.Name;

            var attrs = (ShowUIAttribute[])field.GetCustomAttributes
                (typeof(ShowUIAttribute), false);

            var ignore = false;
            foreach (var attr in attrs)
                if (!attr.show)
                    ignore = true;


            if (!ignore)
            {
                if (type == typeof(SpriteColor))
                {
                    var val = (SpriteColor)value;

                    sizeYGUI += y;

                    OpenTKUIHelper.DrawProperty(name, ref val);

                    field.SetValue(this, val);
                }
                
                if (type == typeof(LightColor))
                {
                    var val = (LightColor)value;

                    sizeYGUI += y;

                    OpenTKUIHelper.DrawProperty(name, ref val);

                    field.SetValue(this, val);
                }

                if (type == typeof(int))
                {
                    var val = (int)value;
                    sizeYGUI += y;
                    OpenTKUIHelper.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }

                if (type == typeof(float))
                {
                    var val = (float)value;
                    sizeYGUI += y;
                    OpenTKUIHelper.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }

                if (type == typeof(bool))
                {
                    var val = (bool)value;
                    sizeYGUI += y;
                    OpenTKUIHelper.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }

                if (type == typeof(Vector2))
                {
                    var val = (Vector2)value;
                    sizeYGUI += y;
                    OpenTKUIHelper.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }

                if (type == typeof(Vector3))
                {
                    var val = (Vector3)value;
                    sizeYGUI += y;
                    OpenTKUIHelper.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }

                if (type == typeof(Vector4))
                {
                    var val = (Vector4)value;
                    sizeYGUI += y;
                    OpenTKUIHelper.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }
            }
        }

        return sizeYGUI;
    }
}