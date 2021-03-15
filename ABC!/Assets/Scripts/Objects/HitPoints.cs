using UnityEngine;

public class HitPoints : MonoBehaviour
{
    [SerializeField] private int hitPoints = 1;
    [SerializeField] private bool infiniteUses;
    public void SetHitPoints(int toSet) { hitPoints = toSet; }
    public int GetHitPoints() { return hitPoints; }

    public void ChangeHitPointsAmount(int change)
    {
        if (infiniteUses) { return; }
        hitPoints += change;
    }
}
