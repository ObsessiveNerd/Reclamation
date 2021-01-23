using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIController : WorldComponent
{
    GameObject m_ActiveUI;
    Transform m_Canvas;

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.UIInput);
        RegisteredEvents.Add(GameEventId.CloseUI);
        RegisteredEvents.Add(GameEventId.OpenInventoryUI);
        m_Canvas = GameObject.Find("Canvas").transform;
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.OpenInventoryUI)
        {
            GameObject ui = Resources.Load<GameObject>("UI/Inventory");
            m_ActiveUI = GameObject.Instantiate(ui, m_Canvas);

            IEntity source = (IEntity)gameEvent.Paramters[EventParameters.Entity];
            List<IEntity> inventory = (List<IEntity>)gameEvent.Paramters[EventParameters.Value];
            m_ActiveUI.GetComponent<InventoryMono>().Setup(source, inventory);
        }

        if (gameEvent.ID == GameEventId.CloseUI)
        {
            if (m_ActiveUI != null)
            {
                GameObject.Destroy(m_ActiveUI);
                m_ActiveUI = null;
            }
        }
    }
}
