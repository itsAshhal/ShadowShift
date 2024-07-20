using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadowShift.DataModels
{
    [System.Serializable]
    public class PlayerData
    {
        // we need to save controls for the player
        public string Controls = string.Empty;
    }


    public static class GameData
    {
        public static string FileName = "PlayerData";
        static List<PlayerData> m_playerDataList = new List<PlayerData>();
        public static void SaveData(PlayerData playerData, bool append = false)
        {
            m_playerDataList.Clear();
            m_playerDataList.Add(playerData);

            // save this list to the JSON file
            FileHandler.SaveToJSON(m_playerDataList, FileName); // also we don't need to append anything, just override the data
        }
        public static PlayerData LoadData()
        {
            var loadedData = FileHandler.ReadFromJSON<PlayerData>(FileName);

            // since we have only one data instance and also we're saving data as overriding, we need the first index of the data for now atleast
            return loadedData[0];
        }

        public static bool FileExists() => FileHandler.JSONFileExists(FileName);
    }
}

