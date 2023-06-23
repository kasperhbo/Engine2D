namespace Engine2D.Core;

internal abstract class AssetBrowserAsset
{
    internal string? AssetName;
    internal abstract void OnGui();
    internal abstract void Refresh();
}