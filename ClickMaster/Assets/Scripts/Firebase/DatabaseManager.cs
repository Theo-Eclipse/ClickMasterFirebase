using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using TMPro;
public class DatabaseManager : MonoBehaviour
{
    [Header("Firebase")]
    public DatabaseReference DBRef;

    public IEnumerator UpdateUserFullName(string _FullName) 
    {
        var DBTask = DBRef.Child("users").Child(AuthManager.instance.firebaseUser.UserId).Child("fullname").SetValueAsync(_FullName);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            // If problem detected.
        }
        else 
        {
            // If Everithing is fine.

        }
    }
}
