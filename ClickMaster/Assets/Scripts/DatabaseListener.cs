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

    public void AddListener(string user_id) 
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").Child(user_id).ValueChanged += HandleValueChanged;
    }

    public static void AddGlobalListener() 
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").LimitToFirst(1).ValueChanged += instance.HandleUserChanged;
    }
    
    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogErrorFormat(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
        string user_id = args.Snapshot.Key.ToString();
        if (args.Snapshot.ChildrenCount > 0 && user_id != AuthManager.instance.firebaseUser.UserId)
            DatabaseManager.instance.UpdateDataFromSnapshot(user_id, args.Snapshot);
        Debug.Log($"user change: {args.Snapshot.ChildrenCount}, args key: {args.Snapshot.Key}");
    }

    private void HandleUserChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogErrorFormat(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
        string user_id = args.Snapshot.Key.ToString();
        Debug.Log($"user change: {args.Snapshot.ChildrenCount}, args key: {args.Snapshot.Key}");
    }
}
