using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class LeaderboardManager : MonoBehaviour, IExecutionManager
{
    public static LeaderboardManager instance { get; private set; }
    [Header("List Container")]
    [SerializeField] private RectTransform ListContainer;
    [SerializeField] private UserListElement CurrentPlayer_ListElement;
    private Dictionary<string, UserListElement> UserID_ListElement = new Dictionary<string, UserListElement>();

    public void Init()
    {
        instance = this;
        Debug.Log("Leaderboard Manager Initialization...");
    }

    public static bool UserExists(string user_id) => instance.UserID_ListElement.ContainsKey(user_id);

    public static User GetUser(string user_id) => instance.UserID_ListElement[user_id].user;

    public static UserListElement GetLeaderboardElement(string user_id) => instance.UserID_ListElement[user_id];

    private void UpdateUserClicks(string user_id, int clicks)
    {
        if (UserID_ListElement.ContainsKey(user_id)) 
        {
            UserID_ListElement[user_id].user.clicks_last_session = clicks;
            UpdateUserListElement(user_id);
            Sort();
        }
    }

    public void UpdateUserListElement(string user_id)
    {
        UserID_ListElement[user_id].SetData();
        Sort();
    }

    public void CreateNewUserListElement(User user)
    {
        if (UserID_ListElement.Count == 0)
        {
            CurrentPlayer_ListElement.user = user;
            CurrentPlayer_ListElement.SetData();
            UserID_ListElement.Add(user.user_id, CurrentPlayer_ListElement);
        }
        else
        {
            UserID_ListElement.Add(user.user_id, NewListElement(user));
        }
        Sort();
    }

    public void CreateRandomUser()// Used only in the editor for test purposes.
    {
        User new_random_user = new User();
        new_random_user.user_id = Random.Range(1000000, 9999999).ToString();
        new_random_user.username = NameGenerator.GetName();
        new_random_user.fullname = "Random Userfield";
        new_random_user.email = "Test@lolz.com";
        new_random_user.clicks_last_session = Random.Range(100, 9999);
        new_random_user.user_color = $"#{ColorUtility.ToHtmlStringRGB(Color.HSVToRGB(Random.Range(0.0000f, 1.0000f), 0.27f, 1.0f))}";
        UserID_ListElement.Add(new_random_user.user_id, NewListElement(new_random_user));
    }
    
    private UserListElement NewListElement(User user)
    {
        UserListElement new_user = Instantiate(CurrentPlayer_ListElement.gameObject, ListContainer).GetComponent<UserListElement>();
        new_user.gameObject.SetActive(true);
        new_user.user = user;
        new_user.SetData();
        return new_user;
    }

    public void Sort() 
    {
        UserID_ListElement = UserID_ListElement.OrderByDescending((u) => u.Value.user.clicks_last_session).ThenBy((u) => u.Value.user.username).ToDictionary(k => k.Key, v => v.Value);
        int index = 1;
        foreach (var user in UserID_ListElement)
        {
            user.Value.OverrideLeaderboardPlace(index);
            user.Value.transform.SetAsLastSibling();
            index++;
        }
        CurrentPlayer_ListElement.OverrideShownName($"{CurrentPlayer_ListElement.user.username} <color=grey>(You)</color>");
    }

    private void OnEnable() => Sort();
}
