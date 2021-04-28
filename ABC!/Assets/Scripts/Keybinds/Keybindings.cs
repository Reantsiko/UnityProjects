using System;
using System.Linq;
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
    [SerializeField] private Keybindings defaultKeybindings = null;
    [Tooltip("Input all the other KeyBindingsScriptableObject that you want to be cross checked for overlapping input.")]
    [SerializeField] private Keybindings[] overlappingKeybindsChecker = null;
    private Dictionary<KeybindEnum, KeyCode> keyBindingsDictionary = null;

    /*  -----------------------------------------------
           PUBLIC METHODS
        -----------------------------------------------*/
    /*
     * Use this method with Input.GetKey(...);
    */
    public KeyCode GetKeyCode(KeybindEnum command) => keyBindingsDictionary[command];
    /*
     * Use this method to set the value of keys as text. eg in options menu
    */
    public string GetKeyAsString(KeybindEnum command) => keyBindingsDictionary[command].ToString();
    /*
     * Used to differentiate between multiple players in local co-op
    */
    public string GetPlayerIdentity() => player;
    public InputKeys[] GetPlayerInput() => playerInput; 
    public void SetMouseSensitivity(float toSet) =>  sensitivity = toSet;
    public float GetMouseSensitivity() => sensitivity;
    public Keybindings GetDefault() => defaultKeybindings;
    /*
     * Will change the KeyCode in the array and dictionary.
    */
    public void SetKey(KeybindEnum command, KeyCode toSet, bool reset = false)
    {
        var commandPosition = playerInput.ToList().FindIndex(k => k.command == command);
        if (keyBindingsDictionary.ContainsKey(command) == false || commandPosition <= -1)
        {
            Debug.LogError($"Error when trying to set the key of command {command}.");
            return;
        }
        if (!reset)
            CheckOverlappingKeybinds(toSet);
        playerInput[commandPosition].keyCode = toSet;
        keyBindingsDictionary[command] = toSet;
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

        playerInput.ToList().ForEach(ik => SetKey(ik.command, defaultKeybindings.GetKeyCode(ik.command), true));
    }
    /*
     * Saving and loading of the keybindings through PlayerPrefs.
     * The keys will be saved under the name of the variable + the player number, in order to differentiate between
     * multiple players in local co-op.
    */
    public void SaveKeys()
    {
        PlayerPrefs.SetString("Keybinds " + player, "Saved");
        playerInput.ToList().ForEach(k => PlayerPrefs.SetInt(k.command + player, k.keyCode.GetHashCode()));
        if (keyBindingsDictionary == null)
            FillDictionary();
        PlayerPrefs.Save();
    }

    public void SaveMouseSensitivity()
    {
        if (useMouseSensitivity)
        {
            PlayerPrefs.SetFloat($"MS {player}", sensitivity);
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
        if (!PlayerPrefs.HasKey($"Keybinds { player}"))
        {
            SaveAll();
            Debug.Log("Created new save keys");
            return;
        }

        if (useMouseSensitivity && PlayerPrefs.HasKey($"MS {player}"))
            sensitivity = PlayerPrefs.GetFloat($"MS {player}");
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
            var index = overlappingKeybindsChecker[i].playerInput.ToList().FindIndex(k => k.keyCode == keyCode);
            if (index > -1)
            {
                var command = overlappingKeybindsChecker[i].playerInput[index].command;
                overlappingKeybindsChecker[i].SetKey(command, KeyCode.None, true); //3rd param has to be set to true here, otherwise infinite loop
                return;
            }
        }
    }
}
