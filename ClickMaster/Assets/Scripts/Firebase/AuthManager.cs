using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;

public class AuthManager : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth firebaseAuth;
    public FirebaseUser firebaseUser;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => 
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else 
            {
                ThrowLoginException($"Couldn't resolve all the depencies! status: {dependencyStatus.ToString()}");
                ThrowRegistrationException($"Couldn't resolve all the depencies! status: {dependencyStatus.ToString()}");
            }
        });
    }

    private void InitializeFirebase() 
    {
        Debug.Log("Initializing firebase auth...");
        firebaseAuth = FirebaseAuth.DefaultInstance;
        UIController.instance.LoginButton.onClick.AddListener(TryLogin);
        UIController.instance.RegisterButton.onClick.AddListener(TryRegister);
    }

    private void TryLogin() 
    {
    }

    private void TryRegister() 
    {
    }

    private IEnumerator Login(string _email, string _password) 
    {
        var LoginTask = firebaseAuth.SignInWithEmailAndPasswordAsync(_email, _password);

        //Show loading screen
        UIController.instance.ShowLoading(true);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);
        UIController.instance.ShowLoading(false);
        if (LoginTask.Exception != null)
        {
            Debug.LogErrorFormat($"Failed to login, with exception: {LoginTask.Exception}");
            FirebaseException ex = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)ex.ErrorCode;
            ThrowLoginException($"Failed to login, with exception: {errorCode.ToString()}");
        }
        else 
        {
            firebaseUser = LoginTask.Result;
            Debug.Log($"User logged in seccessfuly! as username: {firebaseUser.DisplayName}, with email: {firebaseUser.Email}");
            UIController.instance.ShowClickerScreen();
        }
    }

    private void ThrowLoginException(string exception_message) 
    {
        UIController.instance.LoginExceptionField.gameObject.SetActive(true);
        UIController.instance.LoginExceptionField.text = exception_message;
    }
    private void ThrowRegistrationException(string exception_message)
    {
        UIController.instance.RegistrationExceptionField.gameObject.SetActive(true);
        UIController.instance.RegistrationExceptionField.text = exception_message;
    }
}
