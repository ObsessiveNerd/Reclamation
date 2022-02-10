using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : EscapeableMono
{
    public GameObject LoadGames;
    public GameObject Button;
    public GameObject Content;
    public GameObject GetNewSaveName;
    public GameObject Error;

    public void StartNewGame()
    {
        UIManager.Push(this);
        GetNewSaveName.SetActive(true);
        GetNewSaveName.GetComponent<TMP_InputField>().onEndEdit.AddListener((val) =>
        {
            //FindObjectOfType<World>().StartWorld(val);

            if(Directory.Exists($"{GameSaveSystem.kSaveDataPath}/{val}"))
            {
                Error.SetActive(true);
                Error.GetComponentInChildren<TextMeshProUGUI>().text = "Save game already exists, please use a different name or load the requested game.";
                GetNewSaveName.GetComponent<TMP_InputField>().text = "";

            }
            else
            {
                UIManager.ForcePop(this);
                SceneManager.LoadSceneAsync("CharacterCreation").completed += (scene) =>
                {
                    FindObjectOfType<World>().StartWorld(val);
                };
            }
        });
    }

    public void OpenLoadGames()
    {
        UIManager.Push(this);
        foreach (var directory in Directory.EnumerateDirectories(GameSaveSystem.kSaveDataPath))
        {
            Debug.Log(directory);
            GameObject instance = Instantiate(Button, Content.transform);
            instance.GetComponentInChildren<TextMeshProUGUI>().text = Path.GetFileName(directory);
            instance.GetComponent<Button>().onClick.AddListener(() =>
            {
                var contextMenu = ContextMenuMono.CreateNewContextMenu().GetComponent<ContextMenuMono>();
                contextMenu.AddButton(new ContextMenuButton("Load", () =>
                        {
                            UIManager.ForcePop(this);
                            SceneManager.LoadSceneAsync("Reclaim/Scenes/Dungeon").completed += (scene) =>
                              {
                                  FindObjectOfType<World>().StartWorld(directory);
                                  Services.DungeonService.GenerateDungeon(false, directory);
                              };
                        }), null, () => Destroy(contextMenu.gameObject));

                contextMenu.AddButton(new ContextMenuButton("Delete", () =>
                {
                    Directory.Delete(directory, true);
                    Destroy(instance);
                }), null, () => Destroy(contextMenu.gameObject));
            });
        }
        LoadGames.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    protected override void OnDisable() { }

    public override void OnEscape()
    {
        GetNewSaveName?.SetActive(false);
        Error?.SetActive(false);
        LoadGames?.SetActive(false);
        if (Content?.transform.childCount > 0)
        {
            foreach (var tran in Content.transform.GetComponentsInChildren<Transform>())
            {
                if (tran == Content.transform)
                    continue;
                Destroy(tran.gameObject);
            }
        }
    }

    public override bool? AlternativeEscapeKeyPressed
    {
        get
        {
            return Input.GetKeyDown(KeyCode.Escape) && !m_OpenedThisFrame;
        }
    }
}
