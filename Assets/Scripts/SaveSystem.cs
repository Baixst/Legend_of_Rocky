using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static string saveFilePath = Application.persistentDataPath + "/savedata.rocky";
    private static string settingsFilePath = Application.persistentDataPath + "/userSettings.rockpref";

    public static void SavePlayerData(Player player, float y_position, int sceneIndex)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = saveFilePath;
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(player, y_position, sceneIndex);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SavePlayerSettings(float masterVol, float musicVol, float soundVol)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = settingsFilePath;
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveSettings settings = new SaveSettings(masterVol, musicVol, soundVol);

        formatter.Serialize(stream, settings);
        stream.Close();
    }

    public static SaveData LoadPlayerData()
    {
        string path = saveFilePath;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("Savefile not found in " + path);
            return null;
        }
    }

    public static SaveSettings LoadPlayerSettings()
    {
        string path = settingsFilePath;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveSettings settings = formatter.Deserialize(stream) as SaveSettings;
            stream.Close();

            return settings;
        }
        else
        {
            Debug.Log("Settingsfile not found in " + path);
            return null;
        }
    }
}
