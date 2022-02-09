using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class WorldUIController : GameService
{
    static List<IUpdatableUI> UpdatableUI = new List<IUpdatableUI>();

    public void UpdateUI(string id)
    {
        foreach (var ui in UpdatableUI)
            ui?.UpdateUI(EntityQuery.GetEntity(id));

        GameObject.FindObjectOfType<SpellSelectorMono>().Setup(m_ActivePlayer.Value);
    }

    public void UpdateUI()
    {
        foreach (var ui in UpdatableUI)
            ui?.UpdateUI(EntityQuery.GetEntity(Services.WorldDataQuery.GetActivePlayerId()));

        GameObject.FindObjectOfType<SpellSelectorMono>().Setup(m_ActivePlayer.Value);
    }

    public void EntityTookDamage(IEntity entity, int damage)
    {
        if (!m_EntityToPointMap.ContainsKey(entity))
            return;

        Point p = m_EntityToPointMap[entity];
        GameObject mapObject = m_GameObjectMap[p];
        Vector2 newPos = (Vector2)Camera.main.WorldToScreenPoint(mapObject.transform.position);
        newPos.y += (mapObject.GetComponent<SpriteRenderer>().sprite.textureRect.height);

        GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("UI/FadeText"));
        go.GetComponent<FadeTextMono>().Setup($"-{damage}", 1, entity, Color.red);
        go.transform.SetParent(Object.FindObjectOfType<Canvas>().transform);
    }

    public void EntityHealedDamage(IEntity entity, int healing)
    {
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

    public void PromptToGiveItem(IEntity source, IEntity item)
    {
        ContextMenuMono.CreateNewContextMenu().GetComponent<ContextMenuMono>().SelectPlayer((target) =>
        {
            GameEvent unEquip = GameEventPool.Get(GameEventId.Unequip)
                                        .With(EventParameters.Entity, source.ID)
                                        .With(EventParameters.Item, item.ID);
            source.FireEvent(unEquip).Release();

            GameEvent removeFromInventory = GameEventPool.Get(GameEventId.RemoveFromInventory)
                                                .With(EventParameters.Item, item.ID);
            source.FireEvent(removeFromInventory).Release();

            GameEvent addToInventory = GameEventPool.Get(GameEventId.AddToInventory)
                                            .With(EventParameters.Entity, item.ID);
            EntityQuery.GetEntity(target).FireEvent(addToInventory).Release();

            Services.WorldUIService.UpdateUI(source.ID); 
            //GameObject.FindObjectOfType<CharacterManagerMono>().UpdateUI(source);

        }, m_Players.Select(player => player.ID).ToList());
    }

    public void UnregisterUpdatableUI(IUpdatableUI go)
    {
        if (UpdatableUI.Contains(go))
            UpdatableUI.Remove(go);
    }

    public void RegisterUpdatableUI(IUpdatableUI go)
    {
        if (!UpdatableUI.Contains(go))
            UpdatableUI.Add(go);
    }

    public void OpenChestUI(IEntity chest, IEntity character)
    {
        GameObject.FindObjectOfType<ChestMono>().Init(chest);
    }

    public void OpenSpellExaminationUI(List<string> spellIds)
    {
        GameObject.FindObjectOfType<SpellExaminationUI>().Setup(spellIds);
    }

    public void RegisterPlayableCharacter(string id)
    {
        GameObject.FindObjectOfType<PlayableCharacterSelector>().AddCharacterTab(id);
    }

    public void OpenSpellUI(IEntity source)
    {
        GameObject.FindObjectOfType<SpellSelectorMono>().Setup(source);
    }

    public void OpenInventory(IEntity source)
    {
        GameObject.FindObjectOfType<CharacterManagerMono>().Setup(source);
        GameObject.FindObjectOfType<CharacterManagerMono>().AddCharacter(source);

        //foreach (var entity in m_Players)
        //    GameObject.FindObjectOfType<CharacterManagerMono>().AddCharacter(entity);
    }

    public void OpenEnchantmentUI(IEntity source, IEntity enchantment)
    {
        GameObject.FindObjectOfType<EnchantmentManagerMono>().Setup(source, enchantment);
    }
}
