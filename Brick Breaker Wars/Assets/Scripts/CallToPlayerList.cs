using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
public class CallToPlayerList : MonoBehaviour
{
    //[SerializeField] public SyncListGameObject playerObjects = new SyncListGameObject();
    [SerializeField] public string matchID = null;
    [SerializeField] Scene scene;
    // Start is called before the first frame update
    private void Awake()
    {
        scene = gameObject.scene;
    }

    void Start()
    {
        if (!Player.localPlayer.thisServer)
        {
            //PrepClient();

            //server.AddPlayerForConnection(Player.localPlayer.GetComponent<NetworkIdentity>());
            
            Player.localPlayer.AddToInGameList(scene);
            Player.localPlayer.CmdPlayerLoaded(true);
            //Player.localPlayer.SetServer(server);
            //Player.localPlayer.GiveServerMatch();
        }
        else
        {
            Debug.Log($"Scene handler of scene on server: {scene.handle}");
            MatchMaker.instance.gameHandler.GiveUseableScene(scene);
            //StartCoroutine(StartCountdown());
        }

    }

    /*[Command]
    public void AddPlayer(GameObject playerToAdd)
    {
        playerObjects.Add(playerToAdd);
    }*/

    private IEnumerator StartCountdown()
    {
        for (int i = 5; i >= 0; i--)
        {
            Debug.Log($"{i} seconds before game starts!");
            yield return new WaitForSeconds(1f);
        }
        Debug.Log($"Game has started");
    }
}
