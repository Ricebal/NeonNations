using Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyMapConfig : NetworkBehaviour
{
    [SerializeField] private GameObject m_optionItemPrefab = null;
    [SerializeField] private GameObject m_mapOptionList = null;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        // Do not display the configuration if the player is not the host
        if (!isServer)
        {
            return;
        }

        foreach (KeyValuePair<string, int> mapOption in LobbyConfigMenu.Singleton.MapOptions)
        {
            // Create one "Option Item" per element defined in the mapOptions dictionary
            GameObject optionItem = Instantiate(m_optionItemPrefab, m_mapOptionList.transform);
            optionItem.transform.localScale = Vector3.one;
            optionItem.name = mapOption.Key;

            // Set the name of the option in the "Option List"
            TextMeshProUGUI optionNameText = optionItem.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            optionNameText.text = mapOption.Key;

            // Set the default value of the option in the "Option List"
            TextMeshProUGUI optionValue = optionItem.transform.Find("Value").GetComponent<TextMeshProUGUI>();
            optionValue.text = mapOption.Value.ToString();

            // Add the possibility to increase the value of the option
            HoldButton buttonUp = optionItem.transform.Find("ButtonUp").GetComponent<HoldButton>();
            buttonUp.OnValueChanged += OnValueChanged;

            // Add the possibility to decrease the value of the option
            HoldButton buttonDown = optionItem.transform.Find("ButtonDown").GetComponent<HoldButton>();
            buttonDown.OnValueChanged += OnValueChanged;
        }
    }

    // Change the value of the option depending of the incremental value of the button
    private void OnValueChanged(HoldButton button)
    {
        Transform parent = button.gameObject.transform.parent;
        int value = LobbyConfigMenu.Singleton.MapOptions[parent.name];
        value += button.IncrementalValue;
        if (value < 0)
        {
            return;
        }

        LobbyConfigMenu.Singleton.MapOptions[parent.name] = value;
        parent.Find("Value").GetComponent<TextMeshProUGUI>().text = value.ToString();
    }
}
