using System.Diagnostics;
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
    public static void DrawComponentWindow(string id, string title, Action tablesToDraw, float isze = 100)
    {
        ImGui.PushID(id);
        
        
        
        if (ImGui.CollapsingHeader(title, ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.PushStyleColor(ImGuiCol.TableBorderStrong, new System.Numerics.Vector4(.19f, .19f, .19f, 1)); //For visibility
            // ImGui.BeginChild("##transform_c"); // Leave ~100
            
            //Position Table
            if (ImGui.BeginTable("##transform_t", 2, ImGuiTableFlags.Resizable))
            {
                tablesToDraw.Invoke();
        
                ImGui.EndTable();
            }
        
            
            ImGui.PopStyleColor();
            // ImGui.EndChild();
        }
        ImGui.PopID();
    }
    
    

    // public static void BeginGroupPanel(string name, Vector2 size)
    // {
    //     ImGui.BeginGroup();
    //
    //     var cursorPos = ImGui.GetCursorScreenPos();
    //     Vector2 itemSpacing = ImGui.GetStyle().ItemSpacing;
    //     ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,new Vector2(0.0f, 0.0f));
    //     ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0.0f, 0.0f));
    //
    //     float frameHeight = ImGui.GetFrameHeight();
    //     ImGui.BeginGroup();
    //
    //     var effectiveSize = size;
    //     if (size.X < 0.0f)
    //         effectiveSize.X = ImGui.GetContentRegionAvail().X;
    //     else
    //         effectiveSize.X = size.X;
    //     
    //     ImGui.Dummy(new(effectiveSize.X, 0.0f));
    //
    //     ImGui.Dummy(new(frameHeight * 0.5f, 0.0f));
    //     ImGui.SameLine(0.0f, 0.0f);
    //     ImGui.BeginGroup();
    //     ImGui.Dummy(new(frameHeight * 0.5f, 0.0f));
    //     ImGui.SameLine(0.0f, 0.0f);
    //     ImGui.TextUnformatted(name);
    //     var labelMin = ImGui.GetItemRectMin();
    //     var labelMax = ImGui.GetItemRectMax();
    //     ImGui.SameLine(0.0f, 0.0f);
    //     ImGui.Dummy(new(0.0f, frameHeight + itemSpacing.Y));
    //     ImGui.BeginGroup();
    //
    //     //ImGui.GetWindowDrawList()->AddRect(labelMin, labelMax, IM_COL32(255, 0, 255, 255));
    //
    //     ImGui.PopStyleVar(2);
    //
    //     ImGui.GetCurrentWindow().ContentRegionRect.Max.X -= frameHeight * 0.5f;
    //     ImGui.GetCurrentWindow().WorkRect.Max.X          -= frameHeight * 0.5f;
    //     ImGui.GetCurrentWindow().InnerRect.Max.X         -= frameHeight * 0.5f;
    //     ImGui.GetCurrentWindow().Size.X                   -= frameHeight;
    //
    //     var itemWidth = ImGui.CalcItemWidth();
    //     ImGui.PushItemWidth( itemWidth - frameHeight);
    //
    //     // s_GroupPanelLabelStack.push_back(ImRect(labelMin, labelMax));
    //
    // }
    //
    // public void EndGroupPanel()
    // {
    //     ImGui::PopItemWidth();
    //
    //     var itemSpacing = ImGui.GetStyle().ItemSpacing;
    //
    //     ImGui.PushStyleVar(ImGuiStyleVar_FramePadding, ImVec2(0.0f, 0.0f));
    //     ImGui.PushStyleVar(ImGuiStyleVar_ItemSpacing, ImVec2(0.0f, 0.0f));
    //
    //     var frameHeight = ImGui.GetFrameHeight();
    //
    //     ImGui.EndGroup();
    //
    //     //ImGui.GetWindowDrawList()->AddRectFilled(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), IM_COL32(0, 255, 0, 64), 4.0f);
    //
    //     ImGui.EndGroup();
    //
    //     ImGui.SameLine(0.0f, 0.0f);
    //     ImGui.Dummy(new(frameHeight * 0.5f, 0.0f));
    //     ImGui.Dummy(new(0.0, frameHeight - frameHeight * 0.5f - itemSpacing.y));
    //
    //     ImGui.EndGroup();
    //
    //     var itemMin = ImGui.GetItemRectMin();
    //     var itemMax = ImGui.GetItemRectMax();
    //     //ImGui.GetWindowDrawList()->AddRectFilled(itemMin, itemMax, IM_COL32(255, 0, 0, 64), 4.0f);
    //
    //     
    //     
    //
    //     Vector2 halfFrame = new Vector2(frameHeight * 0.25f, frameHeight) * 0.5f;
    //     
    //     ImRect frameRect = new ImRect(itemMin + halfFrame, itemMax - (halfFrame.X, 0.0f));
    //     
    //     labelRect.Min.x -= itemSpacing.x;
    //     labelRect.Max.x += itemSpacing.x;
    //     for (int i = 0; i < 4; ++i)
    //     {
    //         switch (i)
    //         {
    //             // left half-plane
    //             case 0:
    //                 ImGui.PushClipRect(new(-FLT_MAX, -FLT_MAX), ImVec2(labelRect.Min.x, FLT_MAX), true);
    //                 break;
    //             // right half-plane
    //             case 1:
    //                 ImGui.PushClipRect(new(labelRect.Max.x, -FLT_MAX), ImVec2(FLT_MAX, FLT_MAX), true);
    //                 break;
    //             // top
    //             case 2:
    //                 ImGui.PushClipRect(new(labelRect.Min.x, -FLT_MAX), ImVec2(labelRect.Max.x, labelRect.Min.y),
    //                     true);
    //                 break;
    //             // bottom
    //             case 3:
    //                 ImGui.PushClipRect(ImVec2(labelRect.Min.x, labelRect.Max.y), ImVec2(labelRect.Max.x, FLT_MAX),
    //                     true);
    //                 break;
    //         }
    //
    //         ImGui.GetWindowDrawList()->AddRect(
    //             frameRect.Min, frameRect.Max,
    //             new ImColor(ImGui.GetStyleColorVec4(ImGuiCol_Border)),
    //             halfFrame.x);
    //
    //         ImGui.PopClipRect();
    //     }
    //
    //     ImGui.PopStyleVar(2);
    //
    //     ImGui.GetCurrentWindow().ContentRegionRect.Max.x += frameHeight * 0.5f;
    //     ImGui.GetCurrentWindow().WorkRect.Max.x += frameHeight * 0.5f;
    //     ImGui.GetCurrentWindow().InnerRect.Max.x += frameHeight * 0.5f;
    //
    //     ImGui.GetCurrentWindow().Size.x += frameHeight;
    //
    //     ImGui.Dummy(new(0.0f, 0.0f));
    //
    //     ImGui.EndGroup();
    // }


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


    public static bool DrawProperty(string name, ref Vector2 property)
    {
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        if (ImTool.Widgets.Vector2(ref property, name)) changed = true;
        // if (ImGui.DragFloat2("##" + name, ref property)) changed = true;
        return changed;
    }

    public static bool DrawProperty(string name, ref OpenTK.Mathematics.Vector2 property)
    {
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        Vector2 data = new(property.X, property.Y);
        if (ImGui.DragFloat2("##" + name, ref data)) changed = true;
        property = new OpenTK.Mathematics.Vector2(data.X, data.Y);
        return changed;
    }
    

    public static bool DrawProperty(string name, ref bool property)
    {
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        if (ImGui.Checkbox("##" + name, ref property)) changed = true;
        return changed;
    }

    public static bool DrawProperty(string name, ref int property)
    {
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        if (ImGui.DragInt("##" + name, ref property)) changed = true;
        return changed;
    }

    public static bool DrawProperty(string name, ref Vector3 property)
    {
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        if (ImGui.DragFloat3("##" + name, ref property)) changed = true;
        return changed;
    }

    public static bool DrawProperty(string name, ref Vector4 property, float dragSpeed = 0.1f)
    {
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        System.Numerics.Vector4 copy = new(property.X, property.Y, property.Z, property.W);

        if (ImGui.DragFloat4("##" + name, ref copy, dragSpeed)) changed = true;

        property = new Vector4(copy.X, copy.Y, copy.Z, copy.W);
        return changed;
    }

    public static bool DrawProperty(string name, ref System.Numerics.Vector4 property)
    {
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        if (ImGui.DragFloat4("##" + name, ref property)) changed = true;
        return changed;
    }

    public static bool DrawProperty(string name, ref float property, float dragSpeed = 0.1f)
    {
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        if (ImTool.Widgets.FloatLabel(ref property, "")) changed = true;
        // if (ImGui.DragFloat("##" + name, ref property, dragSpeed)) changed = true;
        return changed;
    }

    public static bool DrawProperty(string name, ref float property, float min, float max, float dragSpeed = 0.1f)
    {
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        if (ImGui.DragFloat("##" + name, ref property, dragSpeed, min, max)) changed = true;
        return changed;
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

    public static System.Numerics.Vector4 RectExpanded(System.Numerics.Vector4 rect, float x, float y)
    {
        var result = rect;


        result.X -= x;
        result.Y -= y;

        result.Z += x;
        result.W += y;

        return result;
    }


    public static OpenTK.Mathematics.Vector2 DrawProperty(string name, OpenTK.Mathematics.Vector2 property)
    {
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        Vector2 copy = new(property.X, property.Y);

        if (ImGui.DragFloat2("##" + name, ref copy, 0.1f)) changed = true;

        property = new OpenTK.Mathematics.Vector2(copy.X, copy.Y);
        return property;
        // return changed;
    }
    
    public static bool DrawProperty(string name, ref SpriteColor color)
    {
        ImGui.PushID(name);
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        
        int num1 = 0 | (Widgets.FloatLabel(ref color.Color.X, "R") ? 1 : 0);
        ImGui.SameLine();
        int num2 = Widgets.FloatLabel(ref color.Color.Y, "G", 4278235392U) ? 1 : 0;
        int num3 = num1 | num2;
        ImGui.SameLine();
        int num4 = Widgets.FloatLabel(ref color.Color.Z, "B", 4289789952U) ? 1 : 0;
        int num5 = num3 | num4;
        ImGui.SameLine();
        int num6 = Widgets.FloatLabel(ref color.Color.W, "A", 4287299723U) ? 1 : 0;
        int num7 = num5 | num6;
        ImGui.PopID();
        return num7 != 0;
    }

    public static bool DrawProperty(string name, ref OpenTK.Mathematics.Vector3 property)
    {
        ImGui.PushID(name);
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        
        int num1 = 0 | (Widgets.FloatLabel(ref property.X, "X") ? 1 : 0);
        ImGui.SameLine();
        int num2 = Widgets.FloatLabel(ref property.Y, "Y", 4278235392U) ? 1 : 0;
        int num3 = num1 | num2;
        ImGui.SameLine();
        int num4 = Widgets.FloatLabel(ref property.Z, "Z", 4289789952U) ? 1 : 0;
        int num5 = num3 | num4;
        ImGui.PopID();
        return num5 != 0;
    }
    
    public static bool DrawProperty(string name, ref OpenTK.Mathematics.Quaternion q)
    {
        ImGui.PushID(name);
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();

        System.Numerics.Quaternion quat = new System.Numerics.Quaternion(q.X, q.Y, q.Z, q.W);
        
        ImGui.PushID(name);
        int num1 = 0 | (Widgets.FloatLabel(ref quat.X, "X") ? 1 : 0);
        ImGui.SameLine();
        int num2 = Widgets.FloatLabel(ref quat.Y, "Y", 4278235392U) ? 1 : 0;
        int num3 = num1 | num2;
        ImGui.SameLine();
        int num4 = Widgets.FloatLabel(ref quat.Z, "Z", 4289789952U) ? 1 : 0;
        int num5 = num3 | num4;
        ImGui.SameLine();
        int num6 = Widgets.FloatLabel(ref quat.W, "W", 4287299723U) ? 1 : 0;
        int num7 = num5 | num6;
        ImGui.PopID();
        q = new Quaternion(quat.X, quat.Y, quat.Z, quat.W);
        return num7 != 0;
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