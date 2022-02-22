using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject WelcomeScreen;
    [SerializeField] private GameObject RegistrationScreen;
    [SerializeField] private GameObject LoginScreen;
    [SerializeField] private GameObject ClickerScreen;
    [SerializeField] private GameObject LoadingScreen;
    // Start is called before the first frame update
    void Start()
    {
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
        WelcomeScreen.SetActive(false);
        RegistrationScreen.SetActive(true);
        LoginScreen.SetActive(false);
        ClickerScreen.SetActive(false);
        ShowLoading(false);
    }

    public void ShowLoginScreen()
    {
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
    
}
