using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIController : WorldComponent
{
    public List<IUpdatableUI> UpdatableUI = new List<IUpdatableUI>();

    public override void Init(IEntity self)
    {
        base.Init(self);
        RegisteredEvents.Add(GameEventId.UIInput);
        RegisteredEvents.Add(GameEventId.CloseUI);
        RegisteredEvents.Add(GameEventId.OpenInventoryUI);
        RegisteredEvents.Add(GameEventId.OpenSpellUI);
        RegisteredEvents.Add(GameEventId.UpdateUI);
        RegisteredEvents.Add(GameEventId.RegisterPlayableCharacter);
        RegisteredEvents.Add(GameEventId.OpenChestUI);
        RegisteredEvents.Add(GameEventId.RegisterUI);
        RegisteredEvents.Add(GameEventId.UnRegisterUI);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.OpenInventoryUI)
        {
            IEntity source = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            //List<IEntity> inventory = (List<IEntity>)gameEvent.Paramters[EventParameters.Value];
            GameObject.FindObjectOfType<CharacterManagerMono>().Setup(source);
        }

        else if(gameEvent.ID == GameEventId.OpenSpellUI)
        {
            IEntity source = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            GameObject.FindObjectOfType<SpellSelectorMono>().Setup(source, (List<string>)gameEvent.Paramters[EventParameters.SpellList]);
        }

        else if(gameEvent.ID == GameEventId.RegisterPlayableCharacter)
        {
            string id = gameEvent.GetValue<string>(EventParameters.Entity);
            GameObject.FindObjectOfType<PlayableCharacterSelector>().AddCharacterTab(id);
        }

        else if(gameEvent.ID == GameEventId.UpdateUI)
        {

            foreach (var ui in UpdatableUI)
                ui.UpdateUI();

            //if (gameEvent.Paramters.ContainsKey(EventParameters.Entity))
            //{
            //    string id = gameEvent.GetValue<string>(EventParameters.Entity);
            //    GameObject.FindObjectOfType<CharacterManagerMono>().UpdateUI(id);
            //}
            //else
            //    GameObject.FindObjectOfType<CharacterManagerMono>().UpdateUI(m_ActivePlayer.Value.ID);

            //GameObject.FindObjectOfType<ChestMono>().UpdateUI();
        }

        else if(gameEvent.ID == GameEventId.OpenChestUI)
        {
            IEntity source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
            IEntity character = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Character));
            GameObject.FindObjectOfType<ChestMono>().Init(source, character);
        }

        else if(gameEvent.ID == GameEventId.RegisterUI)
        {
            IUpdatableUI go = gameEvent.GetValue<IUpdatableUI>(EventParameters.GameObject);
            if (!UpdatableUI.Contains(go))
                UpdatableUI.Add(go);
        }

        else if(gameEvent.ID == GameEventId.UnRegisterUI)
        {
            IUpdatableUI go = gameEvent.GetValue<IUpdatableUI>(EventParameters.GameObject);
            if (UpdatableUI.Contains(go))
                UpdatableUI.Remove(go);
        }
    }
}
