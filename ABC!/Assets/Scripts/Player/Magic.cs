using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Magic : MonoBehaviour
{
    [SerializeField] private PlayerInteract _playerInteract = null;
    [SerializeField] private GameObject _groundCheck = null;
    [SerializeField] private float _invulnerableTimer = 3f;
    [SerializeField] private float _strengthTimer = 3f;
    [SerializeField] private Slider _invulSlider = null;
    [SerializeField] private Slider _strSlider = null;
    [SerializeField] private Keybindings _keybinds = null;
    private float strength = 0f;
    [HideInInspector] private Coroutine _activeShield = null;
    [HideInInspector] private Coroutine _activeStr = null;

    private void Start()
    {
        if (!_playerInteract)
            _playerInteract = FindObjectOfType<PlayerInteract>();
        if (!_groundCheck)
            _groundCheck = GameObject.Find("GroundCheck");
        _invulSlider.maxValue = _invulnerableTimer;
        _strSlider.maxValue = _strengthTimer;
        strength = _playerInteract.strength;
    }
    void Update()
    {
        if (_activeShield == null && Input.GetKeyDown(_keybinds.GetKeyCode(KeybindEnum.shield)) && _invulSlider.value > 0)
        {
            //print("starting invul");
            _activeShield = StartCoroutine(Invulnerable());
        }
        else if (_activeShield != null && Input.GetKeyDown(_keybinds.GetKeyCode(KeybindEnum.shield)))
        {
            StopCoroutine(_activeShield);
            _groundCheck.SetActive(true);
            _activeShield = null;
        }
        if (_activeStr == null && Input.GetKeyDown(_keybinds.GetKeyCode(KeybindEnum.strength)) && _strSlider.value > 0)
        {
            _activeStr = StartCoroutine(Strength());
        }
        else if (_activeStr != null && Input.GetKeyDown(_keybinds.GetKeyCode(KeybindEnum.strength)))
        {
            StopCoroutine(_activeStr);
            _playerInteract.strength = strength;
            _activeStr = null;
        }
    }

    IEnumerator Invulnerable()
    {
        _invulSlider.enabled = true;
        _groundCheck.SetActive(false);
        for (float i = _invulSlider.value; i > 0; i -= .1f)
        {
            //print(i);
            _invulSlider.value -= .1f;
            yield return new WaitForSeconds(.1f);
        }
        //print("activating groundcheck");
        _invulSlider.enabled = false;
        //_activeShield = null;
        _groundCheck.SetActive(true);
    }

    IEnumerator Strength()
    {
        _strSlider.enabled = true;
        _playerInteract.strength = strength * 2;
        for (float i = _strSlider.value; i >= 0; i -= .1f)
        {
            _strSlider.value -= .1f;
            yield return new WaitForSeconds(.1f);
        }
        _invulSlider.enabled = false;
        _playerInteract.strength = strength;
    }

    public void IncreaseStrengthValue(float toAdd)
    {
        if (_strSlider.value + toAdd >= _strSlider.maxValue)
            _strSlider.value = _strSlider.maxValue;
        else
            _strSlider.value += toAdd;
    }

    public void IncreaseShieldValue(float toAdd)
    {
        if (_invulSlider.value + toAdd >= _invulSlider.maxValue)
            _invulSlider.value = _invulSlider.maxValue;
        else
            _invulSlider.value += toAdd;
    }
}
