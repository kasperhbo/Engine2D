using System.Numerics;
using System.Runtime.InteropServices;
using Engine2D.Components.SpriteAnimations;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Managers;
using Engine2D.SavingLoading;
using ImGuiNET;
using Newtonsoft.Json;

namespace Engine2D.Components.Sprites.SpriteAnimations;

internal class Keyframe
{
    [JsonProperty]internal float Time { get; set; }
    [JsonProperty]internal Frame Frame { get; private set; }

    [JsonConstructor]
    internal Keyframe(float time, Frame value)
    {
        Time = time;
        Frame = value;
    }
}

internal class Animation : AssetBrowserAsset
{
    [JsonIgnore]  private const float TimeLineHeight = 100;
    [JsonIgnore]  private const float KeyFrameMarkerRadius = 4f;
    [JsonIgnore]  private float _startTime = 0f;
    [JsonIgnore]  private float _currentTime = 0f;
    [JsonIgnore]  private float _mouseTime = 0;
    [JsonIgnore]  private bool _isDraggingTimeline;
    [JsonIgnore]  private bool _isDraggingKeyframe;
    [JsonIgnore]  private Keyframe? _currentDraggingKeyFrame = null;
    [JsonIgnore]  private bool _isOpenPopup = false;
    
    [JsonIgnore]  public bool IsPlaying = false;
    [JsonIgnore]  public bool AttachedToSpriteRenderer = false;
    
    [JsonProperty]public float _endTime = 0;
    [JsonProperty]public List<Keyframe> _keyframes = new List<Keyframe>();
    [JsonProperty]public string SavePath = "";
    

    
    public Animation()
    {
        
    }
    
    internal Animation(string savePath)
    {
        this.SavePath = savePath;
    }


    [JsonConstructor]
    internal Animation(List<Keyframe> keyframes, string savePath)
    {
        this._keyframes = keyframes;
        this.SavePath = savePath;
    }

    private void Init(List<Keyframe> keyframes, string savePath)
    {
        this._keyframes = keyframes;
        this.SavePath = savePath;
    }


    internal void Update(double dt)
    {
        if (IsPlaying)
        {
            _currentTime += (float)dt;
            if (_currentTime >= _endTime)
            {
                _currentTime = _startTime;
            }
        }
    }

    internal override void OnGui()
    {
        if(!AttachedToSpriteRenderer)Update(Engine.DeltaTime);
        
        ImGui.Begin("Timeline", ImGuiWindowFlags.NoCollapse);
        
        ShowTimeLine();

        RenderUnderTimeLineItems();
        RenderAnimationPreview();

        ImGui.End();
        
        if (_isDraggingKeyframe)
            HandleKeyFrameDragging();
    }

    internal override void Refresh()
    {
        Init(this._keyframes, this.SavePath);
    }

    private void SortKeyFrames()
    {
        _keyframes.Sort((kf1, kf2) => kf1.Time.CompareTo(kf2.Time));
    }
    
    private void AddKeyFrame(Keyframe keyFrame)
    {
        _keyframes.Add(keyFrame);
        SortKeyFrames();
        Save(true);
    }

    private void ShowTimeLine()
    {
        ImGui.Text("Timeline");
        if (ImGui.Button("Save"))
        {
            Save(true);
        }

        var fullSizeX = ImGui.GetContentRegionMax().X;
        ImGui.BeginChild("TimelineChild", new Vector2(fullSizeX, TimeLineHeight + 50), true, ImGuiWindowFlags.HorizontalScrollbar);
        
        ImGui.GetWindowDrawList().AddRectFilled(ImGui.GetCursorScreenPos(), ImGui.GetCursorScreenPos() + new Vector2(fullSizeX-5, TimeLineHeight), ImGui.GetColorU32(ImGuiCol.FrameBgActive), 4);
        
        var timeLineWidth = fullSizeX - 20;
        InvisibleButtonForMouseEvents(timeLineWidth);

        // Calculate the width per time step
        float timeStepWidth = timeLineWidth / (_endTime - _startTime);

        GetMouseTime(fullSizeX, timeStepWidth);

        DrawTimeText(timeLineWidth, timeStepWidth);

        DrawKeyFrameMarkers(timeLineWidth);
        
        // Calculate the current time position on the timeline
        float currentTimePos = ImGui.GetCursorScreenPos().X + ((_currentTime - _startTime) / (_endTime - _startTime)) * timeLineWidth;

        // Draw current time indicator
        ImGui.GetWindowDrawList().AddLine(new Vector2(currentTimePos, ImGui.GetCursorScreenPos().Y), new Vector2(currentTimePos, ImGui.GetCursorScreenPos().Y + TimeLineHeight), ImGui.GetColorU32(ImGuiCol.Text), 2);
        
        ImGui.EndChild();
    }

