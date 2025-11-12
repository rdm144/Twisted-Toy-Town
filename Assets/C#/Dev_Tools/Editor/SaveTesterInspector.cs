using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(SaveTester))]
public class SaveTesterInspector : Editor
{
    public override void OnInspectorGUI()
    {
        SaveTester tester = (SaveTester)target;

        DrawDefaultInspector();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save"))
        {
            tester.SaveGame();
        }
        if (GUILayout.Button("Load"))
        {
            tester.LoadGame();
        }
        if (GUILayout.Button("Rebuild"))
        {
            tester.RebuidSave();
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Open File Location"))
        {
            tester.OpenFileLocation();
        }
    }
}
