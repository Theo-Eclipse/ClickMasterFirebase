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
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = FirebaseApp.DefaultInstance;
                firebaseAuth = FirebaseAuth.DefaultInstance;
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                ThrowInitializationException(string.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    private void OnLoginSuccess() 
    {
        DatabaseManager.instance.DBRef = Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference;
        UIController.instance.ShowClickerScreen();
        StartCoroutine(DatabaseManager.instance.LoadLeaderboardData(() => 
        {
            PlayerController.instance.SetCurrentUserInstance();
            ClicksDataPusher.instance.SetEnable(true);
        }));
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
        PlayerPrefs.SetString("LAST_LOGIN_EMAIL", _email);
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
            onSuccess.Invoke();
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
        PlayerPrefs.SetString("LAST_LOGIN_EMAIL", _email);
        var RegisterTask = firebaseAuth.CreateUserWithEmailAndPasswordAsync(_email, _password);
        //Show loading screen
        UIController.instance.ShowLoading(true);
        yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);
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
                    User registered_user = CreateNewUserData(_fullname);
                    StartCoroutine(DatabaseManager.instance.CreateUserData(registered_user, () =>
                    {
                        UIController.instance.ShowLoginScreen();
                    }));
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
        UIController.instance.ShowLoading(false);
    }

    private User CreateNewUserData(string fullname)
    {
        User user = new User();
        user.user_id = firebaseUser.UserId;
        user.username = firebaseUser.DisplayName;
        user.fullname = fullname;
        //user.avatar_url = AuthManager.instance.firebaseUser.PhotoUrl.ToString();
        user.email = firebaseUser.Email;
        user.clicks_last_session = 0;
        user.user_color = $"#{ColorUtility.ToHtmlStringRGB(Color.HSVToRGB(Random.Range(0.0000f, 1.0000f), 0.27f, 1.0f))}";
        return user;
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