    private void RenderAnimationPreview()
    {
        if (_keyframes.Count <= 0) return;
        
        Sprite sprite = GetCurrentSprite();
        
        if(sprite != null)
        {
            // if (sprite.Texture == null) return;
            ImGui.Image(sprite.Texture.TexID, new Vector2(128), sprite.TextureCoords[3],
                sprite.TextureCoords[1]);
        }
    }

    public Sprite? GetSpriteBasedOnTime(float time){
        if(_keyframes.Count <= 0) return null;
        int index = GetKeyFrameBasedOnTime(time);
        if(index >= _keyframes.Count) return null;
        
        Frame currentFrame = _keyframes[index].Frame;
        SpriteSheet spriteSheet = ResourceManager.GetItem<SpriteSheet>(currentFrame.SpriteSheetPath);
        int spriteIndex = currentFrame.SpriteSheetSpriteIndex;

        return spriteSheet.GetSprite(spriteIndex);
    }
    
    public Sprite? GetCurrentSprite()
    {
        return GetSpriteBasedOnTime(_currentTime);
    }

    private void RenderUnderTimeLineItems()
    {
        ImGui.Text("Start time: ");
        ImGui.SameLine();
        if (ImGui.Button("+", new(22)))
        {
            _startTime += 1;
        }

        ImGui.SameLine();
        ImGui.SetNextItemWidth(50);
        ImGui.DragFloat("##StartTime", ref _startTime, 0, 1000, 0.1f);
        ImGui.SameLine();
        if (ImGui.Button("-", new(22)))
        {
            _startTime -= 1;
        }

        ImGui.SameLine();
        ImGui.Dummy(new(20, 0));
        ImGui.SameLine();
        ImGui.Text("End time: ");
        ImGui.SameLine();
        ImGui.PushID("PLUSEND");
        if (ImGui.Button("+", new(22)))
        {
            Console.WriteLine("End");
            _endTime += 1;
        }
        ImGui.SameLine();
        ImGui.SetNextItemWidth(50);
        ImGui.DragFloat("##EndTime", ref _endTime, 0, 1000, 0.1f);
        ImGui.SameLine();
        if (ImGui.Button("-", new(22)))
        {
            _endTime -= 1;
        }
        
        ImGui.PopID();
        
        ImGui.Text("save path: " + SavePath);

        if (IsPlaying)
        {
            if (ImGui.Button("Pause"))
            {
                IsPlaying = false;
            }
        }
        else
        {
            if (ImGui.Button("Play"))
            {
                IsPlaying = true;
            }
        }
    }

    private void InvisibleButtonForMouseEvents(float timeLineWidth)
    {
        var pos = ImGui.GetCursorScreenPos();
        var pos2 = ImGui.GetCursorPos();
        
        ImGui.InvisibleButton("##TimelineBackground", new Vector2(timeLineWidth, TimeLineHeight));
        HandleSpriteDrop();
        
        ImGui.SetCursorScreenPos(pos);
        ImGui.SetCursorPos(pos2);
    }

