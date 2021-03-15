using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractableObject : MonoBehaviour
{
    public Transform dst = null;
    [SerializeField] private bool _isCollectible = false;
    [SerializeField] LayerMask toSet;
    [SerializeField] HitPoints hp;
    private bool _isBeingDragged;
    [SerializeField] private float _distanceFromStart;
    private Vector3 _startPos;
    private Rigidbody _rb = null;
    private DifficultySettings _settings;
    [HideInInspector] public Collected collectedRef = null;
    [HideInInspector] private int _layer;
    [HideInInspector]Coroutine changeLayer = null;

    public float GetDistanceFromStart() { return _distanceFromStart; }
    // Start is called before the first frame update
    void Start()
    {
        if (!hp)
            hp = GetComponent<HitPoints>();
        /*if (_isCollectible)
            gameObject.layer = toSet;*/
        _layer = gameObject.layer.GetHashCode();
        //gameObject.tag = "Interactable";
        if (!_rb)
            _rb = GetComponent<Rigidbody>();
        if (!dst)
            dst = GameObject.Find("Hold Pos").transform;
        if (!_settings)
            _settings = FindObjectOfType<DifficultySettings>();
        _startPos = transform.position;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void Update()
    {

        if (_isBeingDragged) 
        {
            //_rb.velocity = Vector3.zero;
            _rb.MovePosition(dst.position); 
        }
        //transform.position = dst.position;
        if (!_isCollectible)
        {
            _distanceFromStart = (_startPos - transform.position).magnitude;
            if (_distanceFromStart < _settings.GetSettings(Difficulty.normal).messTreshhold)
                _distanceFromStart = 0;
            else
                _distanceFromStart = (_startPos - transform.position).magnitude - _settings.GetSettings(Difficulty.normal).messTreshhold;
        }
        
    }

    public void SetFreeze(bool toSet)
    {
        _rb.freezeRotation = toSet;
    }

    public void PickUp()
    {
        if (hp && hp.GetHitPoints() <= 0) return;
        if (changeLayer != null)
            StopCoroutine(changeLayer);
        //transform.parent = dst.transform;
        _isBeingDragged = true;
        _rb.useGravity = false;
        SetFreeze(true);
    }

    public void Drop()
    {
        //transform.parent = null;
        _isBeingDragged = false;
        _rb.useGravity = true;
        if (hp)
            hp.ChangeHitPointsAmount(-1);
        SetFreeze(false);
        changeLayer = StartCoroutine(ChangeLayer());
    }

    public void SetVelocity(Vector3 dir, float strength)
    {
        //print(dir);
        _rb.velocity = dir * strength;
        Drop();
    }

    IEnumerator ChangeLayer()
    {
        yield return new WaitForSeconds(.2f);
        gameObject.layer = _layer;
    }
}
