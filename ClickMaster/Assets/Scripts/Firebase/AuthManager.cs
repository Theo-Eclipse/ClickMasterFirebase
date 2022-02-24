using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.Events;

public class AuthManager : MonoBehaviour, IExecutionManager
{
    public static AuthManager instance { get; private set; }
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth firebaseAuth;
    public FirebaseUser firebaseUser;
    public FirebaseApp app;

    public void Init()
    {
        instance = this;
        Debug.Log("Auth Manager Initialization...");
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;
                firebaseAuth = FirebaseAuth.DefaultInstance;
            }
            else
                ThrowInitializationException(string.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            
        });
    }

    private void OnLoginSuccess() 
    {
        PlayerPrefs.SetInt("FirstLogin", 0);// 0 - False(When it's not a first time you've logged in);
        PlayerPrefs.SetString("LAST_LOGIN_EMAIL", firebaseUser.Email);// Remeber last login
        if (UIController.instance.KeepPasswordToggle.isOn)
            PlayerPrefs.SetString("LAST_SAVED_PASS", UIController.instance.UserPasswordField.text);// Remeber last login
        UIController.instance.ShowClickerScreen();
        PlayerController.instance.SetCurrentUserInstance(firebaseUser.UserId);
        StartCoroutine(DatabaseManager.instance.LoadLeaderboardData());
        ClicksDataPusher.instance.SetEnable(true);
        UIController.instance.ShowLoading(false);
    }

    public void TryLogin() 
    {
        UIController.instance.ShowLoading(true);
        StartCoroutine(Login(
            UIController.instance.UserEmailField.text, 
            UIController.instance.UserPasswordField.text, OnLoginSuccess));
    }

    public void TryRegister() 
    {
        UIController.instance.ShowLoading(true);
        StartCoroutine(Register(
            UIController.instance.NewUserEmailField.text, 
            UIController.instance.NewUserPasswordField.text,
            UIController.instance.NewUserNickNameField.text,
            UIController.instance.NewUserFullNameField.text));
    }

    private IEnumerator Login(string _email, string _password, UnityAction onSuccess) 
    {
        if (string.IsNullOrEmpty(_email) || string.IsNullOrEmpty(_password))
        {
            string error_message = "Following fields are missing: ";
            if (string.IsNullOrEmpty(_email)) error_message += "Email, ";
            if (string.IsNullOrEmpty(_password)) error_message += "Password.";
            ThrowLoginException(error_message);
            yield break;
        }
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
            // Create database referance after successful login 
            DatabaseManager.instance.DBRef = Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference;
            StartCoroutine(DatabaseManager.instance.CheckUserDataExists(firebaseUser.UserId, onSuccess));
        }
    }

    private IEnumerator Register(string _email, string _password, string _username, string _fullname)
    {
        if (string.IsNullOrEmpty(_email) || string.IsNullOrEmpty(_password) || string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_fullname)) 
        {
            string error_message = "Following fields are missing: ";
            if (string.IsNullOrEmpty(_email)) error_message += "Email, ";
            if (string.IsNullOrEmpty(_password)) error_message += "Password, ";
            if (string.IsNullOrEmpty(_username)) error_message += "User Name, ";
            if (string.IsNullOrEmpty(_fullname)) error_message += "Full Name. ";
            ThrowRegistrationException(error_message);
            yield break;
        }
        //
        // Registration Start
        //
        var RegisterTask = firebaseAuth.CreateUserWithEmailAndPasswordAsync(_email, _password);
        UIController.instance.ShowLoading(true);
        yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);
        if (RegisterTask.Exception != null)// Registration failed
        {
            Debug.LogErrorFormat($"Registration failed with exception: {RegisterTask.Exception}");
            FirebaseException ex = RegisterTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)ex.ErrorCode;
            ThrowRegistrationException($"Registration failed with exception: {errorCode.ToString()}");
            UIController.instance.ShowLoading(false);
        }
        else// Registration success
        {
            firebaseUser = RegisterTask.Result;
            if (firebaseUser != null)
            {
                Debug.Log($"User registered seccessfuly! with username: {firebaseUser.DisplayName}, and email: {firebaseUser.Email}");
                UserProfile profile = new UserProfile { DisplayName = _username };
                var ProfileUpdateTask = firebaseUser.UpdateUserProfileAsync(profile);// updating display name
                yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

                if (ProfileUpdateTask.Exception != null)// Could'nt update display name
                {
                    Debug.Log($"Could not update user profile with exception: {ProfileUpdateTask.Exception.Message}");
                    UIController.instance.ShowLoading(false);
                }
                else
                {
                    Debug.Log($"Registration completed successfully! now you'll log in");
                    //
                    // Automatic login.
                    //
                    var LoginTask = firebaseAuth.SignInWithEmailAndPasswordAsync(_email, _password);
                    yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);
                    // Automatic login failed.
                    if (LoginTask.Exception != null)
                    {
                        Debug.LogErrorFormat($"Failed to login, with exception: {LoginTask.Exception}");
                        FirebaseException ex = LoginTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)ex.ErrorCode;
                        ThrowLoginException($"Failed to login, with exception: {errorCode}");
                        UIController.instance.ShowLoading(false);
                    }
                    else//Automatic login success.
                    {
                        firebaseUser = LoginTask.Result;
                        // Create database referance after successful login 
                        DatabaseManager.instance.DBRef = Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference;
                        Debug.Log($"User logged in seccessfuly! as username: {firebaseUser.DisplayName}, with email: {firebaseUser.Email}");
                        DatabaseManager.instance.CreateLeaderboardAndUserdata(firebaseUser.UserId, _fullname, OnLoginSuccess);
                    }
                }
            }
            else 
            {
                Debug.LogErrorFormat($"Registration failed with exception: {RegisterTask.Exception}");
                FirebaseException ex = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)ex.ErrorCode;
                ThrowRegistrationException($"Registration failed with exception: {errorCode}");
                UIController.instance.ShowLoading(false);
            }
        }
    }

    private void ThrowInitializationException(string exception_message)
    {
        UIController.instance.InitializationExceptionField.gameObject.SetActive(true);
        UIController.instance.InitializationExceptionField.text = exception_message;
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

    private void OnApplicationQuit()
    {
        ClicksDataPusher.instance.SetEnable(false);
        if (firebaseUser != null) 
            firebaseAuth.SignOut();
    }
}
