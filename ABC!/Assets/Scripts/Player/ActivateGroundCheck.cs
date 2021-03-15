using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActivateGroundCheck : MonoBehaviour
{
    public GameObject groundCheck = null;
    [SerializeField] private float maxWaitTime = 1f;
    [SerializeField] private GameObject _floorUI = null;
    [SerializeField] private TMP_Text _floorText = null;
    [SerializeField] private GameObject groundObj = null;
    [SerializeField] private Renderer groundRenderer = null;
    // Ground but Lava
    public Material GroundNormal = null;
    public Material GroundButLava = null;
    
    private void Start()
    {
        if (!groundCheck)
            groundCheck = GameObject.Find("RespawnCheck");
        _floorText.text = "The floor will become lava in: " + maxWaitTime;
        _floorText.enabled = false;
        _floorUI.SetActive(false);
        groundCheck.SetActive(false);
        if (!groundObj)
            groundObj = GameObject.Find("Ground");
        groundRenderer = groundObj.GetComponent<Renderer>();

          // Ground but NO Lava
        GroundNormal = Resources.Load("Ground", typeof(Material)) as Material;
        GroundButLava = Resources.Load("Ground_but_lava", typeof(Material)) as Material;
        groundRenderer.GetComponent<Renderer>().material = GroundNormal;
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            groundCheck.SetActive(!groundCheck.activeSelf);
    }
#endif
    public IEnumerator FloorWillBecomeLava()
    {
        _floorUI.SetActive(true);
        _floorText.enabled = true;
        for (float i = maxWaitTime; i > 0; i--)
        {
            _floorText.text = "The floor will become lava in: " + i;
            yield return new WaitForSeconds(1);
        }
        _floorUI.SetActive(false);
        groundCheck.SetActive(true);

        groundRenderer.GetComponent<Renderer>().material = GroundButLava;
    }
}
