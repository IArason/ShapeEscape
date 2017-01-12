// Intermediary class for networked levels.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[System.Serializable]
public class SharedLevel
{
    public SharedLevel() { }

    public SharedLevel(Dictionary<string, string> values)
    {
        if (values.ContainsKey("PlayerName"))
            AuthorName = values["PlayerName"];
        if (values.ContainsKey("LevelName"))
            LevelName = values["LevelName"];
        if (values.ContainsKey("LevelID"))
            LevelID = values["LevelID"];
        if (values.ContainsKey("PlaythroughCount"))
            Popularity = values["PlaythroughCount"];
    }

    public string AuthorName;

    public string LevelName;
    
    public bool isLoaded { get { return LevelJSON != null; } }

    [UnityEngine.HideInInspector]
    public string LevelJSON;

    public string LevelID;

    public float personalBest;

    public string Popularity;

    public void ToLevel(Action<Level> callback)
    {
        if(isLoaded)
            callback(
               Deserialize()
            );
        else
        {
            Lambdas.GetLevelData(this, () =>
            {
                callback(Deserialize());
            });
        }
    }

    Level Deserialize()
    {
        return JsonConvert.DeserializeObject<Level>(LevelJSON,
                       new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
    }

    public void Rate(Tag tag)
    {
        // Todo
    }

    public override string ToString()
    {
        return
            "Level Name: " + LevelName +
            "\nLevel Author: " + AuthorName +
            "\nLevel ID: " + LevelID;
    }

    public enum Tag
    {
        Challenging,
        Creative,
        Fun
    }
}
