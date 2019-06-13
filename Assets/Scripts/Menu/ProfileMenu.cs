/**
 * Authors: David
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileMenu : MonoBehaviour
{
    private const string USERNAME_KEY = "username";

    private static List<string> s_randomUsernames = new List<string>()
        {
            "Tharemar",
            "Viang",
            "Onomanyth",
            "Sulian",
            "Gwareron",
            "Asorerwen",
            "Rendannon",
            "Doidien",
            "Ocireric",
            "Unaunna"
        };

    [SerializeField] private InputField m_usernameField = null;
    [SerializeField] private TextMeshProUGUI m_savingText = null;

    private void Awake()
    {
        m_usernameField.text = GetUsername();
    }

    // Return the saved username if it exists or a random username
    public static string GetUsername()
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

    public static string GetRandomName()
    {
        return s_randomUsernames[Random.Range(0, s_randomUsernames.Count)];
    }

    private static void SaveUsername(string username)
    {
        PlayerPrefs.SetString(USERNAME_KEY, username);
    }

    // Check if the username is valid and save it
    public void Save()
    {
        if (string.IsNullOrWhiteSpace(m_usernameField.text))
        {
            m_usernameField.text = GetUsername();
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

}
