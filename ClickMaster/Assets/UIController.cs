using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform), typeof(Canvas))]
public class UIController : MonoBehaviour
{
    public static UIController instance { get; private set; }
    [Header("Screens")]
    [SerializeField] private GameObject WelcomeScreen;
    [SerializeField] private GameObject RegistrationScreen;
    [SerializeField] private GameObject LoginScreen;
    [SerializeField] private GameObject ClickerScreen;
    [SerializeField] private GameObject LoadingScreen;

    [Header("Login Form")]
    public TMP_InputField UserNameField;
    public TMP_InputField UserPasswordField;
    public Button LoginButton;
    public TextMeshProUGUI LoginExceptionField;

    [Header("Register Form")]
    public TMP_InputField NewUserFullNameField;
    public TMP_InputField NewUserEmailField;
    public TMP_InputField NewUserNickNameField;
    public TMP_InputField NewUserPasswordField;
    public Button RegisterButton;
    public TextMeshProUGUI RegistrationExceptionField;

    public 
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        ShowWelcomeScreen();
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
        UserNameField.Select();// Select the first field.
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
        UserNameField.text = "";
        UserPasswordField.text = "";
        LoginExceptionField.gameObject.SetActive(false);
    }

}
