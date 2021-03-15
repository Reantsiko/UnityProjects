using UnityEngine;

public class DisablePowerUp : MonoBehaviour
{
    [SerializeField] private DifficultySettings _settings;
    void Start()
    {
        if (!_settings)
            _settings = FindObjectOfType<DifficultySettings>();
        if (!_settings.GetHasPowerups())
            gameObject.SetActive(false);
    }
}
