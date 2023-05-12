using System.Numerics;
using Engine2D.GameObjects;
using ImGuiNET;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace Engine2D.UI;

internal static class OpenTKUIHelper
{
    public static void DrawComponentWindow(string id, string title, Action tablesToDraw, float size = 100)
    {
        ImGui.PushID(id);
        if (ImGui.CollapsingHeader(title, ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new System.Numerics.Vector4(.19f, .19f, .19f, 1)); //For visibility
            ImGui.BeginChild("##transform_c", new Vector2(0, size), false, 0); // Leave ~100
            ImGui.PopStyleColor();
            //Position Table
            if (ImGui.BeginTable("##transform_t", 2, ImGuiTableFlags.Resizable))
            {
                tablesToDraw.Invoke();

                ImGui.EndTable();
            }

            ImGui.EndChild();
        }

        ImGui.PopID();
    }


    public static bool DrawProperty(string name, ref Vector2 property)
    {
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        if (ImGui.DragFloat2("##" + name, ref property)) changed = true;
        return changed;
    }

    public static bool DrawProperty(string name, ref SpriteColor property)
    {
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        if (ImGui.ColorEdit4("##" + name, ref property.Color)) changed = true;
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
        if (ImGui.DragFloat("##" + name, ref property, dragSpeed)) changed = true;
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

    public static void ShiftCursor(float x, float y)
    {
        var cursor = ImGui.GetCursorPos();
        ImGui.SetCursorPos(new Vector2(cursor.X + x, cursor.Y + y));
    }
}

internal class ImageTextIcon
{
    private static System.Numerics.Vector4 _defaultCol = new(1f, 0f, .0f, 1);

    private readonly string _label;
    private readonly IntPtr _texture;
    private IntPtr _textureActive;
    private readonly IntPtr _textureHovered;

    public bool IsSelected = false;

    public ImageTextIcon(string label, IntPtr texture, IntPtr textureHovered, IntPtr textureActive, string path)
    {
        _label = label;
        _texture = texture;
        _textureHovered = textureHovered;
        _textureActive = textureActive;
        Path = path;
    }

    internal string Path { get; }

    public void Draw(out bool doubleClick, out bool singleClick)
    {
        doubleClick = false;
        singleClick = false;

        //TODO: MOVE TO EDITOR SETTINGS
        var thumbnailSize = AssetBrowser.ThumbnailSize;
        var displayAssetType = AssetBrowser.DisplayAssetType;

        ImGui.PushID(Path);

        if (!IsSelected)
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new System.Numerics.Vector4(.19f, .19f, .19f, 1)); //For visibility
        else
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new System.Numerics.Vector4(1f, .19f, .19f, 1));

        ImGui.BeginChild("##transform_c", new Vector2(thumbnailSize + 30, thumbnailSize + 30), true, 0); // Leave ~100
        ImGui.PopStyleColor();
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));

        const float edgeOffset = 4.0f;

        var textLineHeight = ImGui.GetTextLineHeightWithSpacing() * 2.0f + edgeOffset * 2.0f;
        //float infoPanelHeight = std::max(displayAssetType ? 
        //    thumbnailSize * 0.5f : textLineHeight, textLineHeight);
        float infoPanelHeight = 64;

        var topLeft = ImGui.GetCursorScreenPos();
        Vector2 thumbBottomRight = new(topLeft.X + thumbnailSize - 16, topLeft.Y + thumbnailSize - 16);
        Vector2 infoTopLeft = new(topLeft.X - 16, topLeft.Y + thumbnailSize - 16);
        Vector2 bottomRight = new(topLeft.X - 16 + thumbnailSize, topLeft.Y + thumbnailSize + infoPanelHeight);

        {
            //var drawList = ImGui.GetWindowDrawList();
            //const ImRect itemRect = UI::RectOffset(ImRect(topLeft, bottomRight), 1.0f, 1.0f);
            //drawList->AddRect(itemRect.Min, itemRect.Max, Colours::Theme::propertyField, 6.0f, directory ? 0 : ImDrawFlags_RoundCornersBottom, 2.0f);
        }
        ;

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

        ImGui.PopStyleVar(); // ItemSpacing

        // End of the Item Group
        //======================
        ImGui.EndChild();
        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
            if (ImGui.IsItemHovered())
                doubleClick = true;

        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            if (ImGui.IsItemHovered())
                singleClick = true;
    }
}