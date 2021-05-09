using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class HideOnStart : MonoBehaviour
{
    void Start() => gameObject.SetActive(false);
}
