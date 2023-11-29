using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIcon : EntityComponent
{
    public string IconPath;

    public UIIcon(string iconPath)
    {
        IconPath = iconPath;
    }

    public override void Init(IEntity self)
    {
        RegisteredEvents.Add(GameEventId.GetIcon);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.GetIcon)
            gameEvent.Paramters[EventParameter.RenderSprite] = Resources.Load<Sprite>(IconPath);
    }
}

public class DTO_UIIcon : IDataTransferComponent
{
    public IComponent Component { get; set; }

    public void CreateComponent(string data)
    {
        Component = new UIIcon(data);
    }

    public string CreateSerializableData(IComponent component)
    {
        string iconPath = ((UIIcon)component).IconPath;
        return $"{nameof(UIIcon)}={iconPath}";
    }
}