    private void HandleSpriteDrop()
    {
        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGui.AcceptDragDropPayload("spritesheet_drop");
            if (payload.IsValidPayload())
            {
                // Retrieve the payload data as byte array
                byte[] payloadData = new byte[payload.DataSize];
                Marshal.Copy(payload.Data, payloadData, 0, payloadData.Length);

                // Deserialize the sprite from the payload data
                Sprite droppedSprite = SpriteRenderer.DeserializeSprite(payloadData);
                Frame frame = new Frame( droppedSprite.Index, droppedSprite.FullSavePath,_mouseTime);
                Keyframe keyFrame = new Keyframe(_mouseTime, frame);
                AddKeyFrame(keyFrame);
            }

            ImGui.EndDragDropTarget();
        }

    }


    private void DrawKeyFrameMarkers (float timeLineWidth)
    {
        // Draw keyframe markers
        for (int i = 0; i < _keyframes.Count; i++)
        {
            var keyframe = _keyframes[i];
            float markerX = ImGui.GetCursorScreenPos().X +
                            ((keyframe.Time - _startTime) / (_endTime - _startTime)) * timeLineWidth;

            ImGui.PushID(i); // Push ID for ImGui widget differentiation
            //
            ImGui.GetWindowDrawList().AddLine(
                new Vector2(markerX, ImGui.GetCursorScreenPos().Y),
                new Vector2(markerX, ImGui.GetCursorScreenPos().Y + TimeLineHeight),
                ImGui.GetColorU32(new Vector4(1, 0, 0, 1)), KeyFrameMarkerRadius);
            
            
            
            Sprite sprite = GetSpriteBasedOnTime(keyframe.Time);

            if (sprite != null)
            {
                IntPtr texID = sprite.Texture.TexID;

                ImGui.GetWindowDrawList().AddImage(texID,
                    new Vector2(markerX - 16, ImGui.GetCursorScreenPos().Y + (TimeLineHeight / 2 - 16)),
                    new Vector2(markerX + 16, ImGui.GetCursorScreenPos().Y + (TimeLineHeight / 2 + 16)),
                    sprite.TextureCoords[3],
                    sprite.TextureCoords[1]
                );
            }
            //
            // Check if mouse is hovering over the keyframe marker
            if (ImGui.IsMouseHoveringRect(
                    new Vector2(markerX - KeyFrameMarkerRadius, ImGui.GetCursorScreenPos().Y),
                    new Vector2(markerX + KeyFrameMarkerRadius, ImGui.GetCursorScreenPos().Y + TimeLineHeight)))
            {
                if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
                {
                    if(_isDraggingTimeline == false)
                    {
                        if (_isDraggingKeyframe == false)
                        {
                            _isDraggingKeyframe = true;
                            _currentDraggingKeyFrame = keyframe;
                        }
                    }
                }

                if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
                {
                    if (_isDraggingTimeline == false)
                    {
                        _isOpenPopup = true;
                    }
                }
            }
            
            if (_isOpenPopup)
            {
                ImGui.OpenPopup("Components Popup");
            }
            
            if (ImGui.BeginPopup("Components Popup"))
            {
                if (ImGui.Selectable("Delete"))
                {
                    _keyframes.RemoveAt(i);
                    SortKeyFrames();
                }
                ImGui.EndPopup();
            }
            
            if (ImGui.IsAnyMouseDown())
            {
                _isOpenPopup = false;
            }

            if (_isDraggingKeyframe && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                _isDraggingKeyframe = false;
                _currentDraggingKeyFrame.Time = _mouseTime;
                _currentDraggingKeyFrame = null;
                SortKeyFrames();
            }
            
            ImGui.PopID(); // Pop ID
        }
    }

    private void HandleKeyFrameDragging()
    {
        if (_currentDraggingKeyFrame == null) return;

        _currentDraggingKeyFrame.Time = _mouseTime;
    }
    
    private void DrawTimeText(float timeLineWidth, float timeStepWidth)
    {
        var ogPos = ImGui.GetCursorScreenPos();

        float y = ImGui.GetCursorScreenPos().Y + TimeLineHeight;

        // Calculate the number of steps based on the minimum step width (e.g., 50 pixels)
        int numSteps = Math.Max((int)Math.Ceiling(timeLineWidth / 50f), 1);

        // Calculate the adjusted time step based on the number of steps
        float adjustedTimeStep = (_endTime - _startTime) / numSteps;
       
        // Draw time labels at regular intervals
        for (int i = 0; i <= numSteps; i++)
        {
            float time = _startTime + (i * adjustedTimeStep);
            float markerX = ImGui.GetCursorScreenPos().X + (time - _startTime) * timeStepWidth;

            // Draw time marker line
            ImGui.GetWindowDrawList().AddLine(new Vector2(markerX, ImGui.GetCursorScreenPos().Y), new Vector2(markerX, ImGui.GetCursorScreenPos().Y + TimeLineHeight), ImGui.GetColorU32(ImGuiCol.PlotLines), 1);
            
            // Draw time label
            ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetColorU32(ImGuiCol.Text));
            ImGui.SetCursorScreenPos(new Vector2(markerX - ImGui.CalcTextSize(time.ToString("0.00")).X / 2f, y));
            ImGui.Text(time.ToString("0.00"));
            ImGui.PopStyleColor();
        }
        
        ImGui.SetCursorScreenPos(ogPos);
    }
    
    private int GetCurrentKeyframeIndex()
    {
        return GetKeyFrameBasedOnTime(_currentTime);
    }
    
    private int GetKeyFrameBasedOnTime(float time)
    {
        int index = 0;

        for (int i = 0; i < _keyframes.Count; i++)
        {
            if (time >= _keyframes[i].Time)
            {
                index = i;
            }
            else
            {
                break;
            }
        }

        return index;
    }

    private void GetMouseTime(float timelineWidth, float timeStepWidth) 
    {
        // Get the mouse position relative to the timeline child window
        Vector2 mousePos = ImGui.GetMousePos() - ImGui.GetItemRectMin();

        // Check if the mouse is hovering over the timeline background
        if (mousePos.X >= 0 && mousePos.X <= timelineWidth && mousePos.Y >= 0 && mousePos.Y <= TimeLineHeight)
        {
            // Calculate the corresponding time based on the mouse position
            _mouseTime = _startTime + mousePos.X / timeStepWidth;
            
            if(ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                _currentTime = _mouseTime;
            }

            if (ImGui.IsMouseDragging(ImGuiMouseButton.Left))
            {
                _isDraggingTimeline = true;
                _currentTime = _mouseTime;
            }
            else
            {
                _isDraggingTimeline = false;
            }
            
            // Draw a tooltip with the cursor time
            ImGui.BeginTooltip();
            ImGui.Text("Cursor Time: " + _mouseTime.ToString("0.00"));
            ImGui.EndTooltip();
        }
    }
    
    internal void Save(bool overwrite = false)
    {
        var animc = new SaveAnimationClass(SavePath,
            this, overwrite);
        
        ResourceManager.AnimationsToSave.Add(animc);
    }
    
    
}