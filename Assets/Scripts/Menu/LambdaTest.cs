using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class LambdaTest : MonoBehaviour {

    public string username = "my name";
    [BitStrap.Button]
    void SetOrUpdatePlayerName()
    {
        Lambdas.SetOrUpdatePlayerName(username, (v) => { });
    }
    [BitStrap.Button]
    void GetPlayerName()
    {
        Lambdas.GetPlayerName((myName) => { Debug.Log(myName); username = myName; });
    }

    [Space]
    [Space]

    public string levelName;
    public TextAsset levelData;
    public string levelID;
    

    [BitStrap.Button]
    void GetPlayerLevels()
    {
        Lambdas.GetPlayerLevels((levels) =>
        {
            cachedLevels = new List<SharedLevel>(levels);
        });
    }

    [Space]
    [Space]

    public List<SharedLevel> cachedLevels;
    public SharedLevel selectedLevel;

    [BitStrap.Button]
    void GetLevelData()
    {
        Lambdas.GetLevelData(selectedLevel, () =>
        {
            try
            {
                var Level = LevelSerializer.Instance.DeserializeLevel(selectedLevel.LevelJSON);
                Debug.Log("Successfully deserialized level");
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
        });
    }

    [BitStrap.Button]
    void DeleteLevel()
    {
        Lambdas.DeleteLevel(selectedLevel);
    }

    [Space]
    [Space]
    public string shareableLink;

    [BitStrap.Button]
    void GetShareableLink()
    {
        Lambdas.GetShareableLink(selectedLevel, (link) =>
        {
            shareableLink = link;
        });
    }

    [BitStrap.Button]
    void GetLevelFromLink()
    {
        Lambdas.GetLevelFromShareableLink(shareableLink, (level) =>
        {
            Debug.Log(level.ToString());
            selectedLevel = level;
        });
    }

    [Space]
    [Space]
    public int rating = 1;

    [BitStrap.Button]
    void RateLevel()
    {
        Lambdas.AddOrUpdateRating(selectedLevel, rating);
    }
}
