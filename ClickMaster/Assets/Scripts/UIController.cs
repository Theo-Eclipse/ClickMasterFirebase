using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour, IExecutionManager
{
    public static UIController instance { get; private set; }
    [Header("Screens")]
    [SerializeField] private GameObject WelcomeScreen;
    [SerializeField] private GameObject RegistrationScreen;
    [SerializeField] private GameObject LoginScreen;
    [SerializeField] private GameObject ClickerScreen;
    [SerializeField] private GameObject LoadingScreen;

    [Header("Home Screen")]
    public TextMeshProUGUI InitializationExceptionField;

    [Header("Login Form")]
    public TMP_InputField UserEmailField;
    public TMP_InputField UserPasswordField;
    public Button LoginButton;
    public TextMeshProUGUI LoginExceptionField;
    public Toggle KeepPasswordToggle;

    [Header("Register Form")]
    public TMP_InputField NewUserFullNameField;
    public TMP_InputField NewUserEmailField;
    public TMP_InputField NewUserNickNameField;
    public TMP_InputField NewUserPasswordField;
    public Button RegisterButton;
    public TextMeshProUGUI RegistrationExceptionField;

    [Header("Leaderboard Screen")]
    public TextMeshProUGUI ButtonClickCounter;
    public GameObject SignoutScreen;

    // Start is called before the first frame update
    public void Init()
    {
        instance = this;
        Debug.Log("UI Controller Initialization...");
        if (PlayerPrefs.GetInt("FirstLogin", 1) == 1)
            ShowWelcomeScreen();
        else ShowLoginScreen();
    }

    // Update is called once per frame
    void Update()
    {
        //Only for testing!
        if (Input.GetKeyDown(KeyCode.Escape))
            ShowWelcomeScreen();
    }

    public void ShowWelcomeScreen() 
    {
        WelcomeScreen.SetActive(true);
        RegistrationScreen.SetActive(false);
        LoginScreen.SetActive(false);
        ClickerScreen.SetActive(false);
        ShowLoading(false);
    }

    public void ShowRegistrationScreen()
    {
        ClearRegistrationForm();
        NewUserFullNameField.Select();// Select the first field.
        WelcomeScreen.SetActive(false);
        RegistrationScreen.SetActive(true);
        LoginScreen.SetActive(false);
        ClickerScreen.SetActive(false);
        ShowLoading(false);
    }

    public void ShowLoginScreen()
    {
        ClearLoginForm();
        UserEmailField.Select();// Select the first field.
        WelcomeScreen.SetActive(false);
        RegistrationScreen.SetActive(false);
        LoginScreen.SetActive(true);
        ClickerScreen.SetActive(false);
        ShowLoading(false);
    }

    public void ShowClickerScreen()
    {
        WelcomeScreen.SetActive(false);
        RegistrationScreen.SetActive(false);
        LoginScreen.SetActive(false);
        ClickerScreen.SetActive(true);
        ShowLoading(false);
    }

    public void ShowLoading(bool loading) => LoadingScreen.SetActive(loading);

    public void KeepPassword(bool keep) 
    {
        PlayerPrefs.SetString("LAST_SAVED_PASS", keep ? UserPasswordField.text : "");
    }

    private void ClearRegistrationForm() 
    {
        NewUserFullNameField.text = "";
        NewUserEmailField.text = "";
        NewUserNickNameField.text = "";
        NewUserPasswordField.text = "";
        RegistrationExceptionField.gameObject.SetActive(false);
    }

    private void ClearLoginForm()
    {
        UserEmailField.text = PlayerPrefs.GetString("LAST_LOGIN_EMAIL", "");
        UserPasswordField.text = PlayerPrefs.GetString("LAST_SAVED_PASS", "");
        KeepPasswordToggle.isOn = !string.IsNullOrEmpty(UserPasswordField.text);
        LoginExceptionField.gameObject.SetActive(false);
    }  
}
