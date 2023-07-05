using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Engine2D.Components.Sprites;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.SavingLoading;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Graphics.ES30;
using OpenTK.Windowing.Common;
using TiledSharp;

namespace Engine2D.Components.Tiled;

public class TileMapLoadComponent : Component
{
    [JsonProperty] private string _tileMapPath = "";
    [JsonProperty] private List<ObjectGroup> _objectGroups = new();

    [JsonIgnore] TmxMap? _map = null;
    [JsonIgnore] private string _version = "";


    public override void StartPlay()
    {
    }

    public override void Init()
    {
        InitTileMap();
    }

    public override void Update(FrameEventArgs args)
    {
    }

    private void InitTileMap()
    {
        if (File.Exists(ProjectSettings.FullProjectPath + _tileMapPath))
        {
            Log.Message("Loading map: " + _tileMapPath);
            _map = new TmxMap(ProjectSettings.FullProjectPath + _tileMapPath);
            _version = _map.Version;
            Log.Succes("Succesfully loaded map: " + _tileMapPath + " version: " + _version);
            
        }
    }

    public override unsafe void ImGuiFields()
    {
        base.ImGuiFields();

        ImGui.Button("Tilemap");
        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGui.AcceptDragDropPayload("tilemap_drop");
            if (payload.IsValidPayload())
            {
                var filename = (string)GCHandle.FromIntPtr(payload.Data).Target;
                _tileMapPath = filename;
                InitTileMap();
            }

            ImGui.EndDragDropTarget();
        }

        ImGui.Text("Objects");
        ImGui.SameLine();
        if (ImGui.Button("Fill object list"))
        {
            _objectGroups = new();

            if (_map == null) return;
            
            var objectGroupsTMX = _map.ObjectGroups;

            for (int i = 0; i < objectGroupsTMX.Count; i++)
            {
                var tmxObjectGroup = objectGroupsTMX[i];
                List<Spawnable> spawnables = new();

                for (int j = 0; j < tmxObjectGroup.Objects.Count; j++)
                {
                    var obj = tmxObjectGroup.Objects[j];
                    float x = (float)obj.X;
                    float y = (float)obj.Y;
                    float width = (float)obj.Width;
                    float height = (float)obj.Height;
                    string name = obj.Name;
                    int id = obj.Id;

                    var spawnable = new Spawnable(name, id, tmxObjectGroup.Name, new(((x + width / 2)),
                        ((y + height / 2)*-1)), new(width, height));
                    spawnables.Add(spawnable);
                }

                ObjectGroup group = new ObjectGroup(tmxObjectGroup.Name, spawnables);
                _objectGroups.Add(group);
            }

        }

        for (int i = 0; i < _objectGroups.Count; i++)
        {
            var group = _objectGroups[i];
            ImGui.SetNextItemWidth(ImGui.GetContentRegionMax().X - 100);
            ImGui.Dummy(new(0, 30));
            ImGui.PushID(group.Name);
            if (ImGui.BeginListBox(group.Name))
            {
                
                ImGui.BeginTable("table1", 7, ImGuiTableFlags.Resizable | ImGuiTableFlags.Borders);
                for (int n = 0; n < group.Objects.Count; n++)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text(group.Objects[n].ObjectGroupName);
                    ImGui.TableSetColumnIndex(1);
                    ImGui.Text(group.Objects[n].Name);
                    ImGui.TableSetColumnIndex(2);
                    if (group.Objects[n].GameObjectPath != "")
                    {
                        ImGui.Button(group.Objects[n].GameObjectPath);
                    }
                    else
                    {
                        ImGui.Button("Gameobject");
                    }

                    if (ImGui.BeginDragDropTarget())
                    {
                        var payload = ImGui.AcceptDragDropPayload("prefab_drop");
                        if (payload.IsValidPayload())
                        {
                            var filename = (string)GCHandle.FromIntPtr(payload.Data).Target;
                            group.Objects[n].GameObjectPath = filename;
                        }

                        ImGui.EndDragDropTarget();
                    }

                    ImGui.TableSetColumnIndex(3);
                    ImGui.InputInt("##id", ref group.Objects[n].Id);

                    ImGui.TableSetColumnIndex(4);
                    ImGui.DragFloat2("##position", ref group.Objects[n].Position);

                    ImGui.TableSetColumnIndex(5);
                    ImGui.DragFloat2("##size", ref group.Objects[n].Size);

                    ImGui.TableSetColumnIndex(6);
                    if (ImGui.Button("Spawn"))
                    {
                        group.Objects[n].Spawn();
                    }
                }
                ImGui.EndTable();
                ImGui.EndListBox();
            }
            
            //Spawn all at once
            if(ImGui.Button("Spawn All"))
            {
                Console.WriteLine("Spawn all");
                foreach (var groupObject in group.Objects)
                {
                    groupObject.Spawn();
                }
            }
            ImGui.Button("Gameobject");
            if (ImGui.BeginDragDropTarget())
            {
                var payload = ImGui.AcceptDragDropPayload("prefab_drop");
                if (payload.IsValidPayload())
                {
                    var filename = (string)GCHandle.FromIntPtr(payload.Data).Target;
                    foreach (var groupObject in group.Objects)
                    {
                        groupObject.GameObjectPath = filename;
                    }
                }

                ImGui.EndDragDropTarget();
            }
            
            ImGui.PopID();
        }
    }
}

internal class ObjectGroup
{
    public string Name = "";
    public List<Spawnable> Objects = new();

    public ObjectGroup(string name, List<Spawnable> objects)
    {
        Name = name;
        Objects = objects;
    }
}

internal class Spawnable
{
    public string GameObjectPath = "";
    public int Id;
    public string ObjectGroupName;
    public Vector2 Position;
    public Vector2 Size;
    public string Name = "";
    
    public Spawnable(string name, int id, string objectGroupName, Vector2 position, Vector2 size)
    {
        Name = name;
        Id = id;
        ObjectGroupName = objectGroupName;
        Position = position;
        Size = size;
    }

    public void Spawn()
    {
        bool end = GameObjectPath == "";

        if (SaveLoad.LoadGameobject(GameObjectPath) == null)
        {
            Log.Error("Gameobject path not found");
            end = true;
        }

        if (!end)
        {
            Gameobject? go = SaveLoad.LoadGameobject(GameObjectPath);
            
            if (go == null)
            {
                Log.Error("Gameobject is null");
                return;
            }

            Engine.Get().CurrentScene.AddGameObjectToScene(go);
            
            go.Transform.Position = Position;
            go.Transform.Size = Size;
            go.Name = ObjectGroupName + " : " + Id;
        }
    }
}
