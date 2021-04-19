using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PowerUpUI : MonoBehaviour
{
    public static PowerUpUI instance;
    [SerializeField] private Gradient chargeGradient;
    [Header("Bomb Power Up")]
    [SerializeField] private float bombRechargeTimer = 90f;
    [SerializeField] private Slider[] bombSliders = null;
    [SerializeField] public WordDisplay bombWordDisplay = null;
    [SerializeField]private int bombAmount = -1;
    private Coroutine bombCharge = null;
    [Header("Shield Power Up")]
    [SerializeField] private float shieldRechargeTimer = 90f;
    [SerializeField] private Slider[] shieldSliders = null;
    [SerializeField] public WordDisplay shieldWordDisplay = null;
    [SerializeField] private int shieldAmount = -1;
    private Coroutine shieldCharge = null;
    private WordManager wordManager = null;

    private void Awake()
    {
        instance = this;
        wordManager = FindObjectOfType<WordManager>();
    }

    private void Start()
    {
        bombCharge = StartCoroutine(SliderCharge(bombSliders?[bombAmount + 1], bombRechargeTimer, true));
        shieldCharge = StartCoroutine(SliderCharge(shieldSliders?[shieldAmount + 1], shieldRechargeTimer, false));
    }

    private IEnumerator SliderCharge(Slider slider, float rechargeTime, bool isBomb, int startVal = 1)
    {
        if (slider == null) yield return new WaitForEndOfFrame();

        if (slider.gameObject.activeSelf == false)
            slider.gameObject.SetActive(true);
        var img = slider.GetComponentInChildren<Image>();
        for (int i = startVal; i <= rechargeTime; i++)
        {
            slider.value = i / rechargeTime;
            img.color = chargeGradient.Evaluate(slider.value);
            yield return new WaitForSeconds(1f);
        }
        if (isBomb)
            bombWordDisplay.text = slider.GetComponentInChildren<TMP_Text>();
        else
            shieldWordDisplay.text = slider.GetComponentInChildren<TMP_Text>();
        CheckNextCharge(isBomb);
    }

    private void CheckNextCharge(bool isBomb)
    {
        if (isBomb)
        {
            bombAmount++;
            if (bombAmount + 1 < bombSliders.Length)
            {
                bombCharge = StartCoroutine(SliderCharge(bombSliders[bombAmount + 1], bombRechargeTimer, true));
            }
            else
                bombCharge = null;
        }
        else
        {
            shieldAmount++;
            if (shieldAmount + 1 < shieldSliders.Length)
            {       
                shieldCharge = StartCoroutine(SliderCharge(shieldSliders[shieldAmount + 1], shieldRechargeTimer, false));
            }
            else
                shieldCharge = null;
        }
    }

    public void UseBomb()
    {
        if (bombAmount >= 0)
        {
            int val = 1;
            if (bombCharge != null)
            {
                StopCoroutine(bombCharge);
                val = (int)(bombSliders[bombAmount + 1].value * bombRechargeTimer);
                bombSliders[bombAmount + 1].value = 0f;
                bombSliders[bombAmount + 1].gameObject.SetActive(false);
            }
            if (bombAmount > 0)
                bombWordDisplay.text = bombSliders[bombAmount - 1].GetComponentInChildren<TMP_Text>();
            wordManager.Bomb();
            bombAmount--;
            Debug.Log($"{bombAmount} bombs left!");
            bombWordDisplay.word.wordTyped = false;
            bombCharge = StartCoroutine(SliderCharge(bombSliders[bombAmount + 1], bombRechargeTimer, true, val));
        }
    }

    public void UseShield()
    {
        if (shieldAmount >= 0)
        {
            int val = 1;
            if (shieldCharge != null)
            {
                StopCoroutine(shieldCharge);
                val = (int)(shieldSliders[shieldAmount + 1].value * shieldRechargeTimer);
                shieldSliders[shieldAmount + 1].value = 0f;
                shieldSliders[shieldAmount + 1].gameObject.SetActive(false);
            }
            if (shieldAmount > 0)
                shieldWordDisplay.text = shieldSliders[shieldAmount - 1].GetComponentInChildren<TMP_Text>();
            shieldAmount--;
            PlayerHealth.instance.ActivateShield();
            shieldWordDisplay.word.wordTyped = false;
            shieldCharge = StartCoroutine(SliderCharge(shieldSliders[shieldAmount + 1], shieldRechargeTimer, false, val));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            UseBomb();
        if (Input.GetKeyDown(KeyCode.DownArrow))
            UseShield();
    }

    public void StopBombCharging()
    {
        if (bombCharge != null)
        {
            StopCoroutine(bombCharge);
            bombCharge = null;
        }
    }
    public void StopShieldCharging()
    {
        if (bombCharge != null)
        {
            StopCoroutine(shieldCharge);
            bombCharge = null;
        }
    }

    public int GetBombAmount() => bombAmount;
    public int GetShieldAmount() => shieldAmount;
}
