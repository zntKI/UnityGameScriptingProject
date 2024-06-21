using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class EnemySideDetector : MonoBehaviour
{
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }


    private void OnTriggerEnter(Collider other)
    {
        Vector3 enemyRelativeToDoor = transform.InverseTransformPoint(other.transform.position);



        Debug.Log("Door being triggered by "+ other.name + " Direction forward through door? " + (enemyRelativeToDoor.x >0));
        anim.SetBool("IsOpen", true);
        anim.SetInteger("OpenAnim", (enemyRelativeToDoor.x > 0)?1:2);


    }

    private void OnTriggerExit(Collider other)
    {
        anim.SetBool("IsOpen", false);
        anim.SetInteger("OpenAnim", 0);
    }
}
