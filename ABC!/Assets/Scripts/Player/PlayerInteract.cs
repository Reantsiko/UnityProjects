using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float _range = 5f;
    [SerializeField] public float strength = 5f;
    [SerializeField] private bool _drawRange = true;
    //[SerializeField] GameObject cam = null;
    [SerializeField] private GameObject _itemDrag = null;
    [SerializeField] private GameObject _itemToClean = null;
    [SerializeField] private Image crosshair = null;
    [SerializeField] private Keybindings keybindings = null;
    [SerializeField] private GameObject _cleanOverlay = null;
    [SerializeField] private LayerMask _interactable = 0;
    [HideInInspector] private Coroutine coroutine = null;

    private void Start()
    {
        if (_cleanOverlay)
            _cleanOverlay.SetActive(false);
        //cam = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        CastRay();
        if (Input.GetKeyUp(keybindings.GetKeyCode(KeybindEnum.pickup)))
        {
            if (_itemDrag)
            {
                DropItem();
            }
            if (coroutine != null)
            {
                StopAndClearCoRoutine();
            }
        }
        if (Input.GetKeyDown(keybindings.GetKeyCode(KeybindEnum.throwitem)) && _itemDrag)
        {
            _itemDrag.GetComponent<InteractableObject>().SetVelocity(Camera.main.transform.forward, strength);
            _itemDrag = null;
        }

        if (_itemToClean != null)
        {
            float distance = (_itemToClean.transform.position - transform.position).magnitude;
            if (distance > 2f && coroutine != null)
            {
                StopAndClearCoRoutine();
                //print("Too far away.");
            }
        }
    }

    private void StopAndClearCoRoutine()
    {
        StopCoroutine(coroutine);
        coroutine = null;
        if (_cleanOverlay)
            _cleanOverlay.SetActive(false);
        _itemToClean = null;
    }

    void CastRay()
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, _range, _interactable);
        if (hit.collider /*&& (hit.collider.CompareTag("Interactable") || hit.collider.CompareTag("Dirt"))*/)
        {
            var hp = hit.collider.GetComponent<HitPoints>();
            if (hp != null && hp.GetHitPoints() <= 0) return;

            crosshair.color = Color.green;

            if (Input.GetKeyDown(keybindings.GetKeyCode(KeybindEnum.pickup)) /*&& hit.collider*/ && hit.collider.CompareTag("Interactable"))
            {
                hit.collider.GetComponent<InteractableObject>().PickUp();
                hit.collider.gameObject.layer = 10;//15;
                _itemDrag = hit.collider.gameObject;
                //print("Touching collectable " + hit.collider);
            }
            if (Input.GetKeyDown(keybindings.GetKeyCode(KeybindEnum.pickup)) /*&& hit.collider*/ && hit.collider.CompareTag("Dirt"))
            {
                _itemToClean = hit.collider.gameObject;
                coroutine = StartCoroutine(_itemToClean.GetComponent<Dirt>().CleanDirt());
                //print("Touching dirt " + hit.collider);
            }
        }
        else
        {
            crosshair.color = Color.red;
        }
    }

    void DropItem()
    {
        _itemDrag.GetComponent<InteractableObject>().Drop();
        _itemDrag = null;
    }

    private void OnDrawGizmos()
    {
        if (_drawRange)
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * _range, Color.red);
    }
}
