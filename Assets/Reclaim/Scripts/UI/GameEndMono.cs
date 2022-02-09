using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndMono : MonoBehaviour
{
    public GameObject EndState;

    public void EnableEndState(bool victory)
    {
        EndState.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = victory ? "Victory!" : "You Died.";
        EndState.SetActive(true);
    }

    public void RestartNewGame()
    {
        string cachedSaveName = Services.SaveAndLoadService.CurrentSaveName;
        Services.SaveAndLoadService.CleanCurrentSave();
        SceneManager.LoadSceneAsync("Dungeon").completed += (scene) =>
                {
                    FindObjectOfType<World>().StartWorld(cachedSaveName);
                    Services.DungeonService.GenerateDungeon(true, cachedSaveName);
                };
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Title");
    }
}
