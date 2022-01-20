using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Actor : IEntity
{
    PriorityQueue<IComponent> m_Components;
    List<IComponent> m_AddQueue = new List<IComponent>();
    List<IComponent> m_RemoveQueue = new List<IComponent>();

    public string InternalName { get; internal set; }
    public string Name {
        get
        {
            GameEvent getName = GameEventPool.Get(GameEventId.GetName)
                                    .With(EventParameters.Name, InternalName);
            var ret = FireEvent(getName).GetValue<string>(EventParameters.Name);
            getName.Release();
            return ret;
        }
        internal set
        {
            InternalName = value;
        }
    }
    public string ID { get; internal set; }
    public bool NeedsCleanup
    {
        get
        {
            return m_AddQueue.Count > 0 || m_RemoveQueue.Count > 0;
        }
    }

    //public Action<IEntity> Destroyed
    //{
    //    get;
    //    set;
    //}

    public Actor(string name)
    {
        Name = name;
        ID = IDManager.GetNewID().ToString();
        m_Components = new PriorityQueue<IComponent>(new ComponentComparer());

        if (World.Instance != null)
            FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.RegisterEntity)
                .With(EventParameters.Entity, this)).Release();

        EntityMap.AddEntity(this);
        //EntityMap.IDToNameMap[ID] = Name;
    }

    public Actor(string name, string id)
    {
        Name = name;
        ID = string.IsNullOrEmpty(id) ? IDManager.GetNewID().ToString() : id;
        if(!string.IsNullOrEmpty(id))
            IDManager.SetId(int.Parse(id));

        m_Components = new PriorityQueue<IComponent>(new ComponentComparer());
        if (World.Instance != null)
            FireEvent(World.Instance.Self, GameEventPool.Get(GameEventId.RegisterEntity)
                .With(EventParameters.Entity, this)).Release();

        EntityMap.AddEntity(this);
        //EntityMap.IDToNameMap[ID] = Name;
    }

    //public virtual void OnDestroy(IEntity entity)
    //{

    //}

    public void Start()
    {
        foreach (var c in m_Components.ToList())
            c.Start();
    }

    public GameEvent FireEvent(IEntity target, GameEvent gameEvent, bool logEvent = false)
    {
        if (logEvent && target != null)
            SaveSystem.LogEvent(target.ID, gameEvent);

        target?.HandleEvent(gameEvent);
        return gameEvent;
    }

    public GameEvent FireEvent(GameEvent gameEvent, bool logEvent = false)
    {
        if (logEvent)
            SaveSystem.LogEvent(ID, gameEvent);

        HandleEvent(gameEvent);
        return gameEvent;
    }

    public void HandleEvent(GameEvent gameEvent)
    {
        var components = new List<IComponent>(m_Components.Values);
        foreach (IComponent component in components)
        {
            if (component.RespondsTo(gameEvent))
                component.HandleEvent(gameEvent);

            if (!gameEvent.ContinueProcessing)
                break;
        }
    }

    public void AddComponentRange(List<IComponent> components)
    {
        foreach (var c in components)
            AddComponent(c);
    }

    public void AddComponent(IComponent component)
    {
        m_AddQueue.Add(component);
        component.Init(this);
    }

    public List<IComponent> GetComponents()
    {
        return m_Components.ToList();
    }

    public T GetComponent<T>() where T : IComponent
    {
        var value = m_Components.Values.FirstOrDefault(c => c.GetType() == typeof(T));
        return (T)value;
    }

    public void RemoveComponent(IComponent component)
    {
        m_RemoveQueue.Add(component);
        if (m_AddQueue.Contains(component))
            m_AddQueue.Remove(component);
    }

    public void RemoveComponent(Type component)
    {
        IComponent comp = m_Components.Values.Find(c => component.IsAssignableFrom(c.GetType()));
        if (comp != null)
            m_RemoveQueue.Add(comp);
        else
        {
            comp = m_AddQueue.Find(c => component.IsAssignableFrom(c.GetType()));
            if(comp != null)
                m_AddQueue.Remove(comp);
        }
    }

    public bool HasComponent(Type component)
    {
        return m_Components.Values.Any(c => component.IsAssignableFrom(c.GetType()));
    }

    public void CleanupComponents()
    {
        foreach (IComponent component in m_RemoveQueue)
            m_Components.Remove(component);
        m_Components.AddRange(m_AddQueue);
        m_RemoveQueue.Clear();
        m_AddQueue.Clear();
    }

    public string Serialize()
    {
        CleanupComponents();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"<{Name},{ID}>(");

        foreach(IComponent comp in m_Components.ToList())
        {
            string dtoName = $"DTO_{comp.GetType()}";
            Type t = Type.GetType(dtoName);
            if(t == null)
            {
                RecLog.Log($"Can't find type: {dtoName}");
                continue;
            }
            IDataTransferComponent dtc = (IDataTransferComponent)Activator.CreateInstance(t);
            sb.AppendLine($"\t{dtc.CreateSerializableData(comp)}");
        }

        sb.AppendLine(")");
        return sb.ToString();
    }
}
