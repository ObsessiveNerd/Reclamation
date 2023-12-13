using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageGraphic : EntityComponent
{
    public string HasItemsGraphicPath;
    public string EmptyGraphicPath;

    Sprite HasItemsSprite
    {
        get
        {
            return Resources.Load<Sprite>(HasItemsGraphicPath);
        }
    }

    Sprite EmptySprite
    {
        get
        {
            return Resources.Load<Sprite>(EmptyGraphicPath);
        }
    }

    public StorageGraphic(string hasItems, string empty)
    {
        HasItemsGraphicPath = hasItems;
        EmptyGraphicPath = empty;
    }

    public void Start()
    {
        RegisteredEvents.Add(GameEventId.GetSprite);
        
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetSprite)
        {
            GameEvent builder = GameEventPool.Get(GameEventId.GetItems)
                                    .With(EventParameter.Items, new List<string>());

            var result = FireEvent(Self, builder);

            if(result.GetValue<List<string>>(EventParameter.Items).Count == 0)
                gameEvent.Paramters[EventParameter.RenderSprite] = EmptySprite;
            else
                gameEvent.Paramters[EventParameter.RenderSprite] = HasItemsSprite;

            result.Release();
        }
    }
}

public class DTO_StorageGraphic : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        string hasItemsGraphic = "";
        string emptyItemsGraphic = "";

        string[] kvp = data.Split(',');
        foreach(var pair in kvp)
        {
            string key = pair.Split('=')[0];
            string value = pair.Split('=')[1];

            if (key == "HasItemsGraphicPath")
                hasItemsGraphic = value;
            else if (key == "EmptyGraphicPath")
                emptyItemsGraphic = value;
        }

        Component = new StorageGraphic(hasItemsGraphic, emptyItemsGraphic);
    }

    public string CreateSerializableData(IComponent component)
    {
        StorageGraphic sg = (StorageGraphic)component;
        return $"{nameof(StorageGraphic)}: {nameof(sg.HasItemsGraphicPath)}={sg.HasItemsGraphicPath},{nameof(sg.EmptyGraphicPath)}={sg.EmptyGraphicPath}";
    }
}
