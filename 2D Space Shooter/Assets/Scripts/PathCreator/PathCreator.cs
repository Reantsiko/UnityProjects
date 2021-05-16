using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PathCreator : MonoBehaviour
{
    public static PathCreator instance;
    [SerializeField] private int minPoints = 2;
    [SerializeField] private int maxPoints = 7;
    [SerializeField] private int maxPaths = 50;
    [SerializeField]
    [Range(0f,1f)] private float returnSameSideChance = 0.25f;
    [SerializeField] private Transform topSide;
    [SerializeField] private Transform bottomSide;
    [SerializeField] private Transform[] columns;
    [SerializeField] private HashSet<List<Transform>> generatedPathing = new HashSet<List<Transform>>();

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < maxPaths; i++)
        {
            var generatedPath = GeneratePath();
            generatedPathing.Add(generatedPath);
        }
    }

    private List<Transform> GeneratePath()
    {
        List<Transform> path = new List<Transform>();
        var wayPointAmount = Random.Range(minPoints, maxPoints + 1);
        var left = topSide.GetComponentsInChildren<Transform>();
        var right = bottomSide.GetComponentsInChildren<Transform>();
        var startLeft = Random.Range(0, 2) == 0 ? true : false;
        path.Add(startLeft == true ? left[Random.Range(0, left.Length)] : right[Random.Range(0, right.Length)]);
        for (int i = 0; i < wayPointAmount - 2; i++)
        {
            var selectedColumn = columns[Random.Range(0, columns.Length)];
            var transformArr = selectedColumn.GetComponentsInChildren<Transform>();
            path.Add(transformArr[Random.Range(0, transformArr.Length)]);
        }
        var returnSameSide = Random.Range(0f, 1f) <= returnSameSideChance ? true : false;
        if (returnSameSide && wayPointAmount > 2)
            path.Add(startLeft == true ? left[Random.Range(0, left.Length)] : right[Random.Range(0, right.Length)]);
        else
            path.Add(startLeft == false ? left[Random.Range(0, left.Length)] : right[Random.Range(0, right.Length)]);
        return path;
    }

    public List<Transform> GetGeneratedPath() => generatedPathing?.ToList()[Random.Range(0, generatedPathing.Count)];
}
