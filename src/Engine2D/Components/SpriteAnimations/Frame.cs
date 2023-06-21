using System.Numerics;
using Engine2D.Components.Sprites;
using Engine2D.Core;
using Engine2D.Managers;
using ImGuiNET;
using Newtonsoft.Json;

namespace Engine2D.Components.SpriteAnimations;

internal class Frame
{
    private string Type = "Engine2D.Components.SpriteAnimations.Frame";
    
    [JsonProperty]internal float FrameTime;
    [JsonProperty] internal int SpriteSheetSpriteIndex = 0;
    [JsonProperty] internal string? SpriteSheetPath = "";
    
    internal Frame() {

    }

    internal Frame(int spriteSheetSpriteIndex, string spriteSheetPath,float frameTime) {
        this.FrameTime = frameTime;
        this.SpriteSheetSpriteIndex = spriteSheetSpriteIndex;
        this.SpriteSheetPath = spriteSheetPath;
    }
}