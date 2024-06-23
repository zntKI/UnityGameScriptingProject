using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class RandomEnemyMovement : MonoBehaviour
{
    public static RandomEnemyMovement Instance => instance;
    static RandomEnemyMovement instance;

    public static event Action OnPlayerCaught;

    NavMeshAgent agent;
    enum EnemyState
    {
        Patrolling, Targeting, Retreating
    }
    EnemyState state;


    public Transform targetIndicator;

    [Header("Waypoints")]

    [SerializeField]
    List<Transform> waypoints; // TODO: Find them at run-time
    List<Transform> tempRemovedWaypoints = new List<Transform>();

    [Header("Patroling")]

    [SerializeField]
    float minPatrolingRange;
    [SerializeField]
    float maxPatrolingRange;


    [Header("Targeting")]

    [SerializeField]
    float targetRange;

    Transform player;


    [Header("Retreating")]

    [SerializeField]
    float minRetreatRange;
    [SerializeField]
    float maxRetreatRange;

    [Header("TimePhaseVariables")]

    [SerializeField]
    float fasterMoveSpeedAddAmount;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            throw new InvalidOperationException("There can only be one Enemy in the scene!");

        TimeManager.OnTimePhaseChangeToMid += ChangeMovementSpeed;
        TimeManager.OnTimePhaseChangeToGameOver += SetStateToTargeting;
    }

    void Start()
    {
        SetState(EnemyState.Patrolling);

        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerMovement>().transform;
    }

    void SetState(EnemyState newState)
    {
        state = newState;
        Color debugColor;
        switch (state)
        {
            case EnemyState.Targeting:
                debugColor = Color.red; break;
            case EnemyState.Patrolling:
                debugColor = Color.blue; break;
            case EnemyState.Retreating:
                debugColor = Color.green; break;
            default:
                debugColor = Color.white; break;
        }
        GetComponent<MeshRenderer>().material.color = debugColor;
    }

    void Update()
    {
        switch (state)
        {
            case EnemyState.Patrolling:
                HandlePatroling();
                break;
            case EnemyState.Targeting:
                HandleTargeting();
                break;
            case EnemyState.Retreating:
                HandleRetreating();
                break;
            default:
                break;
        }
    }

    void HandleRetreating()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
            SetState(EnemyState.Patrolling);
    }

    void HandleTargeting()
    {
        agent.SetDestination(player.position);
        if (Input.GetMouseButtonDown(0)) // TODO: Expand when knifes are implemented
        {
            SetState(EnemyState.Retreating);
            player.GetComponent<NavMeshObstacle>().enabled = true; // Make an event instead

            Vector3 point = RandomPoint(transform.position, minRetreatRange, maxRetreatRange);
            agent.SetDestination(point);
            targetIndicator.position = point;
        }
        else if (agent.remainingDistance <= agent.stoppingDistance) // Player DEAD
        {
            Debug.Log("Player has been caught!");
            OnPlayerCaught?.Invoke();
        }
    }

    void HandlePatroling()
    {
        if (agent.remainingDistance <= agent.stoppingDistance) // Done with path
        {
            Vector3 point = RandomPoint(transform.position, minPatrolingRange, maxPatrolingRange);
            agent.SetDestination(point);
            targetIndicator.position = point;
        }
        else // Check for player detection
        {
            Vector3 deltaVec = player.position - transform.position;
            if (deltaVec.magnitude <= targetRange
                && Vector3.Dot(transform.forward, deltaVec) > 0)
            // maybe raycast too
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, deltaVec.normalized, out hit, targetRange) && hit.collider.CompareTag("Player")) // Check if there is nothing between the player and the enemy
                {
                    SetState(EnemyState.Targeting);
                }
            }
        }
    }

    Vector3 RandomPoint(Vector3 center, float minRange, float maxRange)
    {
        // Re-adds waypoints if there are no more left
        if (waypoints.Count == 0)
        {
            waypoints.AddRange(tempRemovedWaypoints);
            tempRemovedWaypoints.Clear();
        }

        // Gets all the waypoints that satisfy the given condition
        List<Transform> validWaypoints;
        do
        {
            validWaypoints = waypoints.Where(w => (w.position - transform.position).magnitude > minRange && (w.position - transform.position).magnitude < maxRange).ToList();
            maxRange += 0.1f; // If there are no waypoints that satisfy the conditions broaden the search
        }
        while (validWaypoints.Count == 0);

        // Gets a random waypoint from the valid waypoints
        int rndIndex = UnityEngine.Random.Range(0, validWaypoints.Count);
        Transform rndWaypoint = validWaypoints[rndIndex];

        // Removing waypoints from the list to reduce randomness in the movement, therefore prevent staying mostly at only one area of the level
        waypoints.Remove(rndWaypoint);
        tempRemovedWaypoints.Add(rndWaypoint);


        int countOut = 0;

        float waypointRadius = rndWaypoint.GetComponent<DebugDrawCircleRange>().Radius;
        Vector3 randomPoint;
        while (true)
        {
            randomPoint = rndWaypoint.position + UnityEngine.Random.insideUnitSphere * waypointRadius;

            countOut++;
            if (countOut > 1000)
            {
                throw new System.Exception("Error: infinite while loop. Cannot find point on nav mesh");
            }

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
                //or add a for loop like in the documentation
                return hit.position;
            }
        }
    }

    void ChangeMovementSpeed()
    {
        agent.speed += fasterMoveSpeedAddAmount;
        Debug.Log($"Changed enemy speed to {agent.speed}");
    }

    void SetStateToTargeting()
    {
        SetState(EnemyState.Targeting);
    }

    void OnDestroy()
    {
        TimeManager.OnTimePhaseChangeToMid -= ChangeMovementSpeed;
        TimeManager.OnTimePhaseChangeToGameOver -= SetStateToTargeting;
    }
}