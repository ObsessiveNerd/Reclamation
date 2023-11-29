using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class WorldUIController : GameService
{
    static List<IUpdatableUI> UpdatableUI = new List<IUpdatableUI>();

    public void UpdateUI()
    {
        List<IUpdatableUI> updateableUIs = new List<IUpdatableUI>(UpdatableUI);
        foreach (var ui in updateableUIs)
            ui?.UpdateUI();
    }

    public void SetCharacterView(string setTo)
    {
        GameObject.FindObjectOfType<CharacterMono>().ToggleSelected(setTo);
    }

    public void EntityTookDamage(IEntity entity, int damage)
    {
        if (!m_EntityToPointMap.ContainsKey(entity.ID))
            return;

        Point p = m_EntityToPointMap[entity.ID];
        GameObject mapObject = m_GameObjectMap[p];
        Vector2 newPos = (Vector2)Camera.main.WorldToScreenPoint(mapObject.transform.position);
        newPos.y += (mapObject.GetComponent<SpriteRenderer>().sprite.textureRect.height);

        GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/FadeText"));
        go.GetComponent<FadeTextMono>().Setup($"-{damage}", 1, entity, Color.red);
        go.transform.SetParent(Object.FindObjectOfType<Canvas>().transform);
    }

    public void EntityRegainedMana(IEntity entity, int amount)
    {
        if (!m_EntityToPointMap.ContainsKey(entity.ID))
            return;

        Point p = m_EntityToPointMap[entity.ID];
        GameObject mapObject = m_GameObjectMap[p];
        Vector2 newPos = (Vector2)Camera.main.WorldToScreenPoint(mapObject.transform.position);
        newPos.y += (mapObject.GetComponent<SpriteRenderer>().sprite.textureRect.height);

        GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/FadeText"));
        go.GetComponent<FadeTextMono>().Setup($"+{amount}", 1, entity, Color.blue);
        go.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
    }

    public void EntityHealedDamage(IEntity entity, int healing)
    {
        if (!m_EntityToPointMap.ContainsKey(entity.ID))
            return;

        Point p = m_EntityToPointMap[entity.ID];
        GameObject mapObject = m_GameObjectMap[p];
        Vector2 newPos = (Vector2)Camera.main.WorldToScreenPoint(mapObject.transform.position);
        newPos.y += (mapObject.GetComponent<SpriteRenderer>().sprite.textureRect.height);

        GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/FadeText"));
        go.GetComponent<FadeTextMono>().Setup($"+{healing}", 1, entity, Color.green);
        go.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
    }

    public void PromptToGiveItem(IEntity source, IEntity item)
    {
        var contextMenu = ContextMenuMono.CreateNewContextMenu();
        contextMenu.GetComponent<ContextMenuMono>().SelectPlayer((target) =>
        {
            if(item.HasComponent(typeof(Stackable)))
            {
                contextMenu.GetComponent<ContextMenuMono>().GetAmountToGive((s, i, amount) =>
                {
                    for(int a = 0; a < amount; a++)
                    {
                        var actualItem = source.GetComponent<Inventory>().InventoryItems.FirstOrDefault(it => it.Name == i.Name);
                        if (actualItem == null)
                            break;

                        GameEvent unEquip = GameEventPool.Get(GameEventId.Unequip)
                                            .With(EventParameter.Entity, s.ID)
                                            .With(EventParameter.Item, actualItem.ID);
                        s.FireEvent(unEquip).Release();

                        GameEvent removeFromInventory = GameEventPool.Get(GameEventId.RemoveFromInventory)
                                                            .With(EventParameter.Item, actualItem.ID);
                        s.FireEvent(removeFromInventory).Release();

                        GameEvent addToInventory = GameEventPool.Get(GameEventId.AddToInventory)
                                                        .With(EventParameter.Entity, actualItem.ID);
                        EntityQuery.GetEntity(target).FireEvent(addToInventory).Release();
                    }
                    Services.WorldUIService.UpdateUI();
                }, source, item);
            }
            else
            {
                GameEvent unEquip = GameEventPool.Get(GameEventId.Unequip)
                                            .With(EventParameter.Entity, source.ID)
                                            .With(EventParameter.Item, item.ID);
                source.FireEvent(unEquip).Release();

                GameEvent removeFromInventory = GameEventPool.Get(GameEventId.RemoveFromInventory)
                                                    .With(EventParameter.Item, item.ID);
                source.FireEvent(removeFromInventory).Release();

                GameEvent addToInventory = GameEventPool.Get(GameEventId.AddToInventory)
                                                .With(EventParameter.Entity, item.ID);
                EntityQuery.GetEntity(target).FireEvent(addToInventory).Release();
                Services.WorldUIService.UpdateUI();
            }

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
        GameObject.FindObjectOfType<UIReferences>().OpenChest(chest);
        UpdateUI();
    }

    public void OpenSelectAllSpellUI()
    {
        GameObject.FindObjectOfType<UIReferences>().OpenSelectFromAllSpells();
    }

    public void OpenSpellExaminationUI(List<string> spellIds)
    {
        GameObject.FindObjectOfType<UIReferences>().OpenSpellExaminer(spellIds);
        UpdateUI();
    }

    public void RegisterPlayableCharacter(string id)
    {
        GameObject.FindObjectOfType<PlayableCharacterSelector>().AddCharacterTab(id);
        UpdateUI();
    }

    public void UnRegisterPlayableCharacter(string id)
    {
        GameObject.FindObjectOfType<PlayableCharacterSelector>().RemoveCharacterTab(id);
        UpdateUI();
    }

    public void OpenSpellUI()
    {
        GameObject.FindObjectOfType<UIReferences>().OpenSpellSelector();
    }

    public void OpenInventory()
    {
        GameObject.FindObjectOfType<UIReferences>().OpenCharacterManager();
        UpdateUI();
    }

    public void OpenDebugMenu()
    {
        GameObject.FindObjectOfType<UIReferences>().OnOpenDebugMenu();
    }

    public void OpenEnchantmentUI(IEntity source, IEntity enchantment)
    {
        GameObject.FindObjectOfType<UIReferences>().OpenEnchanter(source, enchantment);
        UpdateUI();
    }
}
