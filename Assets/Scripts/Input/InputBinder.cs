using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InputBinder : MonoBehaviour
{
    public GameObject UI;

    string InputConfigPath
    {
        get
        {
            return $"{Application.streamingAssetsPath}/input.config";
        }
    }

    static InputKeyBindData m_Data;
    static string m_RequestedAction = null;

    // Start is called before the first frame update
    void Start()
    {
        if(UI == null)
            UI = GameObject.Find("Keybinder");

        if (m_Data != null)
        {
            Destroy(gameObject);
            return;
        }
        else
            DontDestroyOnLoad(gameObject);

        Directory.CreateDirectory(Path.GetDirectoryName(InputConfigPath));
        if(!File.Exists(InputConfigPath))
        {
            m_Data = new InputKeyBindData();
            string stringData = JsonUtility.ToJson(m_Data);
            File.WriteAllText(InputConfigPath, stringData);
        }
        else
        {
            m_Data = JsonUtility.FromJson<InputKeyBindData>(File.ReadAllText(InputConfigPath));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            UI.SetActive(!UI.activeInHierarchy);

        if(m_RequestedAction != null && !string.IsNullOrEmpty(Input.inputString))
        {
            var fieldInfo = typeof(InputKeyBindData).GetField(m_RequestedAction, 
            System.Reflection.BindingFlags.NonPublic 
            | System.Reflection.BindingFlags.Instance);

            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(kcode))
                {
                    fieldInfo.SetValue(m_Data, kcode);
                    File.Delete(InputConfigPath);
                    File.WriteAllText(InputConfigPath, JsonUtility.ToJson(m_Data));
                }
            }

            

            m_RequestedAction = null;
        }
    }

    public void ResetToDefault()
    {
        m_Data = new InputKeyBindData();
        File.Delete(InputConfigPath);
        File.WriteAllText(InputConfigPath, JsonUtility.ToJson(m_Data));
    }

    public static KeyCode GetKeyCodeForAction(RequestedAction action)
    {
        string actionName = action.ToString();

        var fieldInfo = typeof(InputKeyBindData).GetField(actionName, 
            System.Reflection.BindingFlags.NonPublic 
            | System.Reflection.BindingFlags.Instance);

        return (KeyCode)fieldInfo.GetValue(m_Data);
    }

    public static bool PerformRequestedAction(RequestedAction action)
    {
        KeyCode matchingKeyCode = GetKeyCodeForAction(action);
        if (Input.GetKeyDown(matchingKeyCode))
            return true;
        return false;
    }

    public static void SetKeyBinder(RequestedAction ra)
    {
        m_RequestedAction = ra.ToString();
    }
}
