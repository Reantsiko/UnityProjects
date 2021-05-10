using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
public class Player : MonoBehaviour
{
    public string userName = null;
    public string displayName = null;
    public TMP_Text playerNameText = null;
    public float lastCommandTime;
    public bool isOnline = true;
    public ActiveAction activeAction = ActiveAction.idle;
    public PlayerStats playerStats = null;
    public PlayerClass playerClass = null;
    public PlayerJob playerJob = null;
    Pathfinder pathfinder;
    delegate IEnumerator PlayerActions();
    Dictionary<ActiveAction, PlayerActions> playerActions;
    public void UpdateNameText()
    {
        if (playerNameText == null)
            playerNameText = GetComponentInChildren<TMP_Text>();
        if (isOnline)
            playerNameText.text = $"<color=green>{displayName}</color>";
        else
            playerNameText.text = $"<color=red>{displayName}</color>";
    }

    public void InitializePlayer()
    {
        playerStats = new PlayerStats(1, 5, 2, 10, 2, 2);
        var classAmount = System.Enum.GetValues(typeof(PClass)).Length;
        playerClass = new PlayerClass((PClass)Random.Range(0, classAmount));
        pathfinder = GetComponent<Pathfinder>();
        pathfinder.SetupPathfinder();
        pathfinder.idleCoroutine = StartCoroutine(pathfinder.IdleRoutine());
        playerActions = new Dictionary<ActiveAction, PlayerActions>();
        playerActions.Add(ActiveAction.idle, pathfinder.IdleRoutine);
    }

    public void SetJob(PJob job)
    {
        playerJob = new PlayerJob(job);
        playerActions.Add(ActiveAction.work, pathfinder.ExploreMap);
    }

    private IEnumerator GainXP()
    {
        while (true)
        {
            playerClass.GainXP(500);
            playerJob.GainXP(200);
            yield return new WaitForSeconds(2f);
        }
    }

    public void StartIdle()
    {
        if (!playerActions.ContainsKey(ActiveAction.idle))
            playerActions.Add(ActiveAction.idle, pathfinder.IdleRoutine);
        if (pathfinder.idleCoroutine != null)
        {
            StopCoroutine(pathfinder.idleCoroutine);
            pathfinder.idleCoroutine = null;
        }
        pathfinder.idleCoroutine = StartCoroutine(playerActions[ActiveAction.idle]());
    }

    public void StartJob()
    {
        if (!playerActions.ContainsKey(ActiveAction.work)) return;
        StartCoroutine(playerActions[ActiveAction.work]());
    }
}
