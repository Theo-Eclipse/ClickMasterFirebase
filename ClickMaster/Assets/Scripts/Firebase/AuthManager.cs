using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;

public class AuthManager : MonoBehaviour
{
    public static AuthManager instance { get; private set; }
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth firebaseAuth;
    public FirebaseUser firebaseUser;
    public FirebaseApp app;

    [Header("Database")]
    public DatabaseManager DBManager;

    public void Awake()
    {
        instance = this;
        //UIController.instance.LoginButton.onClick.AddListener(TryLogin);
        //UIController.instance.RegisterButton.onClick.AddListener(TryRegister);
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = FirebaseApp.DefaultInstance;
                firebaseAuth = FirebaseAuth.DefaultInstance;
                DBManager.DBRef = Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference;
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                ThrowInitializationException(string.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public void ManualAuth() 
    {
        Awake();
    }

    public void TryLogin() 
    {
        UIController.instance.ShowLoading(true);
        StartCoroutine(Login(
            UIController.instance.UserEmailField.text, 
            UIController.instance.UserPasswordField.text));
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
        var RegisterTask = firebaseAuth.CreateUserWithEmailAndPasswordAsync(_email, _password);
        //Show loading screen
        UIController.instance.ShowLoading(true);
        yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);
        UIController.instance.ShowLoading(false);


        if (RegisterTask.Exception != null)
        {
            Debug.LogErrorFormat($"Registration failed with exception: {RegisterTask.Exception}");
            FirebaseException ex = RegisterTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)ex.ErrorCode;
            ThrowRegistrationException($"Registration failed with exception: {errorCode.ToString()}");
        }
        else
        {
            firebaseUser = RegisterTask.Result;
            if (firebaseUser != null)
            {
                Debug.Log($"User registered seccessfuly! with username: {firebaseUser.DisplayName}, and email: {firebaseUser.Email}");
                UserProfile profile = new UserProfile { DisplayName = _username };// Can be used to update Avatar URL!
                var ProfileUpdateTask = firebaseUser.UpdateUserProfileAsync(profile);
                yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

                if (ProfileUpdateTask.Exception != null)
                    Debug.Log($"Could not update user profile with exception: {ProfileUpdateTask.Exception.Message}");
                else
                {
                    Debug.Log($"Profile display name updated successfully!");
                    UIController.instance.UserEmailField.text = _email;
                    UIController.instance.ShowLoginScreen();
                }
            }
            else 
            {
                Debug.LogErrorFormat($"Registration failed with exception: {RegisterTask.Exception}");
                FirebaseException ex = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)ex.ErrorCode;
                ThrowRegistrationException($"Registration failed with exception: {errorCode.ToString()}");
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
        if (firebaseUser != null) 
            firebaseAuth.SignOut();
    }
}
