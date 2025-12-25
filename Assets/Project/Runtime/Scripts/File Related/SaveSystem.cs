using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void Save(GameData gameData)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();    // use binary formatter to securely save information
        string path = Application.persistentDataPath + "/gamedata.atademag";
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = gameData;

        binaryFormatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData Load()   // load the data into game
    {
        string path = Application.persistentDataPath + "/gamedata.atademag";
        if (File.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            GameData data = binaryFormatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
