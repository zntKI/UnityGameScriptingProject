using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorObstacleHandler : MonoBehaviour
{
    NavMeshObstacle meshObstacle;
    DoorControl doorControl;

    void Awake()
    {
        TimeManager.OnTimePhaseChangeToEnd += DisableObstacleComponent;
    }

    void Start()
    {
        meshObstacle = GetComponent<NavMeshObstacle>();
        doorControl = transform.parent.GetComponent<DoorControl>();
    }

    void DisableObstacleComponent()
    {
        meshObstacle.enabled = false;
        Debug.Log("Disabled door's nav mesh obstacle component");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!meshObstacle.isActiveAndEnabled
            && collision.gameObject.CompareTag("Enemy")
            && !doorControl.IsOpen)
        {
            doorControl.HandleDoorInteraction();
        }
    }

    void OnDestroy()
    {
        TimeManager.OnTimePhaseChangeToEnd -= DisableObstacleComponent;
    }
}
