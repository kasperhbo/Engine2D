using Engine2D.Core;
using Engine2D.Scenes;
using ImGuiNET;
using OpenTK.Windowing.Common;

namespace Engine2D.UI.Debug;

public class UIDebugStats
{
    public static void OnGui(FrameEventArgs args)
    {
        ImGui.Begin   ("Debug Stats");
        ImGui.Text    ("Scene");
        ImGui.Text    ($"Scene Name:                 {Engine.Get().CurrentScene.ScenePath}"        );
        ImGui.Text    ($"GameObjects:                {Engine.Get().CurrentScene.Entities.Count}");
        
        ImGui.Checkbox("Do update", ref Scene.DoUpdate);
        
        ImGui.Separator();
        ImGui.Text    ($"FPS:                         {1 / args.Time:0.00}"                  );
        ImGui.Text    ($"Frame Time:                  {args.Time * 1000:0.00}ms"             );
        ImGui.Text    ($"Assembly Reloaded:           {DebugStats.AssemblyReloaded}"         );
        
        //ImGui.Checkbox("Render",                 ref Renderer.RenderEverything);
        //ImGui.Checkbox("Debug Render",           ref Renderer.DebugRender);
        
        ImGui.Text     ($"Draw Calls:                  {DebugStats.DrawCalls}"                );
        ImGui.Text     ($"Sprites Drawn:               {DebugStats.SpritesDrawn}"             );
        ImGui.Text     ($"Textures Loaded:             {DebugStats.TexturesLoaded}"           );
        ImGui.Text     ($"Textures Unloaded:           {DebugStats.TexturesUnloaded}"         );
        ImGui.Text     ($"Textures Reloaded:           {DebugStats.TexturesReloaded}"         );
        ImGui.Text     ($"Textures Created:            {DebugStats.TexturesCreated}"          );
        ImGui.Text     ($"Textures Saved:              {DebugStats.TexturesSaved}"            );
        ImGui.Text     ($"Textures Loaded From Memory: {DebugStats.TexturesLoadedFromMemory}" );
        ImGui.Text     ($"Textures Loaded From File:   {DebugStats.TexturesLoadedFromFile}"   );

        ImGui.Text($"Gameobjects: {Engine.Get().CurrentScene.Entities.Count}");

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
          //  $"\n Render Everything:           {Renderer.RenderEverything}" +
          //  $"\n Debug Render:                {Renderer.DebugRender}" +
            $"\n Draw Calls:                  {DebugStats.DrawCalls}" +
            $"\n Sprites Drawn:               {DebugStats.SpritesDrawn}" +
            $"\n Textures Loaded:             {DebugStats.TexturesLoaded}" +
            $"\n Textures Unloaded:           {DebugStats.TexturesUnloaded}" +
            $"\n Textures Reloaded:           {DebugStats.TexturesReloaded}" +
            $"\n Textures Created:            {DebugStats.TexturesCreated}" +
            $"\n Textures Saved:              {DebugStats.TexturesSaved}" +
            $"\n Textures Loaded From Memory: {DebugStats.TexturesLoadedFromMemory}" +
            $"\n Textures Loaded From File:   {DebugStats.TexturesLoadedFromFile}");
        
        return test;
    }

}