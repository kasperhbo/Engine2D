using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using Engine2D.Components;
using Engine2D.GameObjects;
using Engine2D.Testing;
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

    public static void BeginPopup()
    {
        TestInput.Focussed = false;
    }

    public static void EndPopup()
    {
        TestInput.Focussed = true;
    }

    public static bool DrawProperty(string name, ref string property)
    {
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        if (ImGui.InputText("##" + name, ref property, 256)) changed = true;
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


    public static bool DrawProperty(string name, ref SpriteColor property)
    {
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        if (ImGui.ColorEdit4("##" + name, ref property.Color)) changed = true;
        return changed;
    }

    public static bool DrawProperty(string name, ref LightColor property)
    {
        var changed = false;
        ImGui.TableNextColumn();
        ImGui.Text(name);
        ImGui.TableNextColumn();
        Vector3 data = new(property.R, property.G, property.B);
        if (ImGui.ColorEdit3("##" + name, ref data)) changed = true;
        property = new LightColor(data.X, data.Y, data.Z);
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
}

internal class ImageTextIcon
{
    private static System.Numerics.Vector4 _defaultCol = new(1f, 0f, .0f, 1);

    internal  string Label { get; private set; }
    internal string Path { get; private set;}
    
    private readonly IntPtr _texture;
    private readonly IntPtr _textureHovered;
    private readonly FileType _type;
    private GCHandle? _currentlyDraggedHandle;
    private bool _currentlyDragging;
    private IntPtr _textureActive;

    public bool IsSelected = false;

    public ImageTextIcon(string label, IntPtr texture, IntPtr textureHovered, IntPtr textureActive, string path,
        FileType type)
    {
        Label = label;
        Path = path;
        _texture = texture;
        _textureHovered = textureHovered;
        _textureActive = textureActive;
        _type = type;
    }

    

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
        ImGui.Text(Label);
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
    }
}