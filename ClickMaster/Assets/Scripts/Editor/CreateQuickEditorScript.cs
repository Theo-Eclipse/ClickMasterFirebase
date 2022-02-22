using UnityEditor;
using UnityEngine;
using System.IO;
using System;
//
//To be used in project window => Right click on a c# script => Create => C# Custom Editor.
//It will quickly create a Custom Editor script from a template.
//The created editor script will also be seen in the inspector, when an object with relevant script is selected.
//
public class CreateQuickEditorScript : MonoBehaviour
{
    //
    // NOTE: Script generation is based on a template text file, located in: Assets/Scripts/TemplateScripts/CustomEditorScript.txt
    //
    // A string filter with unique characters for replacment.
    // This is to be used as: text = text.Replace("$$ScriptName$$", "DesiredClassName")
    private static readonly string ScriptNameFilter = "$$ScriptName$$";
    private static readonly string TemplatePath = "Scripts/TemplateScripts/CustomEditorScript.txt";
    private static string readenfile;
    private static string scriptName = "";
    private static string scriptPath = "";
    [MenuItem("Assets/Create/C# Create Custom Inspector", false, 40)]
    private static void CreateEditorScriptForScript()
    {
        if (!hasSelection() || !selectedIsScript() || IsEditorScript())
            return;
        scriptName = Selection.activeObject.name;
        scriptPath = AssetDatabase.GetAssetPath(Selection.activeObject).Replace("Assets/", string.Format("{0}/", Application.dataPath));
        if (ScriptExists(string.Format("{0}Editor", scriptName)))
        {
            Debug.LogErrorFormat(string.Format("{0} Already has an Editor script", scriptName));
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(string.Format("Assets/Scripts/Editor/{0}Editor.cs", scriptName));
            EditorGUIUtility.PingObject(Selection.activeObject);
            return;
        }
        FileInfo TemplateScript = new FileInfo(string.Format("{0}/{1}", Application.dataPath, TemplatePath));
        if (!TemplateScript.Exists)
        {
            Debug.LogErrorFormat(string.Format("Template script was not found!\n Expected file path: {0}", TemplateScript.FullName));
            return;
        }
        readenfile = File.ReadAllText(TemplateScript.FullName);
        readenfile = readenfile.Replace(ScriptNameFilter, scriptName);
        FileInfo newCustomEditorScript = new FileInfo(string.Format("{0}/Scripts/Editor/{1}Editor.cs", Application.dataPath, scriptName));
        File.WriteAllText(newCustomEditorScript.FullName, readenfile);
        AssetDatabase.Refresh();
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(string.Format("Assets/Scripts/Editor/{0}Editor.cs", scriptName));
        EditorGUIUtility.PingObject(Selection.activeObject);
    }
    private static bool ScriptExists(string class_name) 
    {
        var myClassType = Type.GetType(class_name);
        return myClassType == null ? false : myClassType.GetMethod("OnInspectorGUI") != null;
    }
    private static bool IsEditorScript()
    {
        scriptPath = AssetDatabase.GetAssetPath(Selection.activeObject).Replace("Assets/", string.Format("{0}/", Application.dataPath));
        FileInfo CheckCurrentScript = new FileInfo(scriptPath);
        readenfile = File.ReadAllText(CheckCurrentScript.FullName);
        var isEditorScript = readenfile.Contains(string.Format("public class {0} : Editor", Selection.activeObject.name));
        readenfile = null;
        if (isEditorScript)
            Debug.LogErrorFormat(string.Format("{0} is an editor itself!", Selection.activeObject.name));
        return isEditorScript;
    }
    private static bool selectedIsScript() 
    {
        if (Selection.activeObject.GetType() == typeof(MonoScript))
            return true;     
        else Debug.LogErrorFormat(string.Format("{0} is not a MonoScript!\nPlease select a script in order to create a custom inspector for it.", Selection.activeObject.name));
        return false;
    }
    private static bool hasSelection()
    {
        if (Selection.activeObject != null)
            return true;
        else Debug.LogErrorFormat("Please select a script in order to create a custom inspector for it.");
        return false;
    }
}
