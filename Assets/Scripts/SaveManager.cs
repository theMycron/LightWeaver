using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static readonly string savePath = Application.persistentDataPath + "/GameData";
    private static readonly string saveFileName = "save";
    private static readonly string fullSavePath = $"{savePath}/{saveFileName}";

    public static void Save(SaveProfile saveData)
    {
        // create save directory if it doesnt exist
        if (!Directory.Exists(savePath)) { 
            Directory.CreateDirectory(savePath);
        }

        // will overwrite file if it exists
        string jsonString = JsonUtility.ToJson(saveData);
        File.WriteAllText(fullSavePath, jsonString);
        Debug.Log(fullSavePath);
    }

    public static SaveProfile Load()
    {
        if (!File.Exists(fullSavePath))
            return null;

        string fileContents = File.ReadAllText(fullSavePath);
        return JsonUtility.FromJson<SaveProfile>(fileContents);
    }

    public static void DeleteSave()
    {
        if (!File.Exists(fullSavePath))
            return;

        File.Delete(fullSavePath);
    }
}
