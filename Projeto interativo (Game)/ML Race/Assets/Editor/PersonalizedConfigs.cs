using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;

public class PersonalizedConfigs
{
    [MenuItem("ML Race - PI 5/Start Server")]
    public static void StartServer()
    {
        LoadFiles.LoadXML();

        Process.Start(LoadFiles.globalPaths.executeServerFilePath);
    }

    [MenuItem("ML Race - PI 5/Save XML")]
    public static void SaveXMLFile()
    {
        LoadFiles.SetPaths();
        LoadFiles.SaveXML();
    }
    [MenuItem("ML Race - PI 5/Load XML")]
    public static void LoadXMLFile()
    {
        LoadFiles.LoadXML();
    }
}
