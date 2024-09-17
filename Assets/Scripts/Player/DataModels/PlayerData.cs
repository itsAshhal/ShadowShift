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
        public int Stage = 0;
        public float CameraOrthoSize = 10.0f;
        public float CameraHeight = 3.0f;
        public float MusicValue = .25f;
    }


    [System.Serializable]
    public class ColorData
    {
        public string HexColor = string.Empty;
    }


    public static class GameData
    {
        public static string FileName = "PlayerData";
        public static string SavedColorsFileName = "PlayerColors";
        static List<PlayerData> m_playerDataList = new List<PlayerData>();
        static List<ColorData> m_colorData = new List<ColorData>();


        /// <summary>
        /// The current selected color by the player, which is gonna be used in the gameplay
        /// </summary>
        public static Color SelectedColor = Color.black;

        /// <summary>
        /// If true then all the sprites in the gameplay will also be reflected towards the selected player color
        /// </summary>
        public static bool ToggleStageColors = false;

        public static void SaveColorData(ColorData colorData, bool append = false)
        {
            m_colorData.Clear();
            m_colorData.Add(colorData);

            // save this list to the JSON file
            FileHandler.SaveToJSON(m_colorData, SavedColorsFileName, append); // also we don't need to append anything, just override the data
            Debug.Log($"New color saved");
        }
        public static List<ColorData> LoadColorData()
        {
            if (FileHandler.JSONFileExists(SavedColorsFileName) == false) return null;

            var loadedData = FileHandler.ReadFromJSON<ColorData>(SavedColorsFileName);
            return loadedData;
        }
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

            Debug.Log($"LoadedData is {loadedData[0].Stage}");

            // since we have only one data instance and also we're saving data as overriding, we need the first index of the data for now atleast
            return loadedData[0];
        }

        public static bool FileExists() => FileHandler.JSONFileExists(FileName);
        public static bool ColorFileExists() => FileHandler.JSONFileExists(SavedColorsFileName);
    }
}

