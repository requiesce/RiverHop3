using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{   
    public List<GameObject> waypoints;
    private NavMeshAgent agent;
    private const float WP_THRESHOLD = 6.0f;
    private GameObject currentWP;
    private int currentWPindex = -1;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentWP = GetNextWaypoint();
    }


    GameObject GetNextWaypoint()
    {
        currentWPindex++;
        if (currentWPindex == waypoints.Count)
        {
            currentWPindex = 0;
        }
        return waypoints[currentWPindex];
    }


    private void Update()
    {
        if (Vector3.Distance(transform.position, currentWP.transform.position) < WP_THRESHOLD)
        {
            currentWP = GetNextWaypoint();
            agent.SetDestination(currentWP.transform.position /*waypoints[2].transform.position*/);
        }
    }
}
