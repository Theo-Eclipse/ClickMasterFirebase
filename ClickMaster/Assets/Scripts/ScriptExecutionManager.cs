using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptExecutionManager : MonoBehaviour
{
    [SerializeField] private UIController uIController;
    [SerializeField] private AuthManager authManager;
    void Start()
    {
        // Used to manage script execution order from script.
        uIController.Init();
       // authManager.Init();
    }
}
