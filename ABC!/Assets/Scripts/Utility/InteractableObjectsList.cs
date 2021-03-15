using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  
 *  Creates list of all interactable objects that the player can use as platforms.
 *  Can be used to increase amount of times objects can be thrown.
*/
public class InteractableObjectsList : MonoBehaviour
{
    [SerializeField] private List<HitPoints> objects;
    void Start()
    {
        var objectsInScene = FindObjectsOfType<HitPoints>();
        foreach (var hp in objectsInScene)
        {
            objects.Add(hp);
        }
    }

    public void IncreaseHitPointsAllObjects()
    {
        foreach (var hp in objects)
        {
            hp.ChangeHitPointsAmount(1);
        }
    }

    public List<HitPoints> GetList() { return objects; }
}
