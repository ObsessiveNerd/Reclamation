using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellSelectorMono : MonoBehaviour//, IUpdatableUI
{
    public GameObject SpellObject;
    public GameObject SpellView;

    //void Start()
    //{
    //    //WorldUtility.RegisterUI(this);

        
    //}

    public void Setup(IEntity source)
    {
        if (source == null || !WorldUtility.IsActivePlayer(source.ID))
            return;

        //GameEvent updateUI = GameEventPool.Get(GameEventId.UpdateUI)
        //                            .With(EventParameters.Entity, WorldUtility.GetActivePlayerId());
        //World.Instance.Self.FireEvent(updateUI).Release();

        //WorldUIController.UpdateUI(WorldUtility.GetActivePlayerId());

        Close();
        GameEvent getSpells = GameEventPool.Get(GameEventId.GetSpells)
                                    .With(EventParameters.SpellList, new HashSet<string>());

        var spellList = source.FireEvent(getSpells).GetValue<HashSet<string>>(EventParameters.SpellList);
        getSpells.Release();
        
        int index = 1;
        foreach (string spellId in spellList)
        {
            IEntity spell = EntityQuery.GetEntity(spellId);

            GameObject spriteGoResource = Resources.Load<GameObject>("UI/SpellUI");
            GameEvent getSpriteEvent = GameEventPool.Get(GameEventId.GetSprite)
                .With(EventParameters.RenderSprite, null);
            Sprite sprite = spell.FireEvent(spell, getSpriteEvent).GetValue<Sprite>(EventParameters.RenderSprite);
            getSpriteEvent.Release();
            if (sprite != null)
            {
                GameObject spriteGo = Instantiate(spriteGoResource);
                Image spriteRenderer = spriteGo.transform.Find("SpellImage").GetComponent<Image>();
                spriteRenderer.sprite = sprite;
                spriteGo.transform.SetParent(SpellView.transform);

                spriteGo.GetComponentInChildren<TextMeshProUGUI>().text = index.ToString();

                //Button button = spriteGo.AddComponent<Button>();
                //button.onClick.AddListener(() =>
                //{
                //    source.FireEvent(GameEventPool.Get(GameEventId.SpellSelected, new .With(EventParameters.Spell, spellId)));
                //    //Close();
                //});
                index++;
            }
        }
        SpellObject.SetActive(true);
        //UIManager.Push(this);
    }

    public void UpdateUI(IEntity newSource)
    {
        Debug.Log("Update Spellselector");
        Close();
        Setup(newSource);
    }

    //public override void OnEscape()
    //{
    //    Close();
    //}

    void Close()
    {
        //SpellObject.SetActive(false);
        foreach (Transform go in SpellView.GetComponentsInChildren<Transform>())
            if(SpellView.transform != go)
                Destroy(go.gameObject);
    }
}
