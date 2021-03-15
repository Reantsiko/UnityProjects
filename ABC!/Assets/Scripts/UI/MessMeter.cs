using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessMeter : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private InteractableObjectsList objectList;
    [SerializeField] private float value;
    void Start()
    {
        if (!objectList)
            objectList = FindObjectOfType<InteractableObjectsList>();
        StartCoroutine(UpdateSlider());
    }

    private IEnumerator UpdateSlider()
    {
        while (true)
        {
            value = 0;
            foreach (var o in objectList.GetList())
            {
                value += o.GetComponent<InteractableObject>().GetDistanceFromStart();
            }
            slider.value = value;
            yield return new WaitForEndOfFrame();
        }
    }

}
