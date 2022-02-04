using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicContainer : EntityComponent
{
    private Sprite m_Sprite;
    public string SpritePath;

    public override int Priority => 1;

    public GraphicContainer(string spritePath)
    {
        SpritePath = spritePath;
        Sprite sprite = Resources.Load<Sprite>(SpritePath);
        m_Sprite = sprite;
        RegisteredEvents.Add(GameEventId.GetSprite);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(m_Sprite == null)
            Debug.Log("Sprite null");
        gameEvent.Paramters[EventParameters.RenderSprite] = m_Sprite;
    }
}

public class DTO_GraphicContainer : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        if(data.Contains("="))
            Component = new GraphicContainer(data.Split('=')[1]);
        else
            Component = new GraphicContainer(data);
    }

    public string CreateSerializableData(IComponent component)
    {
        GraphicContainer gc = (GraphicContainer)component;
        return $"{nameof(GraphicContainer)}:{nameof(gc.SpritePath)}={gc.SpritePath}";
    }
}