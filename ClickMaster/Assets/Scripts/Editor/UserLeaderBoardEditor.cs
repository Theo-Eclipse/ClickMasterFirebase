using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UserLeaderBoard))]
public class UserLeaderBoardEditor : Editor
{
    private UserLeaderBoard instance;
    private void OnEnable()
    {
        if(!instance) instance = (UserLeaderBoard)target;
    }

    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Editor Script", MonoScript.FromScriptableObject(this), typeof(UserLeaderBoard), false);
        GUI.enabled = true;
        base.OnInspectorGUI();
        if (GUILayout.Button("Create Random User")) 
        {
            instance.users_elements.Add(instance.NewRandomUser());
        }

        if (GUILayout.Button("Sort/Order Users"))
        {
            instance.Sort();
        }
    }
}