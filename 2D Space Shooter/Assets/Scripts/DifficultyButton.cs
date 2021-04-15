using UnityEngine;
using UnityEngine.UI;
public class DifficultyButton : MonoBehaviour
{
    [SerializeField] private Difficulty difficulty;
    [SerializeField] private string playerPrefKey = "highest";
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey(playerPrefKey)) return;
        bool interactable = PlayerPrefs.GetInt(playerPrefKey) >= difficulty.GetHashCode() ? true : false;
        GetComponent<Button>().interactable = interactable;
    }

    
}
