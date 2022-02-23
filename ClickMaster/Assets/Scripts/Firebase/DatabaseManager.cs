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

    public IEnumerator LoadLeaderboardData(UnityAction onCompleted)
    {
        //var DBTask = DBRef.Child("users").OrderByChild("clicks_last_session").LimitToFirst(enteries_amount).GetValueAsync(); <- Version for limited amount of data.
        var DBTask = DBRef.Child("users").OrderByChild("clicks_last_session").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogErrorFormat($"Couldn't get leaderboard data!: {DBTask.Exception.Message}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            foreach (DataSnapshot childsnapshot in snapshot.Children) 
            {
                Debug.Log($"Loaded Leaderboard Entery: {childsnapshot.Key.ToString()}");
                string user_id = childsnapshot.Key.ToString();
                if (SnapshotHasData(childsnapshot))
                {
                    if (LeaderboardManager.UserExists(user_id))
                        UpdateLeaderboardUserFromSnapshot(user_id, childsnapshot);
                    else CreateLeaderboardUserFromSnapshot(user_id, childsnapshot);
                }
            }
            onCompleted.Invoke();
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

    public IEnumerator CreateUserData(User user, UnityAction onCompleted) 
    {
        // Display Name
        var DBTask = DBRef.Child("users").Child(AuthManager.instance.firebaseUser.UserId).Child("username").SetValueAsync(AuthManager.instance.firebaseUser.DisplayName);
        var waiter = new WaitUntil(predicate: () => DBTask.IsCompleted);
        yield return waiter;
        if (DBTask.Exception != null) Debug.LogErrorFormat($"Couldn't update USER NAME!: {DBTask.Exception.Message}");

        // Full Name
        DBTask = DBRef.Child("users").Child(AuthManager.instance.firebaseUser.UserId).Child("fullname").SetValueAsync(user.fullname);
        yield return waiter;
        if (DBTask.Exception != null) Debug.LogErrorFormat($"Couldn't update user FULL NAME!: {DBTask.Exception.Message}");

        //Email
        DBTask = DBRef.Child("users").Child(AuthManager.instance.firebaseUser.UserId).Child("email").SetValueAsync(user.email);
        yield return waiter;
        if (DBTask.Exception != null) Debug.LogErrorFormat($"Couldn't update user EMAIL!: {DBTask.Exception.Message}");

        //User Color
        DBTask = DBRef.Child("users").Child(AuthManager.instance.firebaseUser.UserId).Child("user_color").SetValueAsync(user.user_color);
        yield return waiter;
        if (DBTask.Exception != null) Debug.LogErrorFormat($"Couldn't update user USER COLOR!: {DBTask.Exception.Message}");
        // Avatar URL
        //DBTask = DBRef.Child("users").Child(AuthManager.instance.firebaseUser.UserId).Child("avatar_url").SetValueAsync(AuthManager.instance.firebaseUser.PhotoUrl);
        //yield return waiter;
        //if (DBTask.Exception != null) Debug.LogErrorFormat($"Couldn't update AVATAR URL!: {DBTask.Exception.Message}");
        // Clicks Amount
        DBTask = DBRef.Child("users").Child(AuthManager.instance.firebaseUser.UserId).Child("clicks_last_session").SetValueAsync(user.clicks_last_session);
        yield return waiter;
        if (DBTask.Exception != null)
        {
            Debug.LogErrorFormat($"Couldn't update user CLICKS AMOUNT!: {DBTask.Exception.Message}");
            yield break;
        }
        else
        {
            Debug.Log($"User {user.username}({user.user_id}) database, updates successfuly!");
            onCompleted.Invoke();
        }
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
