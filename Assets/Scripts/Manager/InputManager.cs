/**
 * Authors: Stella, David
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Dictionary composed of the names of the actions and their associated keys (keycodes)
    private static Dictionary<string, KeyCode> s_actionKeys = new Dictionary<string, KeyCode>();
    private const string FORWARD = "Move Forward";
    private const string BACKWARD = "Move Backward";
    private const string LEFT = "Move Left";
    private const string RIGHT = "Move Right";
    private const string SHOOT = "Shoot";
    private const string SONAR = "Sonar";
    private const string DASH = "Dash";

    // Default key values
    private const KeyCode FORWARD_KEY = KeyCode.W;
    private const KeyCode BACKWARD_KEY = KeyCode.S;
    private const KeyCode LEFT_KEY = KeyCode.A;
    private const KeyCode RIGHT_KEY = KeyCode.D;
    private const KeyCode SHOOT_KEY = KeyCode.Mouse0;
    private const KeyCode SONAR_KEY = KeyCode.Space;
    private const KeyCode DASH_KEY = KeyCode.LeftShift;

    private const float ACCELERATION = 0.3f;
    // These 2 variables take values between -1 and 1
    private static float m_horizMovement = 0;
    private static float m_vertMovement = 0;

    private void OnEnable()
    {
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
                m_horizMovement = Mathf.Lerp(m_horizMovement, 1, ACCELERATION);
                return m_horizMovement;
            }
            else if (GetKey("Move Left"))
            {
                m_horizMovement = Mathf.Lerp(m_horizMovement, -1, ACCELERATION);
                return m_horizMovement;
            }
            else
            {
                m_horizMovement = Mathf.Lerp(m_horizMovement, 0, ACCELERATION);
                return m_horizMovement;
            }
        }
        else if (axisName == "Vertical")
        {
            if (GetKey("Move Forward"))
            {
                m_vertMovement = Mathf.Lerp(m_vertMovement, 1, ACCELERATION);
                return m_vertMovement;
            }
            else if (GetKey("Move Backward"))
            {
                m_vertMovement = Mathf.Lerp(m_vertMovement, -1, ACCELERATION);
                return m_vertMovement;
            }
            else
            {
                m_vertMovement = Mathf.Lerp(m_vertMovement, 0, ACCELERATION);
                return m_vertMovement;
            }
        }
        else
        {
            Debug.LogError("Wrong axis name: " + axisName);
            return 0;
        }
    }

    // Saves the key bindings
    public void Apply()
    {
        foreach (KeyValuePair<string, KeyCode> actionKey in s_actionKeys)
        {
            PlayerPrefs.SetInt(actionKey.Key, (int)actionKey.Value);
        }
    }

    // Sets the key bindings to default bindings
    public void SetToDefault()
    {
        s_actionKeys[FORWARD] = FORWARD_KEY;
        s_actionKeys[BACKWARD] = BACKWARD_KEY;
        s_actionKeys[LEFT] = LEFT_KEY;
        s_actionKeys[RIGHT] = RIGHT_KEY;
        s_actionKeys[SHOOT] = SHOOT_KEY;
        s_actionKeys[SONAR] = SONAR_KEY;
        s_actionKeys[DASH] = DASH_KEY;
    }

    // Loads the keycode of each action if it has been saved. Takes the default keycode otherwise
    public void InitActionKeys()
    {
        s_actionKeys[FORWARD] = PlayerPrefs.HasKey(FORWARD) ? (KeyCode)PlayerPrefs.GetInt(FORWARD) : FORWARD_KEY;
        s_actionKeys[BACKWARD] = PlayerPrefs.HasKey(BACKWARD) ? (KeyCode)PlayerPrefs.GetInt(BACKWARD) : BACKWARD_KEY;
        s_actionKeys[LEFT] = PlayerPrefs.HasKey(LEFT) ? (KeyCode)PlayerPrefs.GetInt(LEFT) : LEFT_KEY;
        s_actionKeys[RIGHT] = PlayerPrefs.HasKey(RIGHT) ? (KeyCode)PlayerPrefs.GetInt(RIGHT) : RIGHT_KEY;
        s_actionKeys[SHOOT] = PlayerPrefs.HasKey(SHOOT) ? (KeyCode)PlayerPrefs.GetInt(SHOOT) : SHOOT_KEY;
        s_actionKeys[SONAR] = PlayerPrefs.HasKey(SONAR) ? (KeyCode)PlayerPrefs.GetInt(SONAR) : SONAR_KEY;
        s_actionKeys[DASH] = PlayerPrefs.HasKey(DASH) ? (KeyCode)PlayerPrefs.GetInt(DASH) : DASH_KEY;

    }
}
