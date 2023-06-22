using System.Numerics;
using System.Runtime.InteropServices;
using Engine2D.Components.Sprites;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Managers;
using Engine2D.UI;
using ImGuiNET;
using ImGuizmoNET;
using Newtonsoft.Json;

namespace Engine2D.Components.SpriteAnimations;

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
    
    [JsonIgnore]private float _startTime = 0f;
    [JsonIgnore]private float _currentTime = 0f;
    [JsonIgnore]private SpriteSheet? _spriteSheet = null;
    [JsonIgnore]private bool _isPlaying = false;
    [JsonIgnore]private float _mouseTime = 0;
    [JsonIgnore] private const float TimeLineHeight = 100;

    [JsonProperty]private float _endTime = 0;
    [JsonProperty]private List<Keyframe> _keyframes = new List<Keyframe>();
    [JsonProperty]private string _savePath = "";

    internal Animation(string savePath)
    {
        this._savePath = savePath;
    }


    [JsonConstructor]
    internal Animation(List<Keyframe> keyframes, string savePath)
    {
        this._keyframes = keyframes;
        this._savePath = savePath;
    }


    internal override void OnGui()
    {
        ShowTimeLine();

        if (_isPlaying)
        {
            _currentTime += (float)Engine.DeltaTime;
            if (_currentTime >= _endTime)
            {
                _currentTime = 0f;
            }

        }
    }

    private void AddKeyFrame(Keyframe keyFrame)
    {
        _keyframes.Add(keyFrame);
        _keyframes.Sort((kf1, kf2) => kf1.Time.CompareTo(kf2.Time));
        Save();
    }

    private void ShowTimeLine()
    {
        // Create ImGui window
        
        ImGui.Begin("Timeline Example", ImGuiWindowFlags.NoCollapse);

        ImGui.Text("Timeline");
        if (ImGui.Button("Save"))
        {
            Save();
        }

        var fullSizeX = ImGui.GetContentRegionMax().X;
        ImGui.BeginChild("TimelineChild", new Vector2(fullSizeX, 150), true, ImGuiWindowFlags.HorizontalScrollbar);

        
        // Draw timeline background
        // ImGui.GetWindowDrawList().AddRectFilled(ImGui.GetCursorScreenPos(), ImGui.GetCursorScreenPos() + new Vector2(fullSizeX, timeLineHeight), ImGui.GetColorU32(new Vector4(1,0,0,1)), 4);
        ImGui.GetWindowDrawList().AddRectFilled(ImGui.GetCursorScreenPos(), ImGui.GetCursorScreenPos() + new Vector2(fullSizeX-5, TimeLineHeight), ImGui.GetColorU32(ImGuiCol.FrameBgActive), 4);
        // Add an invisible button on top of the timeline background to capture mouse events
        var timeLineWidth = fullSizeX - 20;
        var pos = ImGui.GetCursorScreenPos();
        var pos2 = ImGui.GetCursorPos();
        
        ImGui.InvisibleButton("##TimelineBackground", new Vector2(timeLineWidth, TimeLineHeight));
        
        ImGui.SetCursorScreenPos(pos);
        ImGui.SetCursorPos(pos2);
        
        
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
                Console.WriteLine("dropped sprite at time" + _mouseTime);
                Frame frame = new Frame( droppedSprite.Index, droppedSprite.FullSavePath,_mouseTime);
                Keyframe keyFrame = new Keyframe(_mouseTime, frame);
                AddKeyFrame(keyFrame);
            }

            ImGui.EndDragDropTarget();
        }
        
        // Calculate the width per time step
        float timeStepWidth = timeLineWidth / (_endTime - _startTime);

        GetMouseTime(fullSizeX, timeStepWidth);

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
            ImGui.SetCursorScreenPos(new Vector2(markerX - ImGui.CalcTextSize(time.ToString("0.0")).X / 2f, y));
            ImGui.Text(time.ToString("0.0"));
            ImGui.PopStyleColor();
        }
        
        
        //Reset position
        ImGui.SetCursorScreenPos(ogPos);
        // Draw keyframe markers
        for (int i = 0; i < _keyframes.Count; i++)
        {
            var keyframe = _keyframes[i];
            float markerX = ImGui.GetCursorScreenPos().X + ((keyframe.Time - _startTime) / (_endTime - _startTime)) * timeLineWidth;

            ImGui.PushID(i); // Push ID for ImGui widget differentiation

            ImGui.GetWindowDrawList().AddLine(new Vector2(markerX, ImGui.GetCursorScreenPos().Y), new Vector2(markerX, ImGui.GetCursorScreenPos().Y + TimeLineHeight), ImGui.GetColorU32(new Vector4(1,0,0,1)), 2);
            
            ImGui.PopID(); // Pop ID
        }

        // Calculate the current time position on the timeline
        float currentTimePos = ImGui.GetCursorScreenPos().X + ((_currentTime - _startTime) / (_endTime - _startTime)) * timeLineWidth;

        // Draw current time indicator
        ImGui.GetWindowDrawList().AddLine(new Vector2(currentTimePos, ImGui.GetCursorScreenPos().Y), new Vector2(currentTimePos, ImGui.GetCursorScreenPos().Y + TimeLineHeight), ImGui.GetColorU32(ImGuiCol.Text), 2);
        
        ImGui.EndChild();

        if (_isPlaying)
        {
            if (ImGui.Button("Pause"))
            {
                _isPlaying = false;
            }
        }
        else
        {
            if (ImGui.Button("Play"))
            {
                _isPlaying = true;
            }
        }

        ImGui.SliderFloat("End Time", ref _endTime, 0, 100);
        

        Frame currentFrame = _keyframes[GetCurrentKeyframeIndex()].Frame;
        SpriteSheet spriteSheet = ResourceManager.GetItem<SpriteSheet>(currentFrame.SpriteSheetPath);
        int spriteIndex = currentFrame.SpriteSheetSpriteIndex;
       
        Sprite sprite = spriteSheet.Sprites[spriteIndex];

        if(sprite != null)
        {
            ImGui.Image(sprite.Texture.TexID, new Vector2(128), sprite.TextureCoords[3],
                sprite.TextureCoords[1]);
        }

        ImGui.End();
    }
    
    private int GetCurrentKeyframeIndex()
    {
        int index = 0;

        for (int i = 0; i < _keyframes.Count; i++)
        {
            if (_currentTime >= _keyframes[i].Time)
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

            // Draw a tooltip with the cursor time
            ImGui.BeginTooltip();
            ImGui.Text("Cursor Time: " + _mouseTime.ToString("0.00"));
            ImGui.EndTooltip();
        }
    }
    
    internal void Save()
    {
        AssetName = _savePath;
        ResourceManager.SaveAnimation(_savePath,
            this, null, true);
    }
}