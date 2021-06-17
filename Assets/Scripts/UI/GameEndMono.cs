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
        SceneManager.LoadSceneAsync("Dungeon").completed += (scene) =>
                {
                    SaveSystem.Instance.CleanCurrentSave();
                    FindObjectOfType<World>().StartWorld(true, SaveSystem.Instance.CurrentSaveName);
                };
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Title");
    }
}
