using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private Collected goal;

    public Collected GetGoal() { return goal; }

    private void Start()
    {
        if (!goal)
            goal = GetComponentInParent<Collected>();
    }
}
