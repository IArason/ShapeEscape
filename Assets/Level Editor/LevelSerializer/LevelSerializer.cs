using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Serializes a level and loads them using FileUtils.
/// TODO: Name validation/overwrite warning.
/// </summary>
public class LevelSerializer : Singleton<LevelSerializer> {

    [SerializeField]
    Text message;

    static string storedLevelName = "Level ";

    [SerializeField]
    InputField levelNameField;

    // Cached level for loading on level load.
    static Level cachedLevel;

    // Whether or not the level should be loaded as editor.
    public static bool levelPlayable;
    
    void OnLevelWasLoaded(int scene)
    {
        if (cachedLevel != null)
        {
            RefreshLevelName();
            storedLevelName = cachedLevel.levelName;
            if(levelNameField != null)
                levelNameField.text = storedLevelName;
            cachedLevel.LoadLevel(levelPlayable);
        }
        else
        {
            if (levelNameField != null)
                levelNameField.text = "Level " + Random.Range(0, 9999);
        }
    }

    /// <summary>
    /// Saves the level to a static Level variable and nothing else.
    /// </summary>
    public void SaveLevel(string levelName = "")
    {
        if (levelName == "") levelName = storedLevelName;
        else
        {
            storedLevelName = levelName;
        }

        ObjectManipulator.Instance.ClearSelection();

        cachedLevel = new Level(levelName);
        cachedLevel.SaveLevel();
    }

    /// <summary>
    /// Loads the current cached level.
    /// EmptyPlay is an empty scene, while LevelEditor contains
    /// an object for manipulating the level.
    /// </summary>
    public void PlayLevel(bool playable)
    {
        if(playable && SceneManager.GetActiveScene().name != "MainMenu")
            SaveToFile();

        if (cachedLevel == null) return;

        levelPlayable = playable;

        if (levelPlayable)
            SceneManager.LoadScene("EmptyPlay");
        else
            SceneManager.LoadScene("LevelEditor");
    }


    /// <summary>
    /// Loads level from persistent path.
    /// </summary>
    public void LoadLevelFromFile(string levelName)
    {
        cachedLevel = DeserializeLevel(FileUtils.LoadLevelFromPersistentPath(levelName));
        Debug.Log(cachedLevel);
        storedLevelName = cachedLevel.levelName;
        levelNameField.text = cachedLevel.levelName;
    }

    /// <summary>
    /// Loads level from resources folder.
    /// </summary>
    public void LoadLevelFromResources(string levelName)
    {
        cachedLevel = DeserializeLevel(FileUtils.LoadLevelFromResources(levelName));
        if (cachedLevel == null) Debug.LogError("Failed to load level at " + levelName);
        if(levelNameField != null)
            levelNameField.text = cachedLevel.levelName;
    }

    public void PlayLevel(Level level, bool levelPlayable = true)
    {
        cachedLevel = level;
        PlayLevel(levelPlayable);
    }


    public IEnumerator PlaySharedLevel(Level level)
    {
        cachedLevel = level;

        if (cachedLevel == null) yield break;

        levelPlayable = true;
        
        yield return new WaitForSeconds(2);
        
        SceneManager.LoadScene("EmptyPlayShared");
    }

    public void PublishLevel()
    {
        if (ConnectionManager.cognitoID == null)
        {
            ConnectionManager.Connect((successful) =>
            {
                if (successful)
                {
                    Publish();
                }
                else
                {
                    message.text = "Unable to connect!";
                }
            }
            );
        }
        else
        {
            Publish();
        }
    }

    void Publish()
    {
        RefreshLevelName();
        storedLevelName = levelNameField.name;
        Debug.Log("Publishing");
        SaveToFile();
        Debug.Log("Saved to file: " + cachedLevel);

        levelPlayable = true;

        SceneManager.LoadSceneAsync("EmptyPlayPublishPlaytest");
    }

    /// <summary>
    /// Saves the level to persistent path.
    /// </summary>
    public void SaveToFile()
    {
        RefreshLevelName();
        SaveLevel();
        FileUtils.SaveLevelToPersistentPath(storedLevelName, GetLevelJSON());
        message.text = "Saved!";
        Debug.Log("Wrote to file");
    }

    /// <summary>
    /// Saves the level to resouces.
    /// </summary>
    public void SaveToResources()
    {
        SaveLevel();
        FileUtils.SaveLevelToResources(storedLevelName, GetLevelJSON());
    }

    public void RefreshLevelName()
    {
        if (levelNameField == null) return;
        if(levelNameField.text == "")
        {
            levelNameField.text = storedLevelName;
        }
        else
        {
            if (storedLevelName != levelNameField.text)
                DeleteLevel();
            storedLevelName = levelNameField.text;
        }
    }

    public static Level GetCurrentLevel()
    {
        return cachedLevel;
    }

    public void DeleteLevel()
    {
        if (GetCurrentLevel() == null) return;
        message.text = "Deleted!";
        FileUtils.DeleteLevel(GetCurrentLevel().levelName);
    }

    /// <summary>
    /// Returns a JSON representation of the currently static stored level.
    /// </summary>
    string GetLevelJSON()
    {
        var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };
        serializer.NullValueHandling = NullValueHandling.Ignore;

        return JsonConvert.SerializeObject(cachedLevel, Formatting.None, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });
    }

    public Level DeserializeLevel(string json)
    {
        return JsonConvert.DeserializeObject<Level>(json,
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
    }
}