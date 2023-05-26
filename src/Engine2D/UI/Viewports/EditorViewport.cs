using System.Numerics;
using Engine2D.Core;
using Engine2D.GameObjects;
using ImGuiNET;
using ImGuizmoNET;
using KDBEngine.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector2 = System.Numerics.Vector2;

namespace Engine2D.UI.Viewports;

public class EditorViewport : ViewportWindow
{
    
    OPERATION _currentOperation = OPERATION.TRANSLATE;
    MODE _currentMode = MODE.WORLD;
    
    IntPtr PlayBtn = IntPtr.Zero;
    IntPtr SimulateBtn = IntPtr.Zero;
    IntPtr StopBtn = IntPtr.Zero;

    private Vector2 pos = new();

    public EditorViewport(string editorVp) : base(editorVp)
    {
        TextureData data = new TextureData();
        data.flipped = false;
        data.magFilter = TextureMagFilter.Linear;
        data.minFilter = TextureMinFilter.Linear;
        data.texturePath = string.Format(Utils.GetBaseEngineDir() + "\\images\\Icons\\Play.png");
        
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
            if (_cameraToRender.projectionSize != (_windowSize.X, _windowSize.Y))
                _cameraToRender.adjustProjection((_windowSize.X, _windowSize.Y));
        }
        

    }

    protected override void AfterImageRender(bool recieveInput = true)
    {
        base.AfterImageRender();
        ImGui.SetCursorPos(new(cursorPos.X + 32, cursorPos.Y + 5));
        //ImGui.SetNextItemWidth(_windowSize.X);
        Vector2 size = new(_windowSize.X - 60, _windowSize.Y);
        ImGui.PushStyleColor(ImGuiCol.ChildBg, new System.Numerics.Vector4(0, 0, 0, 0));
        float s = _windowSize.X / 32;
        if (ImGui.BeginChild("bar", new Vector2(size.X, s+30), false, ImGuiWindowFlags.NoScrollbar))
        {
            int count = 3;
            float div = (count * s) + 12;
            
            ImGui.SetCursorPos(new((size.X / 2) - div/3, 0));
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(1,0));
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new System.Numerics.Vector4(0, 0, 0, 0));
            if(ImGui.BeginChild("inner", new Vector2(s*3 + 40, s+30), false, ImGuiWindowFlags.NoScrollbar))
            {
                //ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 3);
                ImGui.ImageButton(PlayBtn, new Vector2(s-2,s));
                ImGui.SameLine();
                ImGui.ImageButton(SimulateBtn, new Vector2(s-2,s));
                ImGui.SameLine();
                ImGui.ImageButton(StopBtn, new Vector2(s-2,s));
                ImGui.EndChild();
            }
            ImGui.PopStyleColor();
            ImGui.PopStyleVar();
            
        }
        ImGui.PopStyleColor();
        
        //Draw ImGuizmo
        Gameobject go = (Gameobject)Engine.Get().CurrentSelectedAsset;
        if (go != null)
        {
           
            ImGuizmo.Enable(true);
            ImGuizmo.SetOrthographic(true);
            ImGuizmo.SetDrawlist();

            origin = ImGui.GetItemRectMin() ;
            sz =  ImGui.GetItemRectSize();
            pos = ImGui.GetCursorStartPos();
            ImGuizmo.SetRect(origin.X, origin.Y, sz.X, sz.Y);
            
            Matrix4 view = _cameraToRender.getViewMatrix();
            Matrix4 projection = _cameraToRender.getProjectionMatrix();
            Matrix4 translation = go.Transform.GetTranslation();
            
            ImGuizmo.Manipulate(ref view.Row0.X, ref projection.Row0.X, 
                _currentOperation, _currentMode, ref translation.Row0.X);
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
                    go.Transform.position = new(translation.ExtractTranslation().X,
                        translation.ExtractTranslation().Y);
                    
                    go.Transform.size = new(translation.ExtractScale().X, translation.ExtractScale().Y);
                    go.Transform.SetRotation(translation.ExtractRotation());
                }
            }
        }
    }
}