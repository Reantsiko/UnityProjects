using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SetVolume : MonoBehaviour
{
    [Tooltip("Set to true for music, set to false for sound effect.")]
    [SerializeField] private bool isMusic;
    [SerializeField] private Slider slider = null;
    [SerializeField] private string key = null;
    [SerializeField] private TMP_Text percentageText = null;

    private void OnEnable()
    {
        if (!ErrorCheck())
            return;
        if (PlayerPrefs.HasKey(key))
            slider.value = PlayerPrefs.GetFloat(key);
        else
            slider.value = 1f;
        UpdateSoundHandlerValue();
        SetPercentageText();
    }

    public void ChangeVolumeValue()
    {
        if (!ErrorCheck())
            return;
        PlayerPrefs.SetFloat(key, slider.value);
        SetPercentageText();
        UpdateSoundHandlerValue();
    }

    private bool ErrorCheck()
    {
        if (slider == null || string.IsNullOrEmpty(key))
        {
            Debug.LogError($"Value of slider or key is null!");
            return false;
        }
        return true;
    }

    private void SetPercentageText()
    {
        if (percentageText != null)
            percentageText.text = $"{Mathf.Round(slider.value * 100)}%";
    }

    private void UpdateSoundHandlerValue()
    {
        if (SoundHandler.instance == null) return;
        if (isMusic)
            SoundHandler.instance.ChangeMusicVolume(slider.value);
        else
            SoundHandler.instance.ChangeSFXVolume(slider.value);
    }
}
