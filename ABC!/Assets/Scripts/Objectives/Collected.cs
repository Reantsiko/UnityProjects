using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Collected : MonoBehaviour
{
    public ObjectiveManager objectiveManager = null;
    [HideInInspector] private int arrayPos;
    public TMP_Text collectableText;

    public int GetArrayPos() { return arrayPos; }
    public void SetArrayPos(int toSet) { arrayPos = toSet; }

    private void Start()
    {
        if (!objectiveManager)
            objectiveManager = FindObjectOfType<ObjectiveManager>();
        if (!collectableText)
            collectableText = GetComponentInChildren<TMP_Text>();
    }
    private void OnTriggerEnter(Collider other)
    {
        var coll = other.gameObject.GetComponent<InteractableObject>();
        if (coll.collectedRef != null && coll.collectedRef == this)
            objectiveManager.AddAmount(1, 0, arrayPos);
    }

    private void OnTriggerExit(Collider other)
    {
        var coll = other.gameObject.GetComponent<InteractableObject>();
        if (coll.collectedRef != null && coll.collectedRef == this)
            objectiveManager.AddAmount(-1, 0, arrayPos);
    }
}
