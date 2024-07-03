using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeToThrow : MonoBehaviour
{
    public static event Action<Vector3> OnHit;
    public static event Action OnEnemyHit;

    [SerializeField]
    float speed = 5f;

    [Header("Sound")]

    [SerializeField]
    AudioClip knifeThrowSound;
    [SerializeField]
    AudioClip knifeWoodHitSound;
    [SerializeField]
    AudioClip knifeHitEnemyPatrolingSound;
    [SerializeField]
    AudioClip knifeHitEnemyTargetingSOund;

    AudioSource audioSource;

    Rigidbody rb;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

        audioSource.clip = knifeThrowSound;
        audioSource.Play();
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
                            audioSource.clip = knifeHitEnemyPatrolingSound;
                            audioSource.Play();
                            break;
                        case EnemyState.Targeting:
                            audioSource.clip = knifeHitEnemyTargetingSOund;
                            audioSource.Play();
                            break;
                        default:
                            break;
                    }
                    OnEnemyHit?.Invoke();
                    break;
                default:
                    Debug.Log("Knife hit wood");

                    audioSource.clip = knifeWoodHitSound;
                    audioSource.Play();

                    OnHit?.Invoke(other.transform.position);
                    break;
            }

            rb.velocity = Vector3.zero;
            GetComponent<MeshRenderer>().enabled = false;
            Invoke(nameof(DestroyAfterSoundIsPlayed), audioSource.clip.length);
        }
        else
            Destroy(gameObject);
    }

    void DestroyAfterSoundIsPlayed()
    {
        Destroy(gameObject);
    }
}
