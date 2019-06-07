using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeybindDialogBox : MonoBehaviour
{
    [SerializeField] private GameObject m_keyItemPrefab = null;
    [SerializeField] private GameObject m_keyList = null;


    private string m_keyToRebind = null;
    private Dictionary<string, TextMeshProUGUI> m_keyToLabel;

    private void Start()
    {
        string[] actionNames = InputManager.GetActionNames();
        m_keyToLabel = new Dictionary<string, TextMeshProUGUI>();

        for (int i = 0; i < actionNames.Length; i++)
        {
            string actionName = actionNames[i];

            // Creates one "Key List Item" per action defined in InputManager
            GameObject keyItem = Instantiate(m_keyItemPrefab);
            keyItem.transform.SetParent(m_keyList.transform);
            keyItem.transform.localScale = Vector3.one;

            // Sets the name of the action in the "Keybind Dialog Box"
            TextMeshProUGUI actionNameText = keyItem.transform.Find("ActionName").GetComponent<TextMeshProUGUI>();
            actionNameText.text = actionName;

            // Sets the name of the key in the "Keybind Dialog Box"
            TextMeshProUGUI keyNameText = keyItem.transform.Find("Button/KeyName").GetComponent<TextMeshProUGUI>();
            keyNameText.text = InputManager.GetKeyName(actionName);
            m_keyToLabel[actionName] = keyNameText;

            // Adds the possiblity to change the key
            Button keyBindButton = keyItem.transform.Find("Button").GetComponent<Button>();
            keyBindButton.onClick.AddListener(() => { StartRebind(actionName); });
        }
    }

    private void Update()
    {
        // If the user wants to rebind a key and if a key was pressed down
        if (m_keyToRebind != null && Input.anyKeyDown)
        {
            // Loop through all possible keys and see which one was pressed down
            Array keyCodes = Enum.GetValues(typeof(KeyCode));
            foreach (KeyCode keyCode in keyCodes)
            {
                // Rebind the key pressed down and its label. If the new key is 'Escape', do nothing
                if (Input.GetKeyDown(keyCode) && keyCode != KeyCode.Escape)
                {
                    string actionName = InputManager.GetActionName(keyCode);
                    // If the key is already assigned to another action set its value to none for the other action
                    if (actionName != null)
                    {
                        InputManager.SetKey(actionName, KeyCode.None);
                        m_keyToLabel[actionName].text = "";
                    }

                    InputManager.SetKey(m_keyToRebind, keyCode);
                    m_keyToLabel[m_keyToRebind].text = keyCode.ToString();
                    m_keyToRebind = null;
                    break;
                }
            }
        }
    }

    private void StartRebind(string actionName)
    {
        Debug.Log(actionName);
        m_keyToRebind = actionName;
    }

    public void UpdateKeysText()
    {
        foreach (KeyValuePair<string, TextMeshProUGUI> keyToLabel in m_keyToLabel)
        {
            m_keyToLabel[keyToLabel.Key].text = InputManager.GetKeyName(keyToLabel.Key);
        }
    }
}
