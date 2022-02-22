using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class UserLeaderBoard : MonoBehaviour
{
    [SerializeField] private UserListElement element_template;
    [SerializeField] private TextMeshProUGUI ButtonCounter;
    public List<UserListElement> users_elements = new List<UserListElement>();

    public UserListElement NewRandomUser() 
    {
        UserListElement new_user = Instantiate(element_template.gameObject, transform).GetComponent<UserListElement>();
        new_user.gameObject.SetActive(true);
        new_user.user = new User(NameGenerator.GetName());
        new_user.user.clicks_last_session = Random.Range(100, 9999);
        new_user.user.user_color = $"#{ColorUtility.ToHtmlStringRGB(Color.HSVToRGB(Random.Range(0.0000f, 1.0000f), 0.27f, 1.0f))}";
        new_user.Set(0);
        return new_user;
    }

    public void Sort() 
    {
        GetComponentsInChildren(false, users_elements);
        element_template.user.clicks_last_session = Random.Range(10, 9999);
        element_template.Set(0);
        ButtonCounter.text = element_template.user.clicks_last_session.ToString();
        users_elements.Add(element_template);
        users_elements = users_elements.OrderByDescending((s) => s.user.clicks_last_session).ThenBy((s) => s.user.name).ToList();
        foreach (var user in users_elements)
        {
            user.SetLeaderboardPlace(users_elements.IndexOf(user) + 1);
            user.transform.SetAsLastSibling();
        }
        element_template.OverrideShownName("You");
    }

    private void OnEnable() => Sort();
}
