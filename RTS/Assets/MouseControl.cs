using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    public SelectionHandler selectionHandler;

    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;
    void Start()
    {
        if (selectionHandler == null)
            selectionHandler = FindObjectOfType<SelectionHandler>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            CastRay();
    }

    private void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        startPos = ray.origin;
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            var unitComp = hit.collider.GetComponent<UnitInfo>();
            if (unitComp)
                selectionHandler.unitList.Add(unitComp);
            else
                selectionHandler.unitList.Clear();
        }
    }
}
