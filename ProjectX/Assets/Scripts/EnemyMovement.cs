using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomEnemyMovement : MonoBehaviour
{
    NavMeshAgent agent;
    enum EnemyState
    {
        Patrolling, Targeting, Retreating
    }
    EnemyState state = EnemyState.Patrolling;


    public Transform targetIndicator;

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

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerMovement>().transform;
    }

    // Update is called once per frame
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

            Vector3 point = RandomPoint(transform.position, minRetreatRange, maxRetreatRange);
            agent.SetDestination(point);
            targetIndicator.position = point;
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
                SetState(EnemyState.Targeting);
            }
        }
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
                debugColor = Color.yellow; break;
            case EnemyState.Retreating:
                debugColor = Color.green; break;
            default:
                debugColor = Color.white; break;
        }
        GetComponent<MeshRenderer>().material.color = debugColor;
    }

    Vector3 RandomPoint(Vector3 center, float minRange, float maxRange)
    {
        // TODO: add iteration count to prevent infinite while loops
        int countOut = 0;
        while (true)
        {
            bool extraConditionDependingOnState;

            int countIn = 0;
            Vector3 randomPoint; // Random point in a sphere within the max range
            Vector3 deltaVec;
            do
            {
                randomPoint = center + Random.insideUnitSphere * maxRange; // Generate new point if it is within the maxRange
                deltaVec = randomPoint - center;

                switch (state)
                {
                    case EnemyState.Patrolling:
                        extraConditionDependingOnState = Vector3.Dot(transform.forward, deltaVec) < 0; // If the point is in the opposite direction (opposite to below)
                        break;
                    case EnemyState.Retreating:
                        extraConditionDependingOnState = Vector3.Dot(deltaVec.normalized, player.transform.forward) < 0.8f; // If the point is in the same direction (opposite to above)
                        break;
                    default:
                        extraConditionDependingOnState = false;
                        break;
                }

                countIn++;
                if (countIn > 1000)
                {
                    throw new System.Exception("Error: infinite while loop. Cannot find point satisfying special condition");
                }
            }
            while (deltaVec.magnitude < minRange || //If it is within the minRange
                    extraConditionDependingOnState);

            countOut++;
            if (countOut > 1000)
            {
                throw new System.Exception("Error: infinite while loop. Cannot find point on mav mesh");
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
}