using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Dictionary composed of the names of the actions and their associated keys (keycodes)
    private static Dictionary<string, KeyCode> s_actionKeys;
    private static string s_forward = "Move Forward";
    private static string s_backward = "Move Backward";
    private static string s_left = "Move Left";
    private static string s_right = "Move Right";
    private static string s_shoot = "Shoot";
    private static string s_sonar = "Sonar";
    private static string s_dash = "Dash";

    // Default key values
    private static KeyCode s_forwardKey = KeyCode.W;
    private static KeyCode s_backwardKey = KeyCode.S;
    private static KeyCode s_leftKey = KeyCode.A;
    private static KeyCode s_rightKey = KeyCode.D;
    private static KeyCode s_shootKey = KeyCode.Mouse0;
    private static KeyCode s_sonarKey = KeyCode.Space;
    private static KeyCode s_dashKey = KeyCode.LeftShift;

    // These 2 variables take values between -1 and 1
    private static float m_horizMovement = 0;
    private static float m_vertMovement = 0;
    private static float m_acceleration = 0.2f;

    private void OnEnable()
    {
        s_actionKeys = new Dictionary<string, KeyCode>();

        InitActionKeys();
    }

    // Returns true while the key associated to the given action is pressed down
    public static bool GetKey(string actionName)
    {
        if (s_actionKeys.ContainsKey(actionName) == false)
        {
            Debug.LogError("InputManager::GetKey - No action named: " + actionName);
            return false;
        }

        return Input.GetKey(s_actionKeys[actionName]);
    }

    // Returns true when the key associated to the given action is pressed down
    public static bool GetKeyDown(string actionName)
    {
        if (s_actionKeys.ContainsKey(actionName) == false)
        {
            Debug.LogError("InputManager::GetKeyDown - No action named: " + actionName);
            return false;
        }

        return Input.GetKeyDown(s_actionKeys[actionName]);
    }

    // Returns an array containing the names of the actions 
    public static string[] GetActionNames()
    {
        return s_actionKeys.Keys.ToArray();
    }

    // Returns the name of the action corresponding to the given keycode if it exists
    public static string GetActionName(KeyCode keycode)
    {
        foreach (KeyValuePair<string, KeyCode> actionKey in s_actionKeys)
        {
            if (actionKey.Value == keycode)
            {
                return actionKey.Key;
            }
        }
        return null;
    }
    
    // Returns the name of the key corresponding to the given action
    public static string GetKeyName(string actionName)
    {
        if (s_actionKeys.ContainsKey(actionName) == false)
        {
            Debug.LogError("InputManager::GetKeyName - No action named: " + actionName);
            return "N/A";
        }

        return s_actionKeys[actionName].ToString();
    }

    // Sets the value of the given key in s_actionKeys to the given keycode
    public static void SetKey(string actionName, KeyCode keyCode)
    {
        s_actionKeys[actionName] = keyCode;
    }

    // Returns a value corresponding to the movement when a key is pressed down, depending on the given axis
    public static float GetAxis(string axisName)
    {
        if (axisName == "Horizontal")
        {
            if (GetKey("Move Right"))
            {
                m_horizMovement = Mathf.Lerp(m_horizMovement, 1, m_acceleration);
                return m_horizMovement;
            }
            else if (GetKey("Move Left"))
            {
                m_horizMovement = Mathf.Lerp(m_horizMovement, -1, m_acceleration);
                return m_horizMovement;
            }
            else
            {
                m_horizMovement = Mathf.Lerp(m_horizMovement, 0, m_acceleration);
                return m_horizMovement;
            }
        }
        else if (axisName == "Vertical")
        {
            if (GetKey("Move Forward"))
            {
                m_vertMovement = Mathf.Lerp(m_vertMovement, 1, m_acceleration);
                return m_vertMovement;
            }
            else if (GetKey("Move Backward"))
            {
                m_vertMovement = Mathf.Lerp(m_vertMovement, -1, m_acceleration);
                return m_vertMovement;
            }
            else
            {
                m_vertMovement = Mathf.Lerp(m_vertMovement, 0, m_acceleration);
                return m_vertMovement;
            }
        }
        else
        {
            throw new System.Exception("Wrong axis name");
        }
    }

    // Saves the key bindings
    public void Apply()
    {
        foreach(KeyValuePair<string, KeyCode> actionKey in s_actionKeys)
        {
            PlayerPrefs.SetInt(actionKey.Key, (int)actionKey.Value);
        }
    }

    // Sets the key bindings to default bindings
    public void SetToDefault()
    {
        s_actionKeys[s_forward] = s_forwardKey;
        s_actionKeys[s_backward] = s_backwardKey;
        s_actionKeys[s_left] = s_leftKey;
        s_actionKeys[s_right] = s_rightKey;
        s_actionKeys[s_shoot] = s_shootKey;
        s_actionKeys[s_sonar] = s_sonarKey;
        s_actionKeys[s_dash] = s_dashKey;
    }

    // Loads the keycode of each action if it has been saved. Takes the default keycode otherwise
    public void InitActionKeys()
    {
        s_actionKeys[s_forward] = PlayerPrefs.HasKey(s_forward) ? (KeyCode)PlayerPrefs.GetInt(s_forward) : s_forwardKey;
        s_actionKeys[s_backward] = PlayerPrefs.HasKey(s_backward) ? (KeyCode)PlayerPrefs.GetInt(s_backward) : s_backwardKey;
        s_actionKeys[s_left] = PlayerPrefs.HasKey(s_left) ? (KeyCode)PlayerPrefs.GetInt(s_left) : s_leftKey;
        s_actionKeys[s_right] = PlayerPrefs.HasKey(s_right) ? (KeyCode)PlayerPrefs.GetInt(s_right) : s_rightKey;
        s_actionKeys[s_shoot] = PlayerPrefs.HasKey(s_shoot) ? (KeyCode)PlayerPrefs.GetInt(s_shoot) : s_shootKey;
        s_actionKeys[s_sonar] = PlayerPrefs.HasKey(s_sonar) ? (KeyCode)PlayerPrefs.GetInt(s_sonar) : s_sonarKey;
        s_actionKeys[s_dash] = PlayerPrefs.HasKey(s_dash) ? (KeyCode)PlayerPrefs.GetInt(s_dash) : s_dashKey;
        
    }
}
