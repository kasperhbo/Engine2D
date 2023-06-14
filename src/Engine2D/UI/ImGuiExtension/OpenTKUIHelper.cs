﻿using Engine2D.GameObjects;
using ImGuiNET;
using ImTool;
using OpenTK.Mathematics;
using Vector2 = System.Numerics.Vector2;

namespace Engine2D.UI.ImGuiExtension;

internal static class OpenTkuiHelper
{
    public static void DrawComponentWindow(string id, string title, Action tablesToDraw, float size = 100)
    {
        ImGui.PushID(id);
        
        if (ImGui.CollapsingHeader(title, ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Framed))
        {
            ImGui.BeginChild("##child", new Vector2(0, size + 10), true);
            // ImGui.BeginChildFrame(1, new Vector2(0,size));
            ImGui.PushStyleColor(ImGuiCol.TableBorderStrong, new System.Numerics.Vector4(.19f, .19f, .19f, 1)); //For visibility
            
            if (ImGui.BeginTable("##transform_t", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit))
            {
                tablesToDraw.Invoke();
                ImGui.EndTable();
            }

            ImGui.EndChild();
        
            
            ImGui.PopStyleColor();
            // ImGui.EndChild();
        }
        ImGui.PopID();
    }
    
    public static bool Matrix4(ref Matrix4 mat, string name)
    {
        ImGui.PushID(name);
        int num1 = 0 | (Widgets.FloatLabel(ref mat.Row0.X, "M11") ? 1 : 0);
        ImGui.SameLine();
        int num2 = Widgets.FloatLabel(ref mat.Row0.Y, "M12", 4278235392U) ? 1 : 0;
        int num3 = num1 | num2;
        ImGui.SameLine();
        int num4 = Widgets.FloatLabel(ref mat.Row0.Z, "M13", 4289789952U) ? 1 : 0;
        int num5 = num3 | num4;
        ImGui.SameLine();
        int num6 = Widgets.FloatLabel(ref mat.Row0.W, "M14", 4287299723U) ? 1 : 0;
        int num7 = num5 | num6 | (Widgets.FloatLabel(ref mat.Row1.X, "M21") ? 1 : 0);
        ImGui.SameLine();
        int num8 = Widgets.FloatLabel(ref mat.Row1.Y, "M22", 4278235392U) ? 1 : 0;
        int num9 = num7 | num8;
        ImGui.SameLine();
        int num10 = Widgets.FloatLabel(ref mat.Row1.Z, "M23", 4289789952U) ? 1 : 0;
        int num11 = num9 | num10;
        ImGui.SameLine();
        int num12 = Widgets.FloatLabel(ref mat.Row1.W, "M24", 4287299723U) ? 1 : 0;
        int num13 = num11 | num12 | (Widgets.FloatLabel(ref mat.Row2.X, "M31") ? 1 : 0);
        ImGui.SameLine();
        int num14 = Widgets.FloatLabel(ref mat.Row2.Y, "M32", 4278235392U) ? 1 : 0;
        int num15 = num13 | num14;
        ImGui.SameLine();
        int num16 = Widgets.FloatLabel(ref mat.Row2.Z, "M33", 4289789952U) ? 1 : 0;
        int num17 = num15 | num16;
        ImGui.SameLine();
        int num18 = Widgets.FloatLabel(ref mat.Row2.W, "M34", 4287299723U) ? 1 : 0;
        int num19 = num17 | num18 | (Widgets.FloatLabel(ref mat.Row3.X, "M41") ? 1 : 0);
        ImGui.SameLine();
        int num20 = Widgets.FloatLabel(ref mat.Row3.Y, "M42", 4278235392U) ? 1 : 0;
        int num21 = num19 | num20;
        ImGui.SameLine();
        int num22 = Widgets.FloatLabel(ref mat.Row3.Z, "M43", 4289789952U) ? 1 : 0;
        int num23 = num21 | num22;
        ImGui.SameLine();
        int num24 = Widgets.FloatLabel(ref mat.Row3.W, "M44", 4287299723U) ? 1 : 0;
        int num25 = num23 | num24;
        ImGui.PopID();
        return num25 != 0;
    }

    public static void PrepareProperty(string name)
    {
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
    }
    
    public static bool DrawProperty(string name, ref bool property)
    {
        PrepareProperty(name);
        return ImGui.Checkbox("##"+name, ref property);
    }

