using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(BoxCollider))]
public class Dirt : MonoBehaviour
{
    [SerializeField] private List<GameObject> _dirtObjects = null;
    [SerializeField] private int currentDirt = 0;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private ObjectiveManager objectiveManager = null;
    //[HideInInspector] public int arrayPos;

    [SerializeField] private GameObject _cleanOverlay = null;
    [SerializeField] private Image _cleanSlider = null;
    [SerializeField] private MenuHandler _menuHandler;

    
    [SerializeField] public GameObject _worldUICanvas = null;
    [SerializeField] public bool hasCanvas;
    private bool _sent;

    
    private void Start()
    {
        //gameObject.tag = "Dirt";
        
        if (!objectiveManager)
            objectiveManager = FindObjectOfType<ObjectiveManager>();


        if (_worldUICanvas != null)
            hasCanvas = true;
        if (!_menuHandler)
            _menuHandler = FindObjectOfType<MenuHandler>();
        if (!_cleanOverlay)
            _cleanOverlay = _menuHandler.GetCleanOverlay();
        if (!_cleanSlider && _cleanOverlay != null)
            _cleanSlider = _cleanOverlay.GetComponentInChildren<Image>();
        GetDirtPieces();
    }

    public IEnumerator CleanDirt()
    {
        _menuHandler.ActivateCleanOverlay(true);
        _menuHandler.FillCleanedAmount((float)currentDirt / (float)_dirtObjects.Count);
        _cleanOverlay.SetActive(true);
        _cleanSlider.fillAmount = (float)currentDirt / (float)_dirtObjects.Count;
        //print(_cleanSlider.fillAmount);
        while (Input.GetMouseButton(0) && currentDirt < _dirtObjects.Count)
        {
            yield return new WaitForSeconds(waitTime);
            _dirtObjects[currentDirt].SetActive(false);
            currentDirt++;
            _menuHandler.FillCleanedAmount((float)currentDirt / (float)_dirtObjects.Count);
            //_cleanSlider.fillAmount = (float)currentDirt / (float)_dirtObjects.Count;
            //print(_cleanSlider.fillAmount);
        }
        if (currentDirt >= _dirtObjects.Count && !_sent)
        {
            _sent = true;
            objectiveManager.AddAmount(1, 1, /*arrayPos*/0);
        }
        _menuHandler.FillCleanedAmount(0);
        _menuHandler.ActivateCleanOverlay(false);
        //_cleanSlider.fillAmount = 0;
        //_cleanOverlay.SetActive(false);
        MoveWorldUI();
        gameObject.SetActive(false);
    }

    private void MoveWorldUI()
    {
        if (hasCanvas)
        {
            var obj = objectiveManager.SwitchDirtTransform();
            if (obj != null)
            {
                _worldUICanvas.transform.SetParent(obj);
                var objDirt = obj.GetComponent<Dirt>();
                objDirt.hasCanvas = true;
                objDirt._worldUICanvas = _worldUICanvas;
                objDirt._worldUICanvas.transform.localPosition = new Vector3(0f, objDirt._worldUICanvas.transform.position.y, 0f);
            }
        }
    }

    private void GetDirtPieces()
    {
        var am = GetComponentsInChildren<RandomRotation>();
        _dirtObjects = new List<GameObject>();
        foreach (var a in am)
        {
            _dirtObjects.Add(a.gameObject);
        }
    }
}
