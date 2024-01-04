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
            m_CharacterImages = Resources.LoadAll<Sprite>("Textures/Sprites/Characters/").ToList();
        if (m_CharacterArchitypes == null)
            m_CharacterArchitypes = Resources.LoadAll<ClassArchitype>("Architypes").ToList();
        CharacterImage.sprite = m_CharacterImages[currentIndex];
        CharacterClass.text = m_CharacterArchitypes[currentClassIndex].Name;
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
        CharacterClass.text = m_CharacterArchitypes[currentClassIndex].Name;
    }

    public void MoveClassRight()
    {
        currentClassIndex++;
        if (currentClassIndex >= m_CharacterArchitypes.Count)
            currentClassIndex = 0;
        CharacterClass.text = m_CharacterArchitypes[currentClassIndex].Name;
    }

    public string CreateEntityData()
    {
        return "";
    }

    public void Randomize()
    {
       
    }

    public string GetClassReadout()
    {
        return m_CharacterArchitypes[currentClassIndex].GetReadout();
    }
}
