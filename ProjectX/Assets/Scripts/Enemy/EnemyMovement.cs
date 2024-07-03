using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class RandomEnemyMovement : MonoBehaviour
{
    // TODOs:
    //  Make the enemy switch state to patrolling when having lost sight of the player 

    public static RandomEnemyMovement Instance => instance;
    static RandomEnemyMovement instance;

    public static event Action OnPlayerCaught;

    public static event Action OnEnemyTargeting;
    public static event Action OnEnemyRetreating;
    public static event Action OnEnemyDistracted;

    public EnemyState State => state;
    EnemyState state;

    NavMeshAgent agent;

    public Transform targetIndicator;

    List<Transform> waypoints;
    List<Transform> tempRemovedWaypoints;

    [Header("Patroling")]

    [SerializeField]
    float minPatrolingRange = 5f;
    [SerializeField]
    float maxPatrolingRange = 15f;


    [Header("Targeting")]

    [SerializeField]
    float targetRange = 10f;
    [SerializeField]
    float targetingSpeedAddAmount = 5f;

    Transform player;


    [Header("Retreating")]

    [SerializeField]
    float minRetreatRange = 20f;
    [SerializeField]
    float maxRetreatRange = 30f;
    [SerializeField]
    float retreatingSpeedAddAmount = 10f;


    [Header("KnifeReaction")]

    [SerializeField]
    float maxHearingDistance = 5f;


    [Header("TimePhaseVariables")]

    [SerializeField]
    float fasterMoveSpeedAddAmount = 5f;


    EnemySoundHandler soundHandler;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            throw new InvalidOperationException("There can only be one Enemy in the scene!");

        TimeManager.OnTimePhaseChangeToMid += ChangeMovementSpeed;
        TimeManager.OnTimePhaseChangeToGameOver += SetStateToTargeting;

        KnifeToThrow.OnEnemyHit += DamageEnemy;
        KnifeToThrow.OnHit += AddressKnifeHit;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = InputHandler.Player.transform;

        waypoints = GameObject.FindGameObjectsWithTag("Waypoint").Select(w => w.transform).ToList();
        if (waypoints.Count == 0)
            throw new InvalidOperationException("No waypoints found for the Enemy to follow!");
        tempRemovedWaypoints = new List<Transform>();

        SetState(EnemyState.Patrolling);

        soundHandler = transform.GetChild(0).GetComponent<EnemySoundHandler>();
    }

    void SetState(EnemyState newState)
    {
        // Undo changes made in the previous state setting
        switch (state)
        {
            case EnemyState.Patrolling:
                break;
            case EnemyState.Targeting:
                agent.speed -= targetingSpeedAddAmount;
                break;
            case EnemyState.Retreating:
                agent.speed -= retreatingSpeedAddAmount;
                break;
            default:
                break;
        }

        state = newState;
        Color debugColor;
        switch (state)
        {
            case EnemyState.Patrolling:
                player.GetComponent<NavMeshObstacle>().enabled = false;

                debugColor = Color.blue;
                break;
            case EnemyState.Targeting:
                agent.speed += targetingSpeedAddAmount;

                OnEnemyTargeting?.Invoke();

                debugColor = Color.red;
                break;
            case EnemyState.Retreating:
                Vector3 point = RandomPoint(minRetreatRange, maxRetreatRange);
                agent.SetDestination(point);
                targetIndicator.position = point;

                agent.speed += retreatingSpeedAddAmount;

                OnEnemyRetreating?.Invoke();

                player.GetComponent<NavMeshObstacle>().enabled = true;

                debugColor = Color.green;
                break;
            default:
                debugColor = Color.white;
                break;
        }
        //GetComponent<MeshRenderer>().material.color = debugColor;
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
        if (agent.remainingDistance <= agent.stoppingDistance) // Player DEAD
        {
            Debug.Log("Player has been caught!");
            OnPlayerCaught?.Invoke();
        }
    }

    void HandlePatroling()
    {
        if (agent.remainingDistance <= agent.stoppingDistance) // Done with path
        {
            Vector3 point = RandomPoint(minPatrolingRange, maxPatrolingRange);
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

    //int count = 0;
    Vector3 RandomPoint(float minRange, float maxRange)
    {
//        Debug.Log("In");
//        count++;
//        if (count >= 10)
//        {
//            Debug.Log("ShouldQuit");
//#if UNITY_EDITOR
//            EditorApplication.isPlaying = false;
//#endif
//        }

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


        float waypointRadius = rndWaypoint.GetComponent<DebugDrawCircleRange>().Radius;
        return NavMeshSamplePoint(rndWaypoint.position, waypointRadius);
    }

    Vector3 NavMeshSamplePoint(Vector3 center, float samplingRadius=1f)
    {
        int countOut = 0;

        Vector3 randomPoint;
        while (true)
        {
            randomPoint = center + UnityEngine.Random.insideUnitSphere * samplingRadius;

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

    /// <summary>
    /// Called on event fired because time phase changed to Mid
    /// </summary>
    void ChangeMovementSpeed()
    {
        agent.speed += fasterMoveSpeedAddAmount;
        Debug.Log($"Changed enemy speed to {agent.speed}");
    }

    /// <summary>
    /// Called on event fired because time phase changed to GameOver
    /// </summary>
    void SetStateToTargeting()
    {
        Debug.Log($"GameOver - Enemy chasing the Player!");
        //agent.speed += fasterMoveSpeedAddAmount;
        SetState(EnemyState.Targeting);
    }

    /// <summary>
    /// Called on event fired because a knife hit the Enemy
    /// </summary>
    void DamageEnemy()
    {
        switch (state)
        {
            case EnemyState.Patrolling:
                Debug.Log("Hit enemy while patrolling");

                soundHandler.PlayHitWhilePatrolingSound();

                SetState(EnemyState.Targeting);
                break;
            case EnemyState.Targeting:
                Debug.Log("Hit enemy while targeting");

                soundHandler.PlayHitWhileTargetingSound();

                SetState(EnemyState.Retreating);
                break;
            default:
                break;
        }
    }

    void AddressKnifeHit(Vector3 hitPos)
    {
        if (state == EnemyState.Patrolling
            && Vector3.Distance(hitPos, transform.position) <= maxHearingDistance)
        {
            Debug.Log("Enemy distracted");

            OnEnemyDistracted?.Invoke();

            Vector3 point = NavMeshSamplePoint(hitPos);
            agent.SetDestination(point);
            targetIndicator.position = point;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == player)
        {
            Debug.Log($"Player has triggered targeting mode by bumping into the enemy");
            SetState(EnemyState.Targeting);
        }
    }

    void OnDestroy()
    {
        TimeManager.OnTimePhaseChangeToMid -= ChangeMovementSpeed;
        TimeManager.OnTimePhaseChangeToGameOver -= SetStateToTargeting;

        KnifeToThrow.OnEnemyHit -= DamageEnemy;
        KnifeToThrow.OnHit -= AddressKnifeHit;
    }
}

public enum EnemyState
{
    Patrolling, Targeting, Retreating
}