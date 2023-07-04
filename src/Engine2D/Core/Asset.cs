using Newtonsoft.Json;

namespace Engine2D.Core;

public abstract class Asset
{
    [JsonProperty] public bool IsDead = false;
    internal abstract void OnGui();
}