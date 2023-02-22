using System.IO;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    public void SaveLocal(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }
    public void SaveLocal(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }
    public void SaveLocal(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public float GetLocalFloat(string key)
    {
        return PlayerPrefs.GetFloat(key);
    }
    public int GetLocalInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }
    public string GetLocalString(string key)
    {
        return PlayerPrefs.GetString(key);
    }

    string saveFile;
    public static GameData gameData = new GameData();

    protected override void Awake()
    {
        base.Awake();
        saveFile = Application.persistentDataPath + "/gamedata.json";
        //ReadFile();
    }

    public void ReadFile()
    {
        if (File.Exists(saveFile))
        {
            string fileContents = File.ReadAllText(saveFile);
            gameData = JsonUtility.FromJson<GameData>(fileContents);
        }
    }

    public void WriteFile()
    {
        string jsonString = JsonUtility.ToJson(gameData);
        File.WriteAllText(saveFile, jsonString);
    }

    public void ClearData()
    {
        File.Delete(saveFile);
    }
}

[System.Serializable]
public class GameData
{
    public int goldCount;
    public int workersCount;
    public int knightsCount;
    public int raftPartsCount;
}