using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileMenu : MonoBehaviour
{
    private const string USERNAME_KEY = "username";

    private static string s_username;

    [SerializeField]
    private List<string> m_randomNames = new List<string>();
    [SerializeField]
    private InputField m_usernameField;
    [SerializeField]
    private TextMeshProUGUI m_savingText;

    private void Awake()
    {
        s_username = LoadUsername();
        m_usernameField.text = s_username;
    }

    // Return the saved username if it exists or a random username
    private string LoadUsername()
    {
        if (PlayerPrefs.HasKey(USERNAME_KEY))
        {
            return PlayerPrefs.GetString(USERNAME_KEY);
        }
        else
        {
            string username = GetRandomName();
            SaveUsername(username);
            return username;
        }
    }

    private string GetRandomName()
    {
        return m_randomNames[Random.Range(0, m_randomNames.Count)];
    }

    private void SaveUsername(string username)
    {
        PlayerPrefs.SetString(USERNAME_KEY, username);
    }

    // Check if the username is valid and save it
    public void Save()
    {
        if (string.IsNullOrWhiteSpace(m_usernameField.text))
        {
            m_usernameField.text = s_username;
            StartCoroutine(ShowMessage("Username cannot be empty", 2));
        }
        else
        {
            SaveUsername(m_usernameField.text);
            StartCoroutine(ShowMessage("Username saved", 1));
        }
    }

    // Show the given message for the given delay in seconds
    IEnumerator ShowMessage(string message, float delay)
    {
        m_savingText.text = message;
        m_savingText.enabled = true;
        yield return new WaitForSeconds(delay);
        m_savingText.enabled = false;
    }

    public static string GetUsername()
    {
        return s_username;
    }

}
