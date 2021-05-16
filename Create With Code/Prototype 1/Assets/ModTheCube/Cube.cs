using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public float xBorder = 20f;
    public float zBorder = 10f;
    public float yLimit = -20f;
    public float rotateSpeed = 15f;
    Vector3 rotateVector;
    public float maxScale = 5f;
    void Start()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-xBorder, xBorder), Random.Range(0f, yLimit), Random.Range(-zBorder, zBorder));
        transform.position = spawnPos;
        rotateVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        transform.localScale = new Vector3(Random.Range(1f, maxScale), Random.Range(1f, maxScale), Random.Range(1f, maxScale));
        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(.5f, 1f));
        GetComponent<MeshRenderer>().sharedMaterial.color = color;
    }
    
    void Update()
    {
        transform.Rotate(rotateVector * rotateSpeed * Time.deltaTime);
    }
}
