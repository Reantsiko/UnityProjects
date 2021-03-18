using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RefreshGames : MonoBehaviour
{
    [Header("Refresh Objects")]
    [SerializeField] private int _refreshWaitTime = 10;
    [SerializeField] private Button _refreshButton = null;
    [SerializeField] private GameObject _roomInfoPrefab = null;
    [SerializeField] private Transform _viewportTransform = null;
    private List<GameObject> _createdRooms = new List<GameObject>();

    public void RefreshList()
    {
        StartCoroutine(RefreshCooldown());
        foreach (var room in _createdRooms)
            Destroy(room);
        _createdRooms.Clear();
        int i = 0;
        foreach (var matchID in MatchMaker.instance.matchList.Keys)
        {
            if (MatchMaker.instance.matchList.ContainsKey(matchID) && 
                MatchMaker.instance.hasGameStarted.ContainsKey(matchID) && 
                !MatchMaker.instance.hasGameStarted[matchID] && 
                MatchMaker.instance.matchList[matchID].isPublic)
            {
                var obj = Instantiate(_roomInfoPrefab, _viewportTransform);
                _createdRooms.Add(obj);
                obj.transform.localPosition = new Vector2(690f, -72f - (160f * i));
                var matchInfo = obj.GetComponent<FillMatchInfo>();
                matchInfo.matchID = matchID;
                matchInfo.UpdateTexts();
                i++;
            }
        }
    }

    private IEnumerator RefreshCooldown()
    {
        _refreshButton.interactable = false;
        for (int i = 0; i < _refreshWaitTime; i++)
            yield return new WaitForSeconds(1f);
        _refreshButton.interactable = true;
    }
}
