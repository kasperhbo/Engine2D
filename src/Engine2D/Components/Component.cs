#region

using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Engine2D.Core;
using Engine2D.Flags;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Engine2D.UI.ImGuiExtension;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenTK.Windowing.Common;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

#endregion

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
public abstract class Component : ICloneable
{
    [JsonIgnore] private bool _initialized;
    [JsonIgnore] public Gameobject? Parent;
    
    [JsonProperty]internal string Type => GetItemType();

    public abstract void StartPlay();

    public virtual string GetItemType()
    {
        return this.GetType().FullName;
    }

    internal void Init(Gameobject parent)
    {
        if (_initialized) return;
        _initialized = true;
        Parent = parent;
        Init();
    }

    public abstract void Init();
    


    public virtual void Start()
    {
    }

    public abstract void Update(FrameEventArgs args);
    
    public virtual void EditorUpdate(double dt)
    {
    }

    public virtual void GameUpdate(double dt)
    {
    }

    public virtual void StopPlay()
    {
        
    }
    
    public virtual void Destroy()
    {
        // if(this.Parent != null)
            // Engine.Get().CurrentScene?.Renderer?.RemoveSprite(this.Parent);
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
            var ignore = false;
        
            // var attrs = (ShowUIAttribute[])field.GetCustomAttributes
            //     (typeof(ShowUIAttribute), false);
            
            // var jsonAttrs = (Attribute[])field.GetCustomAttributes
            //     (typeof(Attribute), false);
        
            
            // foreach (var attr in attrs)
            //     if (!attr.show)
            //         ignore = true;
        
            // foreach (var jsonPropertyAttribute in jsonAttrs)
            //     if (jsonPropertyAttribute is JsonIgnoreAttribute)
            //         ignore = true;
            
            
        
            if (!ignore)
            { 
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

            var ignore = false;
            
            var attrs = (ShowUIAttribute[])field.GetCustomAttributes
                (typeof(ShowUIAttribute), false);
            
            var jsonAttrs = (Attribute[])field.GetCustomAttributes
                (typeof(Attribute), false);

            
            foreach (var attr in attrs)
                if (!attr.show)
                    ignore = true;

            foreach (var jsonPropertyAttribute in jsonAttrs)
            {
                Console.WriteLine();
                if (jsonPropertyAttribute is JsonIgnoreAttribute)
                    ignore = true;
            }

            if (!ignore)
            {
                count++;
            }
        }

        return 20 * count;
    }
    
    public void CopyAll<T>(T source, T target) where T : class
    {
        var type = typeof(T);
        foreach (var sourceProperty in type.GetProperties())
        {
            var targetProperty = type.GetProperty(sourceProperty.Name);
            targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
        }
        foreach (var sourceField in type.GetFields())
        {
            var targetField = type.GetField(sourceField.Name);
            targetField.SetValue(target, sourceField.GetValue(source));
        }       
    }

    public virtual object Clone()
    {
        var clone = this.MemberwiseClone();
        return clone;
    }

    public virtual void FixedGameUpdate()
    {
    }
}