using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IExecutionManager
{
    public static PlayerController instance { get; private set; }
    public User user { get; private set; }
    private UserListElement user_element;
    public void Init()
    {
        instance = this;
        Debug.Log("Player Controller Initialization...");
    }

    public void SetCurrentUserInstance() 
    {
        if (LeaderboardManager.UserExists(AuthManager.instance.firebaseUser.UserId))
        {
            user = LeaderboardManager.GetUser(AuthManager.instance.firebaseUser.UserId);
            user_element = LeaderboardManager.GetLeaderboardElement(AuthManager.instance.firebaseUser.UserId);
            UpdateUI();
        }
    }

    public void Click() 
    {
        user.clicks_last_session++;
        UpdateUI();
    }

    private void UpdateUI() 
    {
        user_element.SetData();
        UIController.instance.ButtonClickCounter.text = user.clicks_last_session.ToString();
        LeaderboardManager.instance.Sort();
    }
}
