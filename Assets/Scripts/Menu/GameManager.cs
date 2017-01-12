using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager> {

    public static SharedLevel currentLevel;

    public static HashSet<SharedLevel> cachedLevels = new HashSet<SharedLevel>();

    public static void PlayLevel(SharedLevel level)
    {
        cachedLevels.Add(level);
        currentLevel = level;
        level.ToLevel((lv) =>
        {
            LevelSerializer.Instance.PlayLevel(lv);
        });
    }


    public static void CacheLevel(SharedLevel level)
    {
        if (!cachedLevels.Any(x => x.LevelID == level.LevelID))
            cachedLevels.Add(level);
        else
        {
            var existingLevel = cachedLevels.First(x => x.LevelID == level.LevelID);

            existingLevel.LevelJSON = level.LevelJSON;
            existingLevel.LevelName = level.LevelName;
            existingLevel.personalBest = level.personalBest;
            existingLevel.AuthorName = level.AuthorName;
            existingLevel.Popularity = level.Popularity;
        }

    }

    public static bool CompleteLevel(float time)
    {
        if(currentLevel.personalBest > time)
        {
            currentLevel.personalBest = time;
            return true;
        }
        return false;
    }

    static void LoadMain()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

}