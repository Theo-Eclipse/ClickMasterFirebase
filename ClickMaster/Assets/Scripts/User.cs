using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User
{
    public string username = "User name";
    public string fullname = "User name";
    public string email = "";
    //public string avatar_url = "";
    public string user_color = "#A2A2A2";
    public int clicks_last_session = 0;
    public string user_id = "8768FGA111";

    public User(){}
}

public class UserRawJson
{
    public string username = "User name";
    public string fullname = "User name";
    public string email = "";
    public string user_color = "#A2A2A2";
    public int clicks_last_session = 0;

    public UserRawJson(User user) 
    {
        username = user.username;
        fullname = user.fullname;
        email = user.email;
        user_color = user.user_color;
        clicks_last_session = user.clicks_last_session;
    }
}
