using System.Numerics;
using Engine2D.Core;
using Engine2D.Rendering.NewRenderer;
using Engine2D.Scenes;
using ImGuiNET;
using OpenTK.Windowing.Common;

namespace Engine2D.UI.Debug;

public class UIDebugStats
{
    public static void OnGui(FrameEventArgs args)
    {
        var currentCol = ImGui.GetStyle().Colors[(int) ImGuiCol.ChildBg];
        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.1f,0.1f,0.1f, 1.0f));
        ImGui.Begin   ("Debug Stats");
        
        ImGui.Text(debug_data.GetDebugData(args.Time));
        
        ImGui.Separator();
        
        ImGui.Text("Renderer");
        ImGui.Text($"Batches: {Renderer.Batches.Count}");
        ImGui.Text($"Clear Color: {Renderer.ClearColor}");
        ImGui.Text($"Game Frame Buffer: {Renderer.GameFrameBuffer}");
        ImGui.Text($"Editor Frame Buffer: {Renderer.EditorFrameBuffer}");
        for (int i = 0; i < Renderer.Batches.Count; i++)
        {
            if (ImGui.Button("Open / Close DebuggerFor: " + (i + 1)))
            {
                Renderer.Batches[i].DebuggerOpened = !Renderer.Batches[i].DebuggerOpened;
            }
        }
        
        ImGui.Separator();
        ImGui.Text("UI");
        ImGui.Text($"UI Elements: {UiRenderer._windows.Count}");

        if (ImGui.BeginTable("UIWindows", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable))
        {
            for (int i = 0; i < UiRenderer._windows.Count; i++)
            {
                var window = UiRenderer._windows[i];
                ImGui.PushID(window.Title);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text($"{window.Title}: {window.IsVisible}");
                ImGui.TableSetColumnIndex(1);
                ImGui.Checkbox("Is Visible", ref window.IsVisible);
                ImGui.PopID();
            }

            ImGui.EndTable();
        }

        ImGui.End();
        
        for (int i = 0; i < Renderer.Batches.Count; i++)
        {
            var batch = Renderer.Batches[i];
               if (!batch.DebuggerOpened) continue;
               
            ImGui.Begin("Batch: " + (i + 1));
            ImGui.Text("Batch: " + (i + 1));
            ImGui.Separator();
            ImGui.Text($"Z-index: {batch.ZIndex}");
            
            ImGui.Separator();
            ImGui.Text("Shader");
            ImGui.Text($"Shader ID: {batch.Shader.ShaderProgramId}");
            ImGui.Text($"VertexSource: " + batch.Shader.VertexSource);
            ImGui.Text($"FragmentSource: " + batch.Shader.FragmentSource);
            ImGui.Separator();
            
            ImGui.Text("Textures");
            for (int j = 0; j < batch.TextureIDS.Length; j++)
            {
                var texID = batch.TextureIDS[j];
                if(texID != -1)
                {
                    ImGui.Text($"Texture ID: {texID} at index {j}");
                    ImGui.ImageButton(texID, new System.Numerics.Vector2(64, 64));
                }
                
            }
            ImGui.Separator();
            ImGui.End();
        }
        
        ImGui.PopStyleColor();
    }
}

public static class debug_data
{
    public static string GetDebugData(double time)
    {
        string test = (
            "Scene" +
            $"\n Scene Name:                 {Engine.Get().CurrentScene.ScenePath}" +
            $"\n Do update:                  {Scene.DoUpdate}" +
            $"\n Gameobject {Engine.Get().CurrentScene.Entities.Count}" +

            "\n Render Stats" +
            $"\n FPS:                         {1 / time:0.00}" +
            $"\n Frame Time:                  {time * 1000:0.00}ms" +
            $"\n Assembly Reloaded:           {DebugStats.AssemblyReloaded}" +
            $"\n Draw Calls:                  {DebugStats.DrawCalls}" +

            //Textures
            "\n Texture Stats" +
            $"\n Textures Loaded:             {DebugStats.TexturesLoadedByResourceManager}" +
            $"\n Textures Unloaded:           {DebugStats.TexturesUnloaded}" +
            $"\n Textures Reloaded:           {DebugStats.TexturesReloaded}" +
            $"\n Textures Created:            {DebugStats.TexturesCreated}" +
            $"\n Textures Saved:              {DebugStats.TexturesSaved}" +
            $"\n Textures Loaded From Memory: {DebugStats.TexturesLoadedFromMemory}" +
            $"\n Textures Loaded From File:   {DebugStats.TexturesLoadedFromFile}" +

            //Sprites
            "\n Sprite Stats" +
            $"\n Sprites Drawn:                       {DebugStats.SpritesDrawn}" +
            $"\n Sprites Loaded By ResourceManager:   {DebugStats.SpritesLoadedByResourceManager}" +

            //Shaders
            "\n Shader Stats" +
            $"\n Loaded Shaders:                      {DebugStats.LoadedShaders}");



        return test;
    }

}