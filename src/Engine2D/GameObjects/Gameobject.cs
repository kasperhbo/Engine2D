using Engine2D.Components;
using ImGuiNET;

namespace Engine2D.GameObjects
{
    internal class Gameobject
    {
        public Transform transform = new();
        public string Name = "";
        public List<Component> components = new();

        internal void GameObject(string name, List<Component> components, Transform transform)
        {
            Console.WriteLine("build");
            this.Name = name;            
            this.transform = transform;
            this.components = components;
        }

        internal void Init()
        {
            foreach (var component in components) { component.Init(this); }
        }

        internal void Start()
        {            
            foreach (var component in components) { component.Start(); }
        }

        internal void Update(double dt)
        {
            foreach (var component in components) { component.Update(dt); }
        }

        internal void Destroy()
        {
            foreach (var component in components) { component.Destroy(); }
        }

        internal void AddComponent(Component component)
        {
            components.Add(component);
            component.Init(this);
        }

        internal void ImGuiFields()
        {
            transform.ImGuiFields();
            foreach (var component in components) { component.ImGuiFields(); }
        }
    }
}
