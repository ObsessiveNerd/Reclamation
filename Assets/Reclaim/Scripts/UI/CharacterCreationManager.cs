using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCreationManager : MonoBehaviour
{
    public GameObject PlayerCreationObject;
    public int NumberOfCharacters = 4;
    public Transform CharactersContent;
    List<CharacterCreationMono> Characters = new List<CharacterCreationMono>();

    private void Start()
    {
        for(int i = 0; i < NumberOfCharacters; i++)
            Characters.Add(Instantiate(PlayerCreationObject, CharactersContent).GetComponent<CharacterCreationMono>());
    }

    public void Play()
    {
        World world = FindObjectOfType<World>();
        foreach(var cc in Characters)
        {
            string data = cc.CreateEntityData();
            string name = EntityFactory.GetEntityNameFromBlueprintFormatting(data);
            EntityFactory.CreateTemporaryBlueprint($"Characters/{name}", data);
        }

        EntityFactory.ReloadTempBlueprints();

        SceneManager.LoadSceneAsync("Reclaim/Scenes/Dungeon").completed += s =>
        {
            Services.DungeonService.GenerateDungeon(true, Services.SaveAndLoadService.CurrentSaveName);
        };
    }

    public void RandomizeAll()
    {
        foreach (var character in Characters)
            character.Randomize();
    }
}
