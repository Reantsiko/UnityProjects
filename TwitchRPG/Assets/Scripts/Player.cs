using UnityEngine;
using TMPro;
using System.Collections;

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
        playerJob = new PlayerJob(PJob.none);
        //StartCoroutine(GainXP());
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
}
