using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    private WaveConfiguration waveConfig = null;
    [SerializeField]private List<Transform> waypoints = null;
    [SerializeField] private int waypointIndex = 0;

    public void SetWaveConfiguration(WaveConfiguration toSet) => waveConfig = toSet;
    private void Start()
    {
        waypoints = waveConfig.GetWayPoints();
        transform.position = waypoints[waypointIndex].position;
    }
    private void Update() => MoveToNextWaypoint();
    private void MoveToNextWaypoint()
    {
        if (transform.position == waypoints[waypointIndex].position)
            waypointIndex++;
        if (waypointIndex <= waypoints.Count - 1)
        {
            float movement = waveConfig.GetMoveSpeed() * Time.deltaTime;
            Vector2 wp = waypoints[waypointIndex].position;
            transform.position = Vector2.MoveTowards(transform.position, wp, movement);
        }
        else
        {
            GetComponent<WordDisplay>().RemoveWordFromList();
            Destroy(gameObject);
        }
    }
}
