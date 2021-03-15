using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropdownSetter : MonoBehaviour
{
    private TMP_Dropdown dropdown;

    private void OnEnable()
    {
        if (!dropdown)
            dropdown = GetComponent<TMP_Dropdown>();
        dropdown.value = ProgressTracker.difficulty.GetHashCode();
    }
}
