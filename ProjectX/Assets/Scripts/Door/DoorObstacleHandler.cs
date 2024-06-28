using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class DoorObstacleHandler : MonoBehaviour
{
    DoorControl doorControl;

    NavMeshObstacle meshObstacle;

    MeshCollider normalCollider;

    void Awake()
    {
        TimeManager.OnTimePhaseChangeToEnd += DisableObstacleComponent;
    }

    void Start()
    {
        meshObstacle = GetComponent<NavMeshObstacle>();
        doorControl = transform.parent.GetComponent<DoorControl>();

        normalCollider = GetComponent<MeshCollider>();
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

    void OnTriggerEnter(Collider other)
    {
        if (!meshObstacle.isActiveAndEnabled
            && other.CompareTag("Enemy")
            && doorControl.IsOpen)
        {
            normalCollider.enabled = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!meshObstacle.isActiveAndEnabled
            && other.CompareTag("Enemy")
            && doorControl.IsOpen)
        {
            normalCollider.enabled = true;
        }
    }

    void OnDestroy()
    {
        TimeManager.OnTimePhaseChangeToEnd -= DisableObstacleComponent;
    }
}
