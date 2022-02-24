using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClicksDataPusher : MonoBehaviour, IExecutionManager
{
    public static ClicksDataPusher instance;
    [Range(0.5f, 10.0f)] public float PushIntervalSeconds = 2;
    private float updateTimer = 0;
    private int LastClicksAmountSent = 0;

    public void Init()
    {
        instance = this;
        Debug.Log("Clicks Data Pusher Initialization...");
    }

    // Update is called once per frame
    private void Update()
    {
        if (updateTimer == 0) return;
        else if(updateTimer > 0 && Time.time >= updateTimer && LastClicksAmountSent != PlayerController.instance.user.clicks_last_session)
            PushUpdate();
    }

    private void PushUpdate() 
    {
        LastClicksAmountSent = PlayerController.instance.user.clicks_last_session;
        updateTimer = Time.time + PushIntervalSeconds;
        StartCoroutine(DatabaseManager.instance.PushClicksUpdate());
    }

    public void SetEnable(bool enable) 
    {
        LastClicksAmountSent = PlayerController.instance.user.clicks_last_session;
        if (enable) 
            updateTimer = Time.time + PushIntervalSeconds;     
        else updateTimer = 0;
    }
}
