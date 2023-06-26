using Newtonsoft.Json;

namespace Engine2D.Core;

public abstract class Asset
{
    [JsonProperty] internal bool IsDead = false;
    internal abstract void OnGui();
}