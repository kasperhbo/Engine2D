#region

using System.Reflection;
using Engine2D.Flags;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Engine2D.UI.ImGuiExtension;
using Engine2D.Utilities;
using Newtonsoft.Json;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

#endregion

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
public class Component
{
    [JsonIgnore] private bool _initialized;
    [JsonIgnore] internal Gameobject Parent;
    
    [JsonProperty]internal string Type => GetItemType();


    public virtual string GetItemType()
    {
        return this.GetType().FullName;
    }


    internal virtual void Init(Gameobject parent, Renderer? renderer)
    {
        if (_initialized) return;
        _initialized = true;
        Parent = parent;
    }

    internal virtual void Init(Gameobject parent)
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

    public virtual void ImGuiFields()
    {
        var fields = GetType().GetFields(
            BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public 
            | BindingFlags.Static | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.IgnoreCase 
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
                if (type == typeof(KDBColor))
                {
                    var val = (KDBColor)value;
                    Gui.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }
                if (type == typeof(Vector2))
                {
                    var val = (Vector2)value;
                    Gui.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }
                if (type == typeof(OpenTK.Mathematics.Vector2))
                {
                    var val = (OpenTK.Mathematics.Vector2)value;
                    Gui.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }
                
                if (type == typeof(Vector3))
                {
                    var val = (Vector3)value;
                    Gui.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }

                if (type == typeof(OpenTK.Mathematics.Vector3))
                {
                    var val = (OpenTK.Mathematics.Vector3)value;
                    Gui.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }

                if (type == typeof(Vector4))
                {
                    var val = (Vector4)value;
                    Gui.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }
                if (type == typeof(OpenTK.Mathematics.Vector4))
                {
                    var val = (OpenTK.Mathematics.Vector4)value;
                    Gui.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }

                if (type == typeof(float))
                {
                    var val = (float)value;
                    Gui.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }

                if (type == typeof(int))
                {
                    var val = (int)value;
                    Gui.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }

                if (type == typeof(bool))
                {
                    var val = (bool)value;
                    Gui.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }

                if (type == typeof(string))
                {
                    var val = (string)value;
                    Gui.DrawProperty(name, ref val);
                    field.SetValue(this, val);
                }
            }
        }
    }

    internal virtual float GetFieldSize()
    {
        //Calculate size automaticly based on fields
        var count = 0;
        
        var fields = GetType().GetFields(
            BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.IgnoreCase 
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
                count++;
            }
        }

        return 20 * count;
    }
}