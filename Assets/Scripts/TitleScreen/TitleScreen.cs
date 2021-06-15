using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public GameObject LoadGames;
    public GameObject Button;
    public GameObject Content;
    public GameObject GetNewSaveName;

    public void StartNewGame()
    {
        GetNewSaveName.SetActive(true);
        GetNewSaveName.GetComponent<TMP_InputField>().onEndEdit.AddListener((val) =>
        {
            SceneManager.LoadSceneAsync("Dungeon").completed += (scene) =>
            {
                FindObjectOfType<World>().StartWorld(true, val);
            };
        });
    }

    public void OpenLoadGames()
    {
        foreach (var directory in Directory.EnumerateDirectories(SaveSystem.kSaveDataPath))
        {
            Debug.Log(directory);
            GameObject instance = Instantiate(Button, Content.transform);
            instance.GetComponentInChildren<TextMeshProUGUI>().text = Path.GetFileName(directory);
            instance.GetComponent<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadSceneAsync("Dungeon").completed += (scene) =>
                {
                    FindObjectOfType<World>().StartWorld(false, directory);
                };
            });
        }
        LoadGames.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetNewSaveName.SetActive(false);
            LoadGames.SetActive(false);
            if (Content.transform.childCount > 0)
            {
                foreach (var tran in Content.transform.GetComponentsInChildren<Transform>())
                    Destroy(tran.gameObject);
            }
        }
    }
}
