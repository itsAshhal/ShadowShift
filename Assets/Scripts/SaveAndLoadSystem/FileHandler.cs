using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

public static class FileHandler
{
    // Method to save data to JSON file
    public static void SaveToJSON<T>(List<T> toSave, string fileName, bool append = false)
    {
        Debug.Log($"getting path->{GetPath(fileName)}");
        string content = JsonHelper.ToJson(toSave.ToArray());
        WriteToFile(GetPath(fileName), content, append);
        Debug.Log($"write to file called");
    }

    // Method to read data from JSON file
    public static List<T> ReadFromJSON<T>(string filename)
    {
        string content = ReadFile(GetPath(filename));
        if (string.IsNullOrEmpty(content))
        {
            // Returning an empty list if file content is empty
            return new List<T>();
        }
        List<T> res = new List<T>();
        string[] lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            res.AddRange(JsonHelper.FromJson<T>(line));
        }
        return res;
    }

    // Method to check if a JSON file exists
    public static bool JSONFileExists(string fileName)
    {
        return File.Exists(GetPath(fileName));
    }

    // Method to get the path to the file
    private static string GetPath(string fileName)
    {
        // Works for all devices that Unity supports
        return Application.persistentDataPath + "/" + fileName;
    }

    // Method to write to the file
    private static void WriteToFile(string path, string content, bool append)
    {
        // Determine the file mode based on the append parameter
        FileMode fileMode = append ? FileMode.Append : FileMode.Create;

        // Create or open the file
        using (FileStream fileStream = new FileStream(path, fileMode))
        {
            // Ensure that the StreamWriter closes after writing
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                // Write the content to the file
                writer.WriteLine(content);
            }
        }

        Debug.Log($"write to file saved");
    }

    // Method to read the file content
    private static string ReadFile(string path)
    {
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }
        return "";
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("JSON input is empty or null.");
            return new T[0]; // Return an empty array if JSON is empty or null
        }

        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper?.Items ?? new T[0]; // Return the items or an empty array if wrapper is null
    }


    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
