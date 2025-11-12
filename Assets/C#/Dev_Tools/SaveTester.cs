using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SaveTester : MonoBehaviour
{
    public void SaveGame()
    {
        SaveDataManager.Save(SaveType.Player);
        SaveDataManager.Save(SaveType.Config);
    }

    public void LoadGame()
    {
        SaveDataManager.Load(SaveType.Player);
        SaveDataManager.Load(SaveType.Config);
    }

    public void RebuidSave()
    {
        SaveDataManager.RebuildFile(SaveType.Player);
        SaveDataManager.RebuildFile(SaveType.Config);
    }

    public void OpenFileLocation()
    {
        EditorUtility.RevealInFinder(SaveDataManager.savePath);
    }
}
