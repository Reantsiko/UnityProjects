using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Pool : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsInPool = new List<GameObject>();
    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> objectSpawnList = new List<GameObject>();
    [SerializeField] private List<string> objectNames = new List<string>();
    [SerializeField] private GameObject objectToSpawn = null;
    [SerializeField] private int preSpawnAmount = 5;
    [SerializeField] private bool spawnFromList;
    [SerializeField] private bool preSpawn;
    [SerializeField] private Vector3 outOfSightSpawnPosition;
    public GameObject GetRandomFromList() => objectsInPool[Random.Range(0, objectsInPool.Count)];
    public GameObject GetFromList(int pos)
    {
        if (pos < objectsInPool.Count)
            return objectsInPool[pos];
        return default;
    }

    public GameObject GetFromList(bool isActive) => objectsInPool.Find(obj => obj.activeSelf == isActive);
    public GameObject GetFromList(bool isActive, string objName) => objectsInPool.Find(obj => string.Compare(obj.name, objName) == 0 && obj.activeSelf == isActive);
    public GameObject GetFromPrefabList(string objName) => objectSpawnList.Find(obj => string.Compare(obj.name, objName) == 0);

    private void Start()
    {
        if (objectNames.Count != objectSpawnList.Count)
        {
            objectNames.Clear();
            for (int i = 0; i < objectSpawnList.Count; i++)
                objectNames.Add(objectSpawnList[i].name);
        }
        if (preSpawn)
        {
            if (spawnFromList)
                PreSpawnFromList();
            else
                PreSpawnFromSingle();
        }
    }

    private GameObject SpawnFromList(string objName)
    {
        return default;
    }

    public GameObject SpawnFromSingle()
    {
        var temp = Instantiate(objectToSpawn, outOfSightSpawnPosition, Quaternion.identity, transform);
        temp.SetActive(false);
        objectsInPool.Add(temp);
        return temp;
    }

    public GameObject SpawnFromSingle(GameObject objToSpawn)
    {
        var temp = Instantiate(objToSpawn, outOfSightSpawnPosition, Quaternion.identity, transform);
        temp.SetActive(false);
        temp.name = objToSpawn.name;
        objectsInPool.Add(temp);
        return temp;
    }

    private void PreSpawnFromList()
    {
        foreach(var obj in objectSpawnList)
        {
            if (obj == null)
            {
                Debug.LogError($"Object in PreSpawnFromList is null, skipping to next!");
                continue;
            }
            for (int i = 0; i < preSpawnAmount; i++)
                SpawnFromSingle(obj);
        }
    }

    private void PreSpawnFromSingle()
    {
        if (objectToSpawn == null) return;
        for (int i = 0; i < preSpawnAmount; i++)
            SpawnFromSingle();
    }

    public void Spawn(Vector3 spawnPos)
    {
        var temp = GetFromList(false);//objectsInPool.Find(obj => obj.activeSelf == false);
        if (temp == null)
            InstantiateNew(spawnPos);
        else
        {
            temp.transform.position = spawnPos;
            temp.SetActive(true);
        }
    }
    
    public void Spawn(Vector3 spawnPos, string objName)
    {
        var temp = GetFromList(false, objName);
        if (temp == null)
            InstantiateNew(spawnPos, objName);
        else
        {
            temp.transform.position = spawnPos;
            temp.SetActive(true);
        }
    }

    private void InstantiateNew(Vector3 spawnPos, string objName = null)
    {
        GameObject temp = null;
        if (string.IsNullOrEmpty(objName))
            temp = Instantiate(objectToSpawn, spawnPos, Quaternion.identity, transform);
        else
        {
            var spawnObj = GetFromPrefabList(objName);
            temp = Instantiate(spawnObj, spawnPos, Quaternion.identity, transform);
            temp.name = objName;
        }
        if (temp != null)
            objectsInPool.Add(temp);
    }
}
