using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptExecutionManager : MonoBehaviour
{
    void Start()
    {
        // Used to manage script execution by their order in Hierarchy
        Debug.Log($"Program starde. Initializing {transform.childCount} scripts.");
        for (int i = 0; i < transform.childCount; i++) 
        {
            var script = transform.GetChild(i).GetComponent<IExecutionManager>();
            if (script != null) script.Init();
        }
    }
}
