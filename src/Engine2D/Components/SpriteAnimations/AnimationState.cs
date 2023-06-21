using Engine2D.Components.Sprites;
using Engine2D.GameObjects;
using Engine2D.Managers;
using Newtonsoft.Json;

namespace Engine2D.Components.SpriteAnimations;

internal class SpriteAnimator : Component
{
    [JsonProperty]internal List<Frame> AnimationFrames = new();

    private float _time = 0.0f;
    private int _currentSprite = 0;
    private int _previousSprite = -1;
    
    private bool _doesLoop = true;
    private bool _isPlaying = true;
    
    private int _framesElapsed = 0;
    
    internal SpriteAnimator()  : base(){
        for (int i = 0; i < 10; i++)
        {
            Frame frame = new Frame(i, @"D:\dev\Engine2D\src\ExampleGame\Assets\bigSpritesheet (1).spritesheet", .1f);
            AnimationFrames.Add(frame);
        }   
    }
    
    public override void GameUpdate(double dt)
    {
        base.GameUpdate(dt);
    }

    public override void EditorUpdate(double dt)
    {
        if (_isPlaying)
        {
            if (_currentSprite < AnimationFrames.Count) {
                _time -= (float)dt;
                if (_time <= 0) {
                    if (!(_currentSprite == AnimationFrames.Count - 1 && !_doesLoop)) {
                        _currentSprite = (_currentSprite + 1) % AnimationFrames.Count;
                        if (_previousSprite != _currentSprite)
                        {
                            _previousSprite = _currentSprite;
                            _framesElapsed++;
                            Parent.GetComponent<SpriteRenderer>().SetSprite(AnimationFrames[_currentSprite].SpriteSheetSpriteIndex, AnimationFrames[_currentSprite].SpriteSheetPath);
                        }
                    }
                    
                    _time = AnimationFrames[_currentSprite].FrameTime;
                }
            }
        }
        base.EditorUpdate(dt);
    }
}