    public static bool DrawProperty(string name, ref float property, bool label = true)
    {
        PrepareProperty(name);
        return KDBFloat(ref property, name, color: 4278190257, label: label);
    }
    
    public static bool DrawProperty(string name, ref int property)
    {
        bool changed = false;
        PrepareProperty(name);
        float tempProp = (float)property;
        if (Widgets.FloatLabel(ref tempProp, name)) changed = true;
        property = (int)tempProp;
        return changed;
    }
    
    public static bool DrawProperty(string name, ref System.Numerics.Vector2 property, float dragSped = .1f)
    {
        PrepareProperty(name);
        return Vector2(ref property, name, dragSpeed:dragSped);
    }
    
    public static bool DrawProperty(string name, ref OpenTK.Mathematics.Vector2 property)
    {
        bool changed = false;
        PrepareProperty(name);
        System.Numerics.Vector2 tempProp = new(property.X, property.Y);
        if (Widgets.Vector2(ref tempProp, name)) changed = true;
        property = new(tempProp.X, tempProp.Y);
        return changed;
    }
    
    public static bool DrawProperty(string name, ref OpenTK.Mathematics.Vector2i property)
    {
        bool changed = false;
        PrepareProperty(name);
        System.Numerics.Vector2 tempProp = new(property.X, property.Y);
        if (Widgets.Vector2(ref tempProp, name)) changed = true;
        property = new((int)tempProp.X, (int)tempProp.Y);
        return changed;
    }

    public static bool DrawProperty(string name, ref System.Numerics.Vector3 property)
    {
        PrepareProperty(name);
        return Widgets.Vector3(ref property, name);
    }
    
    public static bool DrawProperty(string name, ref OpenTK.Mathematics.Vector3 property)
    {
        bool changed = false;
        PrepareProperty(name);
        System.Numerics.Vector3 tempProp = new(property.X, property.Y, property.Z);
        if (Widgets.Vector3(ref tempProp, name)) changed = true;
        return changed;
    }
    
    public static bool DrawProperty(string name, ref float Roll, ref float Pitch, ref float Yaw, float dragSpeed = 1)
    {
        bool changed = false;
        PrepareProperty(name);     
        
        ImGui.PushID(name);
        int num1 = 0 | (KDBFloat(ref Pitch, "Pitch", 4278190257, dragSpeed) ? 1 : 0);
        ImGui.SameLine();
        int num2 = KDBFloat(ref Yaw, "Yaw", 4278235392U, dragSpeed) ? 1 : 0;
        int num3 = num1 | num2;
        ImGui.SameLine();
        int num4 = KDBFloat(ref Roll, "Roll", 4289789952U, dragSpeed) ? 1 : 0;
        int num5 = num3 | num4;
        ImGui.PopID();
        return num5 != 0;
    }
    
    public static bool DrawProperty(string name, ref System.Numerics.Vector4 property)
    {
        PrepareProperty(name);
        return Widgets.Vector4(ref property, name);
    }
    
    public static bool DrawProperty(string name, ref OpenTK.Mathematics.Vector4 property)
    {
        bool changed = false;
        PrepareProperty(name);
        System.Numerics.Vector4 tempProp = new(property.X, property.Y, property.Z, property.W);
        if (Widgets.Vector4(ref tempProp, name)) changed = true;
        return changed;
    }
    
    public static bool DrawProperty(string name, ref System.Numerics.Quaternion property)
    {
        PrepareProperty(name);
        return Widgets.Quaternion(ref property, name);
    }

    
    public static bool DrawProperty(string name, ref KDBColor property)
    {
        PrepareProperty(name);
        ImGui.PushID(name);

        int num1 = 0 | (KDBFloat(ref property.r, "R", 4278190257, 1, 0, 255) ? 1 : 0);
        ImGui.SameLine();
        int num2 = KDBFloat(ref property.g, "G", 4278235392U, 1, 0, 255) ? 1 : 0;
        int num3 = num1 | num2;
        ImGui.SameLine();
        int num4 = KDBFloat(ref property.b, "B", 4289789952U, 1, 0, 255) ? 1 : 0;
        int num5 = num3 | num4;
        ImGui.SameLine();
        int num6 = KDBFloat(ref property.a, "A", 4287299723U, 1, 0, 255) ? 1 : 0;
        int num7 = num5 | num6;
        ImGui.PopID();

        System.Numerics.Vector4 tempColor = new System.Numerics.Vector4(property.RNormalized, property.GNormalized, property.BNormalized, property.ANormalized);
        
        ImGui.ColorEdit4(name, ref tempColor, ImGuiColorEditFlags.NoLabel | ImGuiColorEditFlags.NoInputs);
        
        property.r = tempColor.X*255;
        property.g = tempColor.Y*255;
        property.b = tempColor.Z*255;
        property.a = tempColor.W*255;
        
        return num7 != 0;
    }

