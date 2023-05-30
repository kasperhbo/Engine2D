﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using Engine2D.GameObjects;
using ImGuiNET;
using ImTool;
using OpenTK.Mathematics;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace Engine2D.UI;

internal static class OpenTKUIHelper
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

    private static void PrepareProperty(string name)
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

    public static bool DrawProperty(string name, ref float property)
    {
        PrepareProperty(name);
        return Widgets.FloatLabel(ref property, name);
    }
    
    public static bool DrawProperty(string name, ref System.Numerics.Vector2 property)
    {
        PrepareProperty(name);
        return Widgets.Vector2(ref property, name);
    }
    
    public static bool DrawProperty(string name, ref OpenTK.Mathematics.Vector2 property)
    {
        bool changed = false;
        PrepareProperty(name);
        System.Numerics.Vector2 tempProp = new(property.X, property.Y);
        if (Widgets.Vector2(ref tempProp, name)) changed = true;
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

    public static bool DrawProperty(string name, ref KDBColor property)
    {
        PrepareProperty(name);
        ImGui.PushID(name);
        int num1 = 0 | (KDBFloatLabel(ref property.r, "R") ? 1 : 0);
        ImGui.SameLine();
        int num2 = KDBFloatLabel(ref property.g, "G", 4278235392U) ? 1 : 0;
        int num3 = num1 | num2;
        ImGui.SameLine();
        int num4 = KDBFloatLabel(ref property.b, "B", 4289789952U) ? 1 : 0;
        int num5 = num3 | num4;
        ImGui.SameLine();
        int num6 = KDBFloatLabel(ref property.a, "A", 4287299723U) ? 1 : 0;
        int num7 = num5 | num6;
        ImGui.PopID();
        return num7 != 0;
    }

    public static bool KDBFloatLabel(ref float val, string name, uint color = 4278190257, float dragSpeed = 1)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0.0f);
        System.Numerics.Vector2 vector2 = new System.Numerics.Vector2(ImGui.CalcTextSize(name).X + 7f, 2f);
        System.Numerics.Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
        System.Numerics.Vector2 p_max = cursorScreenPos + new System.Numerics.Vector2(vector2.X, ImGui.GetFrameHeight());
        ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
        windowDrawList.AddRectFilled(cursorScreenPos, p_max, color, 6f, ImDrawFlags.RoundCornersLeft);
        windowDrawList.AddText(cursorScreenPos + new System.Numerics.Vector2(3f, 4f), uint.MaxValue, name);
        ImGui.SetCursorPosX(ImGui.GetCursorPos().X + vector2.X);
        ImGui.PushItemWidth(80f);
        int num = ImGui.DragFloat("###" + name, ref val, dragSpeed) ? 1 : 0;
        ImGui.PopStyleVar(1);
        return num != 0;
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

internal class ImageTextIcon
{
    private static System.Numerics.Vector4 _defaultCol = new(1f, 0f, .0f, 1);

    private readonly string _label;
    private readonly IntPtr _texture;
    private readonly IntPtr _textureHovered;
    private GCHandle? _currentlyDraggedHandle;
    private bool _currentlyDragging;
    private IntPtr _textureActive;
    private readonly FileType _type;

    public bool IsSelected = false;

    public ImageTextIcon(string label, IntPtr texture, IntPtr textureHovered, IntPtr textureActive, string path,
        FileType type)
    {
        _label = label;
        _texture = texture;
        _textureHovered = textureHovered;
        _textureActive = textureActive;
        _type = type;
        Path = path;
    }

    internal string Path { get; }

    public unsafe void Draw(out bool doubleClick, out bool singleClick, out bool rightClick)
    {
        doubleClick = false;
        singleClick = false;
        rightClick = false;


        //TODO: MOVE TO EDITOR SETTINGS
        var thumbnailSize = AssetBrowser.ThumbnailSize;
        var displayAssetType = AssetBrowser.DisplayAssetType;

        ImGui.PushID(Path);

        if (!IsSelected)
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new System.Numerics.Vector4(.19f, .19f, .19f, 1)); //For visibility
        else
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new System.Numerics.Vector4(1f, .19f, .19f, 1));

        ImGui.BeginChild("##transform_c", new Vector2(thumbnailSize + 32, thumbnailSize + 32), false,
            ImGuiWindowFlags.NoScrollbar); // Leave ~100
        ImGui.PopStyleColor();
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));

        //float infoPanelHeight = std::max(displayAssetType ? 
        //    thumbnailSize * 0.5f : textLineHeight, textLineHeight);
        float infoPanelHeight = 64;

        var topLeft = ImGui.GetCursorScreenPos();
        Vector2 thumbBottomRight = new(topLeft.X + thumbnailSize - 14, topLeft.Y + thumbnailSize - 14);
        Vector2 infoTopLeft = new(topLeft.X - 16, topLeft.Y + thumbnailSize - 14);
        Vector2 bottomRight = new(topLeft.X - 14 + thumbnailSize, topLeft.Y + thumbnailSize + infoPanelHeight);


        var isFocused = ImGui.IsWindowFocused();

        //isSelected = SelectionManager::IsSelected(SelectionContext::ContentBrowser, m_ID);

        // Fill background
        //----------------

        if (ImGui.IsItemHovered())
            ImGui.GetWindowDrawList().AddRectFilled(topLeft, bottomRight, (int)ImGuiCol.ButtonHovered, 6.0f);

        // Thumbnail
        //==========
        // TODO: replace with actual Asset Thumbnail interface

        if (ImGui.InvisibleButton("##thumbnailButton", new Vector2(thumbnailSize, thumbnailSize))) singleClick = true;
        OpenTKUIHelper.DrawButtonImage(
            _texture, _textureHovered, _texture,
            OpenTKUIHelper.RectExpanded(
                OpenTKUIHelper.GetItemRect(),
                -6,
                -6
            )
        );

        if (_type == FileType.Scene)
            if (ImGui.BeginDragDropSource())
            {
                _currentlyDragging = true;
                _currentlyDraggedHandle ??= GCHandle.Alloc(Path);

                ImGui.SetDragDropPayload("Scene_Drop", GCHandle.ToIntPtr(_currentlyDraggedHandle.Value),
                    (uint)sizeof(IntPtr));

                ImGui.EndDragDropSource();
            }

        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
            if (ImGui.IsItemHovered())
                doubleClick = true;
        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            if (ImGui.IsItemHovered())
                singleClick = true;


        //UI::RectExpanded(UI::GetItemRect(), -6.0f, -6.0f));
        ImGui.Text(_label);
        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
            if (ImGui.IsItemHovered())
                doubleClick = true;
        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            if (ImGui.IsItemHovered())
                singleClick = true;

        if (ImGui.BeginPopupContextWindow("test"))
        {
            if (ImGui.MenuItem("Show In Explorer"))
            {
                Console.WriteLine(Path);
                var proc = Process.Start("explorer.exe", "/select, " + Path);
            }

            if (ImGui.MenuItem("Delete"))
                //TODO: MAKE SURE WINDOW
                throw new NotImplementedException();
            ImGui.EndPopup();
        }


        // End of the Item Group
        //======================
        ImGui.PopStyleVar(); // ItemSpacing

        ImGui.EndChild();


//        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
        //     if (ImGui.IsItemHovered())
        //         doubleClick = true;
        // if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
        //     if (ImGui.IsItemHovered())
        //         singleClick = true;
        // if (ImGui.BeginPopupContextWindow("test"))
        // {
        //     if (ImGui.MenuItem("New Child"))
        //     {
        //         Console.WriteLine("c");
        //     }
        //     ImGui.EndPopup();
        // }
    }
}