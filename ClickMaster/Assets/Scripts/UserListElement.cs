using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class UserListElement : MonoBehaviour
{
    [Header("User Data")]
    public User user;

    [Header("Element's UI")]
    [SerializeField] private TextMeshProUGUI UserName_Label;
    [SerializeField] private Image UserAvatar;
    [SerializeField] private Image UserColor;
    [SerializeField] private TextMeshProUGUI UserPlace_Label;
    [SerializeField] private TextMeshProUGUI UserClicks_Label;
    private Color UColor;

    public void SetData() 
    {
        UserName_Label.text = user.username;
        UserAvatar.gameObject.SetActive(false);// 
        if (ColorUtility.TryParseHtmlString(user.user_color, out UColor))
            UserColor.color = UColor;
        UserClicks_Label.text = user.clicks_last_session.ToString();
    }

    public void OverrideLeaderboardPlace(int place) => UserPlace_Label.text = $"{place}#";

    public void OverrideShownName(string name) => UserName_Label.text = name;

    public bool SetAvatar(string url, Sprite out_sprite) 
    {
        out_sprite = null;
        return false;
    }
}