    public static bool KDBFloat(ref float val, string name, uint color = 4278190257, float dragSpeed = -1, float min = -1, float max = -1, bool label = true)
    {
        if (label) 
            CreateFloatLabel(name, color);
        
        float valTemp = val;
        
        int num = 0;
        if(min != -1 && max != -1)
            num = ImGui.DragFloat("###" + name, ref valTemp, dragSpeed, min, max) ? 1 : 0;
        else
            num = ImGui.DragFloat("###" + name, ref valTemp, dragSpeed) ? 1 : 0;
        
        if (label) 
            ImGui.PopStyleVar(1);

        // var mouse = Engine.Get().MousePosition;
        // var size = Engine.Get().Size;
        //     
        // if (mouse.X > size.X)
        // {
        //     Engine.Get().MousePosition = new(0, mouse.Y);
        //         
        //     valTemp += size.X;
        // }
        // if (mouse.X <= 0.0001f)
        // {
        //     Engine.Get().MousePosition = new(size.X, mouse.Y);
        //     valTemp -= size.X;
        // }
        //
        val = valTemp;
        return num != 0;
    }
    
    private static void CreateFloatLabel(string name, uint color)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0.0f);
        System.Numerics.Vector2 vector2 = new System.Numerics.Vector2(ImGui.CalcTextSize(name).X + 7f, 2f);
        System.Numerics.Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
        System.Numerics.Vector2 p_max =
            cursorScreenPos + new System.Numerics.Vector2(vector2.X, ImGui.GetFrameHeight());
        ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
        windowDrawList.AddRectFilled(cursorScreenPos, p_max, color, 6f, ImDrawFlags.RoundCornersLeft);
        windowDrawList.AddText(cursorScreenPos + new System.Numerics.Vector2(3f, 4f), uint.MaxValue, name);
        ImGui.SetCursorPosX(ImGui.GetCursorPos().X + vector2.X);
        ImGui.PushItemWidth(80f);
    }
    
    private static bool Vector2(ref System.Numerics.Vector2 vec, string name, float dragSpeed = 1)
    {
        ImGui.PushID(name);
        int num1 = 0 | (KDBFloat(ref vec.X, "X", dragSpeed:dragSpeed) ? 1 : 0);
        ImGui.SameLine();
        int num2 = KDBFloat(ref vec.Y, "Y", 4278235392U, dragSpeed:dragSpeed) ? 1 : 0;
        int num3 = num1 | num2;
        ImGui.PopID();
        return num3 != 0;
    }
    
    public static ImRect RectExpanded(ImRect rect, float x, float y)
    {
        ImRect result = rect;
        result.Min.X -= x;
        result.Min.Y -= y;
        result.Max.X += x;
        result.Max.Y += y;
        return result;
    }
    
    public static System.Numerics.Vector4 RectExpanded(System.Numerics.Vector4 rect, float x, float y)
    {
        var result = rect;


        result.X -= x;
        result.Y -= y;

        result.Z += x;
        result.W += y;

        return result;
    }
    
    public static void DrawButtonImage(
        IntPtr imageNormal,
        IntPtr imageHovered,
        IntPtr imagePressed,
        System.Numerics.Vector4 rect
    )
    {
        if (ImGui.IsItemActive())
            ImGui.GetWindowDrawList().AddImage(
                imagePressed,
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.Z, rect.W),
                new Vector2(0, 0),
                new Vector2(1, 1)
            );
        else if (ImGui.IsItemHovered())
            ImGui.GetWindowDrawList().AddImage(
                imageHovered,
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.Z, rect.W),
                new Vector2(0, 0),
                new Vector2(1, 1)
            );

        else
            ImGui.GetWindowDrawList().AddImage(
                imageNormal,
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.Z, rect.W),
                new Vector2(0, 0),
                new Vector2(1, 1)
            );
    }

    public static System.Numerics.Vector4 GetItemRect()
    {
        return new System.Numerics.Vector4(
            ImGui.GetItemRectMin().X, ImGui.GetItemRectMin().Y,
            ImGui.GetItemRectMax().X, ImGui.GetItemRectMax().Y);
    }

}