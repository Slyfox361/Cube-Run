using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

//save path for reference: C:\Users\<YourUserName>\AppData\LocalLow\Slyfox Studio\Cyberpunk Endless Runner\Game.playerData

public static class saveAndLoad
{
    public static void SavePlayerData(playerManager p) //save data function
    {
        BinaryFormatter formatter = new BinaryFormatter(); //creates a BinaryFormatter for writing

        string path = Application.persistentDataPath + "/Game.playerData"; //sets the path for saving data
        

        using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None)) //open/creats a file at the path specified above
        {
            playerData pData = new playerData(p); //converts the 'test' object to a 'testData' object
            formatter.Serialize(stream, pData); //serialises the 'testData' to a binary format and writes it to the file
        }

        //stream.Close(); //closes the file stream
    }

    public static playerData LoadPlayerData() //load data function
    {
        string path = Application.persistentDataPath + "/Game.playerData"; //sets the path for loading data

        if(File.Exists(path)) //if the file exists...
        {
            BinaryFormatter formatter = new BinaryFormatter(); //creates a BinaryFormatter for reading

            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) //opens the file
            {
                return formatter.Deserialize(stream) as playerData; //deserializes the binary data and converts it to a 'testData' object
            }
            
           // stream.Close(); //closes the file stream
        }
        else //errors
        {
            Debug.Log("Error: Save file not found in " + path);
            return null;
        }
    }

    public static void DeleteInventoryData()
    {
        string path = Application.persistentDataPath + "/Game.playerData";

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("file deleted");
        }
        else
        {
            Debug.Log("No file to delete");
        }
    }
}