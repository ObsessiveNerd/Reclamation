using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldUIController : WorldComponent
{
    public List<IUpdatableUI> UpdatableUI = new List<IUpdatableUI>();

    public override int Priority => 10;

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
        RegisteredEvents.Add(GameEventId.PromptToGiveItem);
        RegisteredEvents.Add(GameEventId.EntityTookDamage);
        RegisteredEvents.Add(GameEventId.EntityHealedDamage);
        RegisteredEvents.Add(GameEventId.OpenSpellExaminationUI);
    }

    public override void HandleEvent(GameEvent gameEvent)
    {
        if(gameEvent.ID == GameEventId.OpenInventoryUI)
        {
            IEntity source = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            List<IEntity> inventory = (List<IEntity>)gameEvent.Paramters[EventParameters.Value];

            GameObject.FindObjectOfType<CharacterManagerMono>().Setup(source);

            foreach (var entity in m_Players)
                GameObject.FindObjectOfType<CharacterManagerMono>().AddCharacter(entity);

            //GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CharacterManager"));
        }

        else if(gameEvent.ID == GameEventId.OpenSpellUI)
        {
            IEntity source = EntityQuery.GetEntity((string)gameEvent.Paramters[EventParameters.Entity]);
            GameObject.FindObjectOfType<SpellSelectorMono>().Setup(source);
        }

        else if(gameEvent.ID == GameEventId.RegisterPlayableCharacter)
        {
            string id = gameEvent.GetValue<string>(EventParameters.Entity);
            GameObject.FindObjectOfType<PlayableCharacterSelector>().AddCharacterTab(id);
        }

        else if(gameEvent.ID == GameEventId.UpdateUI)
        {
            string newId = gameEvent.GetValue<string>(EventParameters.Entity);
            foreach (var ui in UpdatableUI)
                ui?.UpdateUI(EntityQuery.GetEntity(newId));

            GameObject.FindObjectOfType<SpellSelectorMono>().Setup(m_ActivePlayer.Value);

            //if (gameEvent.Paramters.ContainsKey(EventParameters.Entity))
            //{
            //    string id = gameEvent.GetValue<string>(EventParameters.Entity);
            //    GameObject.FindObjectOfType<CharacterManagerMono>().UpdateUI(id);
            //}
            //else
            //    GameObject.FindObjectOfType<CharacterManagerMono>().UpdateUI(m_ActivePlayer.Value.ID);

            //GameObject.FindObjectOfType<ChestMono>().UpdateUI();
        }

        else if(gameEvent.ID == GameEventId.OpenSpellExaminationUI)
        {
            GameObject.FindObjectOfType<SpellExaminationUI>().Setup(gameEvent.GetValue<List<string>>(EventParameters.SpellList));
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

        else if(gameEvent.ID == GameEventId.PromptToGiveItem)
        {
            IEntity source = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Entity));
            IEntity item = EntityQuery.GetEntity(gameEvent.GetValue<string>(EventParameters.Item));

            ContextMenuMono.CreateNewContextMenu().GetComponent<ContextMenuMono>().SelectPlayer((target) =>
            {
                GameEvent unEquip = GameEventPool.Get(GameEventId.Unequip)
                                            .With(EventParameters.Entity, source.ID)
                                            .With(EventParameters.Item, item.ID);
                source.FireEvent(unEquip).Release();

                GameEvent removeFromInventory = GameEventPool.Get(GameEventId.RemoveFromInventory)
                                                    .With(EventParameters.Entity, item.ID);
                source.FireEvent(removeFromInventory).Release();

                GameEvent addToInventory = GameEventPool.Get(GameEventId.AddToInventory)
                                                .With(EventParameters.Entity, item.ID);
                EntityQuery.GetEntity(target).FireEvent(addToInventory).Release();

                GameObject.FindObjectOfType<CharacterManagerMono>().UpdateUI(source);

            }, m_Players.Select(player => player.ID).ToList());
        }

        else if(gameEvent.ID == GameEventId.EntityTookDamage)
        {
            string entityId = gameEvent.GetValue<string>(EventParameters.Entity);
            var entity = EntityQuery.GetEntity(entityId);
            int damage = gameEvent.GetValue<int>(EventParameters.Damage);

            if (!m_EntityToPointMap.ContainsKey(entity))
                return;

            Point p = m_EntityToPointMap[entity];
            GameObject mapObject = m_GameObjectMap[p];
            Vector2 newPos = (Vector2)Camera.main.WorldToScreenPoint(mapObject.transform.position);
            newPos.y += (mapObject.GetComponent<SpriteRenderer>().sprite.textureRect.height);

            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("UI/FadeText"));
            go.GetComponent<FadeTextMono>().Setup($"-{damage}", 1, entity, Color.red);
            go.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
        }

        else if(gameEvent.ID == GameEventId.EntityHealedDamage)
        {
            string entityId = gameEvent.GetValue<string>(EventParameters.Entity);
            var entity = EntityQuery.GetEntity(entityId);
            int healing = gameEvent.GetValue<int>(EventParameters.Healing);

            if (!m_EntityToPointMap.ContainsKey(entity))
                return;

            Point p = m_EntityToPointMap[entity];
            GameObject mapObject = m_GameObjectMap[p];
            Vector2 newPos = (Vector2)Camera.main.WorldToScreenPoint(mapObject.transform.position);
            newPos.y += (mapObject.GetComponent<SpriteRenderer>().sprite.textureRect.height);

            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("UI/FadeText"));
            go.GetComponent<FadeTextMono>().Setup($"+{healing}", 1, entity, Color.green);
            go.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
        }
    }
}
