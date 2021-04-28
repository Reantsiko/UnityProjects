using System.Collections;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

/*
 * On the buttons in the inspector under "On Click()" press on the + sign and drag the object
 * on which this script is placed. Select the function SetButton and then drag the button into the slot under it.
 * You can also add a tag to the button, this has to be the same as the scriptable object's Player string,
 * to differentiate between keybindings in local co-op.
 * 
 * With the variable array keysToIgnore you can set input that you don't want to be bound to keys.
*/

public class KeybindHandler : MonoBehaviour
{
    [Tooltip("Put all of the created objects for keybindings here.")]
    public Keybindings[] playerKeybinds = null;
    public Slider[] mouseSensititivy;
    [Tooltip("Input here all of the prefabs that came with this package for the keybind changes in game.")]
    [SerializeField] private TMP_Text[] keyBindText = null;
    [Tooltip("Use this array to select which keys shouldn't be used a keybinds.")]
    [SerializeField] private KeyCode[] keysToIgnore = null;
    private Coroutine listeningForInput = null;

    void Start()
    {
        for (int i = 0; i < playerKeybinds.Length; i++)
        {
            playerKeybinds[i].LoadKeys();
            playerKeybinds[i].GetDefault().FillDictionary();
            mouseSensititivy[i].value = playerKeybinds[i].GetMouseSensitivity();
        }
        
        SetGameObjectTexts(true);
    }
    /*  -----------------------------------------------
        PUBLIC METHODS
        -----------------------------------------------*/
    /*
     * Depending on your menu setup you might have to change
     * pressedButton.GetComponentInChildren<T>(); to pressedButton.GetComponent<T>();
    */
    public void SetButton(Button pressedButton)
    {
        var keybindCommand = pressedButton.GetComponentInParent<KeybindCommand>();
        if (!keybindCommand)
        {
            Debug.LogError($"No KeybindCommand script on {pressedButton.name}");
            return;
        }

        //change this depending on how your button is set up
        var textComponent = pressedButton.GetComponentInChildren<TMP_Text>(); 
        if (!textComponent)
        {
            Debug.LogError("No text on button");
            return;
        }
        textComponent.text = "Press a key...";
        listeningForInput = StartCoroutine(ListenForKeyInput(keybindCommand.GetCommand(), pressedButton.tag));
    }

    public void SaveAll()
    {
        for (int i = 0; i < playerKeybinds.Length; i++)
        {
            playerKeybinds[i].SetMouseSensitivity(mouseSensititivy[i].value);
            playerKeybinds[i].SaveAll();
        }
        
    }

    /*
     * Use this method on buttons, or add it to the end of the IEnumerator ListenForKeyInput
     * to save automatically.
    */
    public void SaveKeys()
    {
        for (int i = 0; i < playerKeybinds.Length; i++)
            playerKeybinds[i].SaveKeys();
    }
    /*
     * Reset all of the keybindings to there default if a default set exists.
    */
    public void ResetKeys()
    {
        for (int i = 0; i < playerKeybinds.Length; i++)
            playerKeybinds[i].ResetKeys();
        SetGameObjectTexts(false);
    }

    public void SaveMouseSensitivity(int player)
    {
        playerKeybinds[player].SetMouseSensitivity(mouseSensititivy[player].value);
    }

    /*  -----------------------------------------------
        PRIVATE METHODS
        -----------------------------------------------*/
    /*
     * Listener method to change keybindings.
    */
    private IEnumerator ListenForKeyInput(KeybindEnum command, string p)
    {
        while (!Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (KeyCode keyValue in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyValue))
                {
                    if (IgnoreInput(keyValue))
                        continue;
                    GetCorrectPlayer(p, command, keyValue);
                    SetGameObjectTexts(false);
                    listeningForInput = null;
                    yield break;
                }
            }
            yield return new WaitForSecondsRealtime(0);
        }
        SetGameObjectTexts(false);
        listeningForInput = null;
    }

    /*
     * Returns true if an inputted key needs to be ignored. 
    */
    private bool IgnoreInput(KeyCode keyValue)
    {
        foreach (var ignore in keysToIgnore)
            if (keyValue == ignore)
                return true;
        return false;
    }

    private void GetCorrectPlayer(string p, KeybindEnum command, KeyCode keyValue)
    {
        var temp = playerKeybinds.ToList().FindIndex(playerIndex => string.Compare(playerIndex.GetPlayerIdentity(), p) == 0);
        if (temp != -1)
            playerKeybinds[temp].SetKey(command, keyValue);
    }
    /*
     * Updates the UI
     * This method can use some improvement.
    */
    private void SetGameObjectTexts(bool setCommandTexts)
    {
        for (int i = 0; i < playerKeybinds.Length; i++)
        {
            var playerInput = playerKeybinds[i].GetPlayerInput();
            var tempList = keyBindText.ToList().Where(t => t.tag == playerKeybinds[i].GetPlayerIdentity()).ToList();

            if (tempList.Count == 0)
            {
                Debug.LogError($"Length of found TMP_Text items is: {tempList.Count}");
                return;
            }

            for (int j = 0; j < tempList.Count; j++)
            {
                var command = tempList[j].GetComponent<KeybindCommand>().GetCommand();
                var buttonText = tempList[j].GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>();
                var comPos = FindCorrectCommand(command, playerInput);
                if (buttonText.CompareTag(playerKeybinds[i].GetPlayerIdentity()))
                {
                    if (comPos > -1 && setCommandTexts)
                        tempList[j].text = playerInput[comPos].commandName;
                    buttonText.text = playerKeybinds[i].GetKeyAsString(command);
                }
            }
            if (i < mouseSensititivy.Length)
                mouseSensititivy[i].value = playerKeybinds[i].GetMouseSensitivity();
        }
    }

    /*
     * Method to help organize the UI.
    */
    private int FindCorrectCommand(KeybindEnum toFind, InputKeys[] searchArea) => searchArea.ToList().FindIndex(a => a.command == toFind);
}
