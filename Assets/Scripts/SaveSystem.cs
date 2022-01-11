using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static string saveFilePath = Application.persistentDataPath + "/savedata.rocky";

    public static void SavePlayerData(Player player, float y_position, int sceneIndex)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = saveFilePath;
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(player, y_position, sceneIndex);

        formatter.Serialize(stream, data);
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

    public static void Reset()
    {

    }
}
