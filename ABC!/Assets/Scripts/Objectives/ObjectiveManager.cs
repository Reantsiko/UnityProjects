using UnityEngine;
using TMPro;
using System;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;

[Serializable]
public struct ObjectiveCollect
{
    public GameObject parentObject;
    public GameObject[] objectsToCollect;
    public int collected;
    public int collectAmount;
    public Collected collectedScript;
    public TMP_Text collectText;
}
[Serializable]
public struct CleanObjective
{
    public GameObject[] objectsToClean;
    public int cleaned;
    public int cleanAmount;
    public TMP_Text cleanText;
}

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private ObjectiveCollect[] _collectables = null;
    [SerializeField] private CleanObjective _cleanables;
    [SerializeField] private MenuHandler _mainMenu = null;
    [SerializeField] private FirstPersonController _fpc = null;
    [SerializeField] private TMP_Text _recordText = null;
    [SerializeField] private TMP_Text _timeText = null;
    [SerializeField] private CheckFloorTouch _floorTouch;
    [SerializeField] private float _waitTime = 3f;
    private void Start()
    {
        if (!_floorTouch)
            _floorTouch = FindObjectOfType<CheckFloorTouch>();
        InitializeCollectables();
        InitializeCleanables();
    }

    public void AddAmount(int toAdd, int type, int posInArray)
    {
        if (_floorTouch.GetGameOver()) return;

        switch (type)
        {
            case 0:
                _collectables[posInArray].collected += toAdd;
                if (_collectables[posInArray].collectText)
                    _collectables[posInArray].collectText.text = _collectables[posInArray].collected + "/" + _collectables[posInArray].collectAmount;
                break;
            case 1:
                _cleanables/*[posInArray]*/.cleaned += toAdd;
                if (_cleanables.cleanText)
                    _cleanables.cleanText.text = _cleanables.cleaned + "/" + _cleanables.cleanAmount;
                break;
        }

        if (IsCollectObjectiveDone() && IsCleanObjectiveDone())
            ShowVictoryScreen(true);
    }

    public void ShowVictoryScreen(bool victory)
    {
//        int i = 0;
        _floorTouch.SetLavaActive(false);
        _floorTouch.SetGameOver(true);
        /*while (i < _waitTime)
        {
            //turn off lava effect?
            //something.text = "All objectives completed, the level will end in " + _waitTime - i;
            yield return new WaitForSeconds(1f);
            i++;
        }*/
        var pos = SceneManager.GetActiveScene().buildIndex;
        float timer = Time.timeSinceLevelLoad - _fpc.moveTime;
        /*_fpc.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;*/
        _mainMenu.OpenEndLevelScreen(victory, "Everything has been cleaned up!");
        timer *= 100;
        //print(timer);
        timer = Mathf.Round(timer);
        //print(timer);
        timer /= 100;
        //print(timer);
        /*if (timer < LevelProgressSaver.instance.times[pos])
        {
            if (_recordText != null)
                _recordText.text = "!!! NEW RECORD !!!";
            LevelProgressSaver.instance.times[pos] = timer;
        }
        else
            _recordText.text = "Current record: " + LevelProgressSaver.instance.times[pos].ToString();*/
        //_timeText.text = "Time: " + timer.ToString() + " seconds";
        //LevelProgressSaver.instance.finished[pos] = 1;
        //LevelProgressSaver.instance.SaveProgress();
    }

    private void InitializeCollectables()
    {
        AddCollectibles();
        for (int i = 0; i < _collectables.Length; i++)
        {
            FillCollectables(i);
            _collectables[i].collectAmount = _collectables[i].objectsToCollect.Length;
            if (_collectables[i].collectedScript == null)
            {
                Debug.LogError("No reference to the goal on " + _collectables[i].parentObject);
                continue;
            }
            else
                _collectables[i].collectedScript.SetArrayPos(i);
            if (_collectables[i].collectText != null)
                _collectables[i].collectText.text = _collectables[i].collected + "/" + _collectables[i].collectAmount;
        }
    }

    private void AddCollectibles()
    {
        if (_collectables.Length == 0)
        {
            var am = FindObjectsOfType<Collected>();
            _collectables = new ObjectiveCollect[am.Length];
            for (int i = 0; i < am.Length; i++)
            {
                var collectable = am[i].GetComponentInChildren<Collectable>();
                _collectables[i].parentObject = collectable.gameObject;
                _collectables[i].collectedScript = collectable.GetGoal();
                _collectables[i].collectText = am[i].GetComponentInChildren<TMP_Text>();
            }
        }
    }

    private void FillCollectables(int i)
    {
        var objects = _collectables[i].parentObject.gameObject.GetComponentsInChildren<Transform>();
        var coll = _collectables[i].collectedScript;
        _collectables[i].objectsToCollect = new GameObject[objects.Length - 1];
        for (int j = 1; j < objects.Length; j++)
        {
            _collectables[i].objectsToCollect[j - 1] = objects[j].gameObject;
            _collectables[i].objectsToCollect[j - 1].GetComponent<InteractableObject>().collectedRef = coll;
        }
    }

    private void InitializeCleanables()
    {
        if (_cleanables.cleanText == null)
        {
            Debug.LogError($"No cleanable text canvas in scene or not linked!");
            return;
        }
        var dirtAm = FindObjectsOfType<Dirt>();
        if (dirtAm.Length == 0) return;

        var cleanUI = FindObjectOfType<CleanUI>();
        _cleanables.objectsToClean = new GameObject[dirtAm.Length];
        for (int i = 0; i < dirtAm.Length; i++)
            _cleanables.objectsToClean[i] = dirtAm[i].gameObject;
        _cleanables.cleanAmount = dirtAm.Length;
        _cleanables.cleanText = cleanUI.cleanedText;
        dirtAm[0]._worldUICanvas = cleanUI.gameObject;
        dirtAm[0].hasCanvas = true;
        cleanUI.transform.SetParent(dirtAm[0].transform);
        var canvasPos = _cleanables.objectsToClean[0].transform.position;
        cleanUI.gameObject.transform.position = new Vector3(canvasPos.x, canvasPos.y + 1, canvasPos.z);
        if (_cleanables.cleanText != null)
        {
            _cleanables.cleanAmount = _cleanables.objectsToClean.Length;
            _cleanables.cleanText.text = _cleanables.cleaned + "/" + _cleanables.cleanAmount;
        }
    }

    private bool IsCollectObjectiveDone()
    {
        if (_cleanables.cleaned != _cleanables.cleanAmount)
            return false;
        return true;
    }

    private bool IsCleanObjectiveDone()
    {
        foreach(var collectable in _collectables)
        {
            if (collectable.collected != collectable.collectAmount)
                return false;
        }
        return true;
    }

    public Transform SwitchDirtTransform()
    {
        foreach(var trans in _cleanables.objectsToClean)
        {
            if (trans.gameObject.activeSelf == true && !trans.GetComponent<Dirt>().hasCanvas)
                return trans.transform;
        }
        return null;
    }
}
