using System.Runtime.InteropServices;
using System.Text;
using Engine2D.Components.Sprites;
using Engine2D.Components.Sprites.SpriteAnimations;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Managers;
using Engine2D.Rendering;
using ImGuiNET;
using Newtonsoft.Json;
using Vulkan;

namespace Engine2D.Components.SpriteAnimations;

internal class SpriteAnimator : Component
{

    [JsonIgnore] public Animation? Animation { get; private set; } = null;
    [JsonIgnore] private SpriteRenderer _spriteRenderer = null;
    
    [JsonProperty]private string _animationPath = ""; 
    
    internal override void Init(Gameobject parent, Renderer? renderer)
    {
        base.Init(parent, renderer);
        Initialize();
    }

    internal override void Init(Gameobject parent)
    {
        base.Init(parent);
        Initialize();
    }

    private void Initialize()
    {
        GetSpriteRenderer();
        GetAnimation();
    }

    public void Refresh()
    {
        Initialize();
    }
    
    private void GetSpriteRenderer()
    {
        _spriteRenderer = Parent.GetComponent<SpriteRenderer>();
    }
    
    private void GetAnimation()
    {
        //Error check if something goes wrong just remove it from the sprite renderer
        if (_animationPath == "")
        {
            _animationPath = "";
            _spriteRenderer.SpriteSheetSpriteIndex = -1;
            _spriteRenderer.SpriteSheetPath = "";
            _spriteRenderer.HasSpriteSheet = false;
            _spriteRenderer.Refresh();
            return;
        }
        
        var loaded = ResourceManager.GetItem<Animation>(_animationPath);
        if (loaded == null)
        {
            _animationPath = "";
            _spriteRenderer.SpriteSheetSpriteIndex = -1;
            _spriteRenderer.SpriteSheetPath = "";
            _spriteRenderer.HasSpriteSheet = false;
            _spriteRenderer.Refresh();
            return;
        }
        //if (loaded == null) return;
        
        var animInstance = (Animation)Activator.CreateInstance(loaded.GetType());
        
        animInstance._endTime = loaded._endTime;
        animInstance._keyframes = loaded._keyframes;
        animInstance.SavePath = loaded.SavePath;
        animInstance.AttachedToSpriteRenderer = true;
        
        Animation = animInstance;
        
        Engine.Get().CurrentSelectedAnimationAssetBrowserAsset = Animation;
    }
    

    public override void GameUpdate(double dt)
    {
        base.GameUpdate(dt);
    }

    public override void EditorUpdate(double dt)
    {
        base.EditorUpdate(dt);

        if (Animation != null)
        {
            Animation.Update(dt);
            
            if (_spriteRenderer != null)
            {
                var currentFrame = Animation.GetCurrentSprite();
                
                if(currentFrame == null)
                    return;
                
                _spriteRenderer.SetSprite(currentFrame.Index, currentFrame.FullSavePath);
            }
        }
    }

    public override void ImGuiFields()
    {
        base.ImGuiFields();
        if (ImGui.Button("play"))
        {
            if(Animation != null)
                Animation.IsPlaying = true;
        }
        if (ImGui.Button("Stop"))
        {
            if(Animation != null)
                Animation.IsPlaying = false;
        }

        
        ImGui.Button("Animator");
        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGui.AcceptDragDropPayload("animation_drop");
            if (payload.IsValidPayload())
            {
                var filename = (string)GCHandle.FromIntPtr(payload.Data).Target;
                _animationPath = filename;
                Initialize();
            }
            ImGui.EndDragDropTarget();
        }
    }
}