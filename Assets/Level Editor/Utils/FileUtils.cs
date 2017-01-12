using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using Object = UnityEngine.Object;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

public static class FileUtils {

    static string EscapeFileName(string filename)
    {
        string illegal = "\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?";
        string regexSearch = filename;
        Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
        filename = r.Replace(illegal, "");
        return filename;
    }

    public static Dictionary<string, FileInfo> GetSavedLevels()
    {
        var directory = new DirectoryInfo(Application.persistentDataPath + "/CustomLevels");
        var fileInfo = directory.GetFiles();
        Dictionary<string, FileInfo> dict = new Dictionary<string, FileInfo>();
        foreach (FileInfo file in fileInfo)
        {
            dict.Add(file.Name, file);
        }
        return dict;
    }

    public static void SaveLevelToPersistentPath(string fileName, string json)
    {
        var path = Application.persistentDataPath + "/CustomLevels/";
        WriteLevelToPath(fileName, json, path);
    }

    /// <summary>
    /// For editor use only.
    /// </summary>
    public static void SaveLevelToResources(string fileName, string json)
    {
        fileName = EscapeFileName(fileName);

        if (!Application.isEditor) return;

        #if UNITY_EDITOR
        string path = null;
        path = "Assets/Resources/StoredLevels/";
        WriteLevelToPath(fileName, json, path);
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }

    static void WriteLevelToPath(string fileName, string json, string path, string extension = ".json")
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var fullPath = path + fileName + extension;
        if (File.Exists(fullPath))
        {
            // If the file exists, create a backup
            Backup(path, fileName);
        }

        // Backup moves it, so we recreate it.
        File.Create(fullPath).Dispose();

        using (StreamWriter sw = new StreamWriter(fullPath, false))
        {
            sw.Write(json);
            sw.Close();
        }
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public static string LoadLevelFromResources(string fileName)
    {
        return Resources.Load<TextAsset>("StoredLevels/" + fileName).text;
    }

    public static string[] GetLevelsInPersistentPath()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/CustomLevels/");
        var fileinfo = dir.GetFiles();
        string[] files = fileinfo.Select(x => x.Name).ToArray();

        // Strip file extension
        files = files.Select(x => x.Substring(0, x.LastIndexOf('.'))).ToArray();
        
        return files;
    }

    public static string LoadLevelFromPersistentPath(string fileName)
    {
        var path = Application.persistentDataPath + "/CustomLevels/" + fileName + ".json";
        return LoadLevelFromFile(path);
    }

    static string LoadLevelFromFile(string path)
    {
        if (File.Exists(path))
        {
            var sr = File.OpenText(path);
            return sr.ReadToEnd();
        }
        return "";
    }

    public static void DeleteLevel(string level)
    {
        var path = Application.persistentDataPath + "/CustomLevels/" + level + ".json";

        File.Delete(path);
    }

    public static List<Level> GetAllLevelsInPath(string path)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        var fileinfo = dir.GetFiles();
        string[] files = fileinfo.Select(x => x.FullName).ToArray();
        
        List<Level> levels = new List<Level>();
        foreach(string st in files)
        {
            var json = LoadLevelFromFile(st);
            levels.Add(JsonConvert.DeserializeObject<Level>(json,
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }));
        }
        return levels;
    }

    static void Backup(string directory, string levelName)
    {
        if (!Directory.Exists(directory + "/" + levelName))
        {
            Debug.Log("Creating dir");
            Directory.CreateDirectory(directory + "/" + levelName);
            Debug.Log("Finished creating dir");
        }

        var newFilePath = directory +"/" + levelName + "/" + levelName + " " + 
            DateTime.Now.ToString("dd'-'MM'-'yyyy_HHmmss") + " " + UnityEngine.Random.Range(0, 99999) + ".json";

        //File.Create(newFilePath).Dispose();
        if (File.Exists(newFilePath + "/" + levelName + ".json"))
            return;

        File.Move(directory + "/" + levelName + ".json", newFilePath);
    }

    public static void StoreColorThemes(string colorJson)
    {
        WriteLevelToPath("themes", colorJson, "Assets/Resources/", ".json");
    }

    public static string LoadColorThemes()
    {
        return Resources.Load<TextAsset>("themes").text;
    }
}
