using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InfiniteGameLoopDetector : MonoBehaviour
{
    float startTime;
    float maxExecutionTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.realtimeSinceStartup - startTime > maxExecutionTime)
        {
            Debug.LogWarning("Max execution time exceeded. Stopping play mode to prevent infinite loop.");

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }
    }
}
