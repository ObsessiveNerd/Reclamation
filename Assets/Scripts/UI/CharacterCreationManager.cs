using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCreationManager : MonoBehaviour
{
    public List<CharacterCreationMono> Characters;

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

        SceneManager.LoadSceneAsync("Dungeon").completed += s =>
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
