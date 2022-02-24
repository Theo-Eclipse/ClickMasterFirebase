using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions; // for ContinueWithOnMainThread
using System;

public class DatabaseListener : MonoBehaviour, IExecutionManager
{
    public static DatabaseListener instance { get; private set; }
    private DatabaseReference DatabaseReference;
    public void Init()
    {
        instance = this;
        DatabaseReference = DatabaseManager.instance.DBRef;
    }

    public static void EnableListener(bool enable) 
    {
        if (enable)
            FirebaseDatabase.DefaultInstance.GetReference("users").ChildChanged += HandleChildChange;
        else FirebaseDatabase.DefaultInstance.GetReference("users").ChildChanged -= HandleChildChange;
    }

    private static void HandleChildChange(object sender, ChildChangedEventArgs e)
    {
        if (e.DatabaseError != null)
        {
            Debug.LogErrorFormat(e.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
        string user_id = e.Snapshot.Key.ToString();
        if (e.Snapshot.ChildrenCount > 0 && user_id != AuthManager.instance.firebaseUser.UserId)
            DatabaseManager.instance.UpdateDataFromSnapshot(user_id, e.Snapshot);
        Debug.Log($"user changed: {user_id}, snapshot children: {e.Snapshot.ChildrenCount}, snapshot data: {e.Snapshot.Value.ToString()}");
    }
}
