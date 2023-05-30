using System.Numerics;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.GameObjects;
using ImGuiNET;
using ImGuizmoNET;
using KDBEngine.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine2D.UI.Viewports;

public class EditorViewport : ViewportWindow
{
    
    OPERATION _currentOperation = OPERATION.TRANSLATE;
    MODE _currentMode = MODE.WORLD;
    
    IntPtr TransformControlLocalIcon = IntPtr.Zero;
    IntPtr TransformControlWorldIcon = IntPtr.Zero;
    
    IntPtr PlayBtn = IntPtr.Zero;
    IntPtr SimulateBtn = IntPtr.Zero;
    IntPtr StopBtn = IntPtr.Zero;

    private System.Numerics.Vector2 pos = new();

    public EditorViewport(string editorVp) : base(editorVp)
    {
        TextureData data = new TextureData();
        data.flipped = false;
        data.magFilter = TextureMagFilter.Linear;
        data.minFilter = TextureMinFilter.Linear;
        data.texturePath = string.Format(Utils.GetBaseEngineDir() + "\\images\\Icons\\Play.png");
        
        TextureData TransformLocalIconData = TextureData.CopyToNew(data,
            Utils.GetBaseEngineDir() + "\\images\\Icons\\Play.png");
        TransformControlLocalIcon = (IntPtr)ResourceManager.GetTexture(TransformLocalIconData).TexID;
        
        TextureData TransformLWorldIconData = TextureData.CopyToNew(data,
            Utils.GetBaseEngineDir() + "\\images\\Icons\\Play.png");
        TransformControlWorldIcon = (IntPtr)ResourceManager.GetTexture(TransformLWorldIconData).TexID;
        
        PlayBtn = (IntPtr)ResourceManager.GetTexture(data).TexID;
        SimulateBtn = (IntPtr)ResourceManager.GetTexture(data).TexID;
        StopBtn = (IntPtr)ResourceManager.GetTexture(data).TexID;
    }

    protected override void BeforeImageRender()
    {
        base.BeforeImageRender();
        ImGui.Button("test");
        if (_cameraToRender != null)
        {
            if (_cameraToRender.ProjectionSize.X != _windowSize.X || _cameraToRender.ProjectionSize.Y != _windowSize.Y)
                _cameraToRender.adjustProjection((_windowSize.X, _windowSize.Y));
        }
        

    }

    protected override void AfterImageRender(bool recieveInput = true)
    {
        base.AfterImageRender();
        ImGui.SetCursorPos(new(cursorPos.X, cursorPos.Y + 5));
        
        Vector2 size = new(_windowSize.X, _windowSize.Y);
        ImGui.PushStyleColor(ImGuiCol.ChildBg, new System.Numerics.Vector4(0, 0, 0, 0));
        
        float s = _windowSize.X / 32;
        
        if (ImGui.BeginChild("bar", new System.Numerics.Vector2(size.X, s+30), true, 
                ImGuiWindowFlags.NoScrollbar))
        {
            int count = 3;
            float div = (count * s) + 12;
            
            ImGui.SetCursorPos(new Vector2(0,0));
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(1,0));
            
            if (ImGui.BeginChild("inner_transform_components",
                    new System.Numerics.Vector2(s * 3 + 40, s + 30), true, ImGuiWindowFlags.NoScrollbar))
            {
                ImGui.ImageButton(PlayBtn, new System.Numerics.Vector2(s-2,s));
                ImGui.SameLine();
                
                ImGui.ImageButton(SimulateBtn, new System.Numerics.Vector2(s-2,s));
                ImGui.SameLine();
                
                ImGui.ImageButton(StopBtn, new System.Numerics.Vector2(s-2,s));
                ImGui.EndChild();
            }
            
            ImGui.SetCursorPos(new((size.X / 2) - div/3, 0));
            
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new System.Numerics.Vector4(0, 0, 0, 0));
            ImGui.PushStyleColor(ImGuiCol.Button, new System.Numerics.Vector4(.2f,.2f,.2f,1));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new System.Numerics.Vector4(.8f,.8f,.8f,1));

            if(ImGui.BeginChild("inner",
                   new System.Numerics.Vector2(s*3 + 40, s+30), true, ImGuiWindowFlags.NoScrollbar))
            {
                ImGui.ImageButton(PlayBtn, new System.Numerics.Vector2(s-2,s));
                ImGui.SameLine();
                ImGui.ImageButton(SimulateBtn, new System.Numerics.Vector2(s-2,s));
                ImGui.SameLine();
                ImGui.ImageButton(StopBtn, new System.Numerics.Vector2(s-2,s));
                ImGui.EndChild();
            }
            ImGui.PopStyleColor();
            ImGui.PopStyleVar();
            
            ImGui.EndChild();
        }
        ImGui.PopStyleColor();
        
        
        
        //Draw ImGuizmo
        Gameobject go = (Gameobject)Engine.Get().CurrentSelectedAsset;
        if (go != null)
        {
           
            ImGuizmo.Enable(true);
            ImGuizmo.SetOrthographic(true);
            ImGuizmo.SetDrawlist();

            
            pos = ImGui.GetCursorStartPos();
            
            ImGuizmo.SetRect(origin.X, origin.Y, sz.X, sz.Y);
            
            OpenTK.Mathematics.Matrix4 view = _cameraToRender.getViewMatrix();
            OpenTK.Mathematics.Matrix4 projection = _cameraToRender.getProjectionMatrix();
            Matrix4x4 translation = go.Transform.GetTranslation();
            
            ImGuizmo.Manipulate(ref view.Row0.X, ref projection.Row0.X, 
                _currentOperation, _currentMode, ref translation.M11);
            
            if(recieveInput)
            {
                if (Engine.Get().IsKeyPressed(Keys.W))
                {
                    _currentOperation = OPERATION.TRANSLATE;
                }

                if (Engine.Get().IsKeyPressed(Keys.E))
                {
                    _currentOperation = OPERATION.SCALE;
                }

                if (Engine.Get().IsKeyPressed(Keys.R))
                {
                    _currentOperation = OPERATION.ROTATE;
                }

                if (ImGuizmo.IsUsing())
                {
                    var outScale = new System.Numerics.Vector3();
                    var outPos = new System.Numerics.Vector3();
                    Quaternion outQuaternion = new();

                    Matrix4x4.Decompose(translation, out outScale, out outQuaternion, out outPos);

                    EulerDegrees deg = new EulerDegrees(outQuaternion);

                    if(_currentOperation == OPERATION.TRANSLATE)
                        go.Transform.Position = new(outPos.X, outPos.Y);
                    if(_currentOperation == OPERATION.ROTATE)
                        go.Transform.Rotation.SetRotation(deg);
                    if(_currentOperation == OPERATION.SCALE)
                        go.Transform.Size = new(outScale.X, outScale.Y);
                }
            }
        }
    }
}