using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionHandler : MonoBehaviour
{
    public List<UnitInfo> unitList;
    //public List<Building> buildingList;
    
    void Start()
    {
        unitList = new List<UnitInfo>();
        //buildingList = new List<Building>();
    }
}
