using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreationMono : MonoBehaviour
{
    public TMP_InputField InputField;
    public Image CharacterImage;
    public TextMeshProUGUI CharacterClass;

    static List<Sprite> m_CharacterImages;
    static List<ClassArchitype> m_CharacterArchitypes;
    int currentIndex = 0;
    int currentClassIndex = 0;

    private void Start()
    {
        if(m_CharacterImages == null)
            m_CharacterImages = Resources.LoadAll<Sprite>("Textures/Characters/").ToList();
        if (m_CharacterArchitypes == null)
            m_CharacterArchitypes = Resources.LoadAll<ClassArchitype>("Architypes").ToList();
        CharacterImage.sprite = m_CharacterImages[currentIndex];
        CharacterClass.text = m_CharacterArchitypes[currentClassIndex].GetReadout();
    }

    public void MoveRight()
    {
        currentIndex++;
        if (currentIndex >= m_CharacterImages.Count)
            currentIndex = 0;
        CharacterImage.sprite = m_CharacterImages[currentIndex];
    }

    public void MoveLeft()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = m_CharacterImages.Count - 1;
        CharacterImage.sprite = m_CharacterImages[currentIndex];
    }

    public void MoveClassLeft()
    {
        currentClassIndex--;
        if (currentClassIndex < 0)
            currentClassIndex = m_CharacterArchitypes.Count - 1;
        CharacterClass.text = m_CharacterArchitypes[currentClassIndex].GetReadout();
    }

    public void MoveClassRight()
    {
        currentClassIndex++;
        if (currentClassIndex >= m_CharacterArchitypes.Count)
            currentClassIndex = 0;
        CharacterClass.text = m_CharacterArchitypes[currentClassIndex].GetReadout();
    }

    public string CreateEntityData()
    {
        IEntity character = EntityFactory.CreateEntity("CharacterTemplate");

        string name = string.IsNullOrEmpty(InputField.text) ? EntityFactory.GetRandomCharacterName() : InputField.text;

        character.AddComponent(new Name(name));
        character.AddComponent(new GraphicContainer("Textures/Characters/" + m_CharacterImages[currentIndex].name));
        character.AddComponent(new Portrait("Textures/Characters/" + m_CharacterImages[currentIndex].name));
        character.AddComponent(new Stats(m_CharacterArchitypes[currentClassIndex].Str,
            m_CharacterArchitypes[currentClassIndex].Agi,
            m_CharacterArchitypes[currentClassIndex].Con,
            m_CharacterArchitypes[currentClassIndex].Wis,
            m_CharacterArchitypes[currentClassIndex].Int,
            m_CharacterArchitypes[currentClassIndex].Cha,
            0));
        var ca = m_CharacterArchitypes[currentClassIndex];

        character.AddComponent(new EquipmentSlot(ca.HeadEquip, BodyPart.Head));
        character.AddComponent(new EquipmentSlot(ca.TorosEquip, BodyPart.Torso));
        character.AddComponent(new EquipmentSlot(ca.ArmEquip1, BodyPart.Arm));
        character.AddComponent(new EquipmentSlot(ca.ArmEquip2, BodyPart.Arm));
        character.AddComponent(new EquipmentSlot(ca.LegEquip1, BodyPart.Leg));
        character.AddComponent(new EquipmentSlot(ca.LegEquip2, BodyPart.Leg));

        character.AddComponent(new PrimaryStatType(ca.PrimaryStatType));

        character.CleanupComponents();
        character.Start();
        return character.Serialize();
    }
}
