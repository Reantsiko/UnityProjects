using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupPickup : MonoBehaviour
{
    [SerializeField] private float value = 0.5f;
    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("Strength"))
            other.GetComponentInParent<Magic>().IncreaseStrengthValue(value);
        else if (gameObject.CompareTag("Shield"))
            other.GetComponentInParent<Magic>().IncreaseShieldValue(value);
        gameObject.SetActive(false);
    }
}
