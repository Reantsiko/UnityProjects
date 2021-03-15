using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class RespawnCheck : MonoBehaviour
{
    //change this with raycast
    [SerializeField] private MenuHandler _mainMenu = null;
    public bool checkForGround = true;
    [SerializeField] private FirstPersonController _fpc = null;
    private void OnTriggerEnter(Collider other)
    {
        if (!checkForGround) { return; }
        if (other.gameObject.CompareTag("Ground"))
        {
            _fpc.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            _mainMenu.OpenMenu("You Lost");
        }
    }
}
