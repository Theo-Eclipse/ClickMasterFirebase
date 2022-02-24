using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using TMPro;
using UnityEngine.Events;
using System;

public class DatabaseManager : MonoBehaviour, IExecutionManager
{
    public static DatabaseManager instance { get; private set; }

    [Header("Firebase")]
    public DatabaseReference DBRef;

    public void Init()
    {
        instance = this;
        Debug.Log("Database Manager Initialization...");
    }

    public IEnumerator LoadLeaderboardData()
    {
        //var DBTask = DBRef.Child("users").OrderByChild("clicks_last_session").LimitToFirst(enteries_amount).GetValueAsync(); <- Version for limited amount of data.
        var DBTask = DBRef.Child("users").OrderByChild("clicks_last_session").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogErrorFormat($"Couldn't get leaderboard data!: {DBTask.Exception.Message}");
            Debug.Log($"Couldn't get leaderboard data!: {DBTask.Exception.Message}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            foreach (DataSnapshot childsnapshot in snapshot.Children) 
            {
                Debug.Log($"Loaded Leaderboard Entery: {childsnapshot.Key.ToString()}");
                string user_id = childsnapshot.Key.ToString();
                UpdateDataFromSnapshot(user_id, childsnapshot);
            }
        }
    }

    public void UpdateDataFromSnapshot(string user_id, DataSnapshot snapshot) 
    {
        if (SnapshotHasData(snapshot))
        {
            if (LeaderboardManager.UserExists(user_id))
                UpdateLeaderboardUserFromSnapshot(user_id, snapshot);
            else
                CreateLeaderboardUserFromSnapshot(user_id, snapshot);
        }
    }

    private void UpdateLeaderboardUserFromSnapshot(string user_id, DataSnapshot snapshot) 
    {
        User user = LeaderboardManager.GetUser(user_id);
        SnapshotToUser(user, snapshot);
        LeaderboardManager.instance.UpdateUserListElement(user_id);
    }

    private void CreateLeaderboardUserFromSnapshot(string user_id, DataSnapshot snapshot)
    {
        User user = new User();
        user.user_id = user_id;
        SnapshotToUser(user, snapshot);
        LeaderboardManager.instance.CreateNewUserListElement(user);
    }

    public void CreateLeaderboardAndUserdata(string user_id, string fullname, UnityAction onCompleted)
    {
        User new_userdata = CreateNewUserData(user_id, fullname);
        LeaderboardManager.instance.CreateNewUserListElement(new_userdata);
        StartCoroutine(CreateUserData(new_userdata, onCompleted));
    }

    private void SnapshotToUser(User user, DataSnapshot snapshot) 
    {
        if (!snapshot.HasChild("username"))
            return;
        user.username = snapshot.Child("username").Value.ToString();
        user.fullname = snapshot.Child("fullname").Value.ToString();
        user.email = snapshot.Child("email").Value.ToString();
        //user.avatar_url = snapshot.Child("avatar_url").Value.ToString();
        user.user_color = snapshot.Child("user_color").Value.ToString();
        user.clicks_last_session = int.Parse(snapshot.Child("clicks_last_session").Value.ToString());
    }

    private bool SnapshotHasData(DataSnapshot snapshot) 
    {
        Debug.Log($"Snapshot user_id: {snapshot.Key.ToString()}, Has username: {snapshot.HasChild("username")}, Has fullname: {snapshot.HasChild("fullname")}, Has email: {snapshot.HasChild("email")}, Has user_color: {snapshot.HasChild("user_color")}, Has clicks: {snapshot.HasChild("clicks_last_session")}");
        return snapshot.HasChild("username") && snapshot.HasChild("fullname") && snapshot.HasChild("email") && snapshot.HasChild("user_color") && snapshot.HasChild("clicks_last_session");
    }

    public IEnumerator CheckUserDataExists(string user_id, UnityAction onCheckCompleted) 
    {
        UIController.instance.ShowLoading(true);
        var DBTask = DBRef.Child("users").Child(user_id).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
            Debug.Log($"Check user exists failed!: {DBTask.Exception.Message}");   
        else 
        {
            DataSnapshot snapshot = DBTask.Result;
            if (snapshot.ChildrenCount >= 5)// Data exists
                CreateLeaderboardUserFromSnapshot(user_id, snapshot);
            else 
            {
                User new_userdata = CreateNewUserData(user_id, "Empty");
                LeaderboardManager.instance.CreateNewUserListElement(new_userdata);
                bool DatabaseCreated = false;
                StartCoroutine(CreateUserData(new_userdata, () => DatabaseCreated = true));
                yield return new WaitUntil(predicate: () => DatabaseCreated);
            }
        }
        onCheckCompleted.Invoke();
        UIController.instance.ShowLoading(false);
    }

    private User CreateNewUserData(string user_id, string fullname)
    {
        User user = new User();
        user.user_id = user_id;
        user.username = AuthManager.instance.firebaseUser.DisplayName;
        user.fullname = fullname;
        //user.avatar_url = AuthManager.instance.firebaseUser.PhotoUrl.ToString();
        user.email = AuthManager.instance.firebaseUser.Email;
        user.clicks_last_session = 0;
        user.user_color = $"#{ColorUtility.ToHtmlStringRGB(Color.HSVToRGB(UnityEngine.Random.Range(0.0000f, 1.0000f), 0.27f, 1.0f))}";
        return user;
    }

    public IEnumerator CreateUserData(User user, UnityAction onCompleted) 
    {
        UserRawJson json = new UserRawJson(user);
        var DBTask = DBRef.Child("users").Child(user.user_id).SetRawJsonValueAsync(JsonUtility.ToJson(json));
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
            Debug.Log($"Couldn't update user CLICKS AMOUNT!: {DBTask.Exception.Message}");      
        else Debug.Log($"User {user.username}({user.user_id}) database, updates successfuly!");
        onCompleted.Invoke();
    }
    public IEnumerator PushClicksUpdate()
    {
        User user = PlayerController.instance.user;
        var DBTask = DBRef.Child("users").Child(user.user_id).Child("clicks_last_session").SetValueAsync(user.clicks_last_session);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
            Debug.LogErrorFormat($"Couldn't update user clicks amount!: {DBTask.Exception.Message}");      
        else Debug.Log($"User {user.username}({user.user_id}) clicks amount updates successfuly!");
    }

    public void StartDatabaseListeners() 
    {
    }

    private void ChildChangedEventArgs()
    {
        throw new NotImplementedException();
    }
}
