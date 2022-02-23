using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User
{
    public string name = "User name";
    public string avatar_url = "";
    public string user_color = "#A2A2A2";
    public int clicks_last_session = 0;
    public string user_id = "8768FGA111";

    public User(string name) 
    {
        this.name = name;
        user_color = ColorUtility.ToHtmlStringRGB(Color.HSVToRGB(1, 1, 1));
        user_id = Random.Range(1000000, 9999999).ToString();
    }
}
