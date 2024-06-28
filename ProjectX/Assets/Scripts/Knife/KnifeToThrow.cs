using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeToThrow : MonoBehaviour
{
    public static event Action OnKnifeLauch; // If there was a knife to throw and the launch is successful

    public static event Action<Vector3> OnHit;
    public static event Action OnEnemyHit; // Different states in order to play different sounds on hit (change if the sound ends up being the same)

    [SerializeField]
    float speed = 5f;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.velocity = transform.forward * speed;

        OnKnifeLauch?.Invoke();
    }

    /// <summary>
    /// Knife colliding with other collider
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Knife destroyed");

        if (TimeManager.TimePhase != TimePhase.GameOver) // Do not address knife hit if game is in the final phase and the Enemy is chasing the player
        {
            switch (other.tag)
            {
                case "Enemy":
                    switch (RandomEnemyMovement.Instance.State)
                    {
                        case EnemyState.Patrolling:
                            OnEnemyHit?.Invoke();
                            break;
                        case EnemyState.Targeting:
                            OnEnemyHit?.Invoke();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    OnHit?.Invoke(other.transform.position);
                    break;
            }
        }

        Destroy(gameObject);
    }
}
