using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct InputKeys
{
    public string commandName;
    public KeybindEnum command;
    public KeyCode keyCode;
}

[CreateAssetMenu(fileName = "Keybindings", menuName = "Keybindings")]
public class Keybindings : ScriptableObject
{
    [Tooltip("Input a string ID for the player. This string will be used to differentiate between players in local co-op")]
    [SerializeField] private string player = null;
    [SerializeField] private InputKeys[] playerInput = null;
    [SerializeField] private bool useMouseSensitivity;
    [SerializeField] private float sensitivity = 2f;
    public Keybindings defaultKeybindings = null;
    [Tooltip("Input all the other KeyBindingsScriptableObject that you want to be cross checked for overlapping input.")]
    [SerializeField] private Keybindings[] overlappingKeybindsChecker = null;
    private Dictionary<KeybindEnum, KeyCode> keyBindingsDictionary = null;

    /*  -----------------------------------------------
           PUBLIC METHODS
        -----------------------------------------------*/
    /*
     * Use this method with Input.GetKey(...);
    */
    //public KeyCode GetKeyCode(string command) { return keyBindingsDictionary[command]; }
    public KeyCode GetKeyCode(KeybindEnum command) { return keyBindingsDictionary[command]; }
    /*
     * Use this method to set the value of keys as text. eg in options menu
    */
    public string GetKeyAsString(KeybindEnum command) { return keyBindingsDictionary[command].ToString(); }
    /*
     * Used to differentiate between multiple players in local co-op
    */
    public string GetPlayerIdentity() { return player; }
    public InputKeys[] GetPlayerInput() { return playerInput; }
    public void SetMouseSensitivity(float toSet) {  sensitivity = toSet;    }
    public float GetMouseSensitivity() { return sensitivity; }
    /*
     * Will change the KeyCode in the array and dictionary.
    */
    public void SetKey(KeybindEnum command, KeyCode toSet, bool reset = false)
    {
        if (!keyBindingsDictionary.ContainsKey(command))
        {
            Debug.LogError("Command name: " + command + " does not exist.");
            return;
        }

        for (int i = 0; i < playerInput.Length; i++)
        {
            if (playerInput[i].command == command)
            {
                if (!reset)
                    CheckOverlappingKeybinds(toSet);
                playerInput[i].keyCode = toSet;
                keyBindingsDictionary[command] = toSet;
                return;
            }
        }
    }
    /*
     * Resets all keybindings to the default set you created.
    */
    public void ResetKeys()
    {
        if (!defaultKeybindings)
        {
            Debug.LogError("No default keybindings set.");
            return;
        }

        foreach (var keybind in playerInput)
            SetKey(keybind.command, defaultKeybindings.GetKeyCode(keybind.command), true);
    }
    /*
     * Saving and loading of the keybindings through PlayerPrefs.
     * The keys will be saved under the name of the variable + the player number, in order to differentiate between
     * multiple players in local co-op.
    */
    public void SaveKeys()
    {
        PlayerPrefs.SetString("Keybinds " + player, "Saved");
        foreach (var keybind in playerInput)
            PlayerPrefs.SetInt(keybind.command + player, keybind.keyCode.GetHashCode());
        if (keyBindingsDictionary == null)
            FillDictionary();
        PlayerPrefs.Save();
    }

    public void SaveMouseSensitivity()
    {
        if (useMouseSensitivity)
        {
            PlayerPrefs.SetString("Mouse Sensitivity " + player, "Saved");
            PlayerPrefs.SetFloat("MS" + player, sensitivity);
            PlayerPrefs.Save();
        }
    }

    public void SaveAll()
    {
        SaveKeys();
        SaveMouseSensitivity();
        PlayerPrefs.Save();
    }
    public void LoadKeys()
    {
        if (!PlayerPrefs.HasKey("Keybinds " + player))
        {
            SaveAll();
            Debug.Log("Created new save keys");
            return;
        }
        if (useMouseSensitivity)
            sensitivity = PlayerPrefs.GetFloat("MS" + player);
        for (int i = 0; i < playerInput.Length; i++)
            playerInput[i].keyCode = (KeyCode)PlayerPrefs.GetInt(playerInput[i].command + player);
        if (keyBindingsDictionary == null)
            FillDictionary();
    }
    public void FillDictionary()
    {
        if (keyBindingsDictionary == null)
        {
            keyBindingsDictionary = new Dictionary<KeybindEnum, KeyCode>();
            foreach (var input in playerInput)
                keyBindingsDictionary.Add(input.command, input.keyCode);
        }
    }
    /*  -----------------------------------------------
       PRIVATE METHODS
        -----------------------------------------------*/
    private void CheckOverlappingKeybinds(KeyCode keyCode)
    {
        if (overlappingKeybindsChecker == null) { return; }

        for (int i = 0; i < overlappingKeybindsChecker.Length; i++)
        {
            for (int j = 0; j < overlappingKeybindsChecker[i].playerInput.Length; j++)
            {
                if (overlappingKeybindsChecker[i].playerInput[j].keyCode == keyCode)
                {
                    var command = overlappingKeybindsChecker[i].playerInput[j].command;
                    overlappingKeybindsChecker[i].SetKey(command, KeyCode.None, true); //3 param has to be set to true here, otherwise infinite loop
                    return;
                }
            }
        }
    }
}
