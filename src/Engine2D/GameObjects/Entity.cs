using System;

public class Entity
{
    private int m_EntityHandle = 0;

    public Entity() { }

    public Entity(int handle)
    {
        m_EntityHandle = handle;
    }

    public T AddComponent<T>(params object[] args)
    {
        if (HasComponent<T>())
        {
            throw 
                new Exception
                    ("Entity already has component!");
        }
        return m_Scene.Registry.Emplace<T>(m_EntityHandle, args);
    }

    public T GetComponent<T>()
    {
        if (!HasComponent<T>())
        {
            throw new Exception
                ("Entity does not have component!");
        }
        return m_Scene.Registry.Get<T>(m_EntityHandle);
    }

    public bool HasComponent<T>()
    {
        return m_Scene.Registry.Has<T>(m_EntityHandle);
    }

    public void RemoveComponent<T>()
    {
        if (!HasComponent<T>())
        {
            throw new Exception("Entity does not have component!");
        }
        m_Scene.Registry.Remove<T>(m_EntityHandle);
    }

    public static implicit operator bool(Entity entity)
    {
        return entity.m_EntityHandle != 0;
    }
}
