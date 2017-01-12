using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class LevelPublisher : MonoBehaviour {

	// Update is called once per frame
	public void Publish (float time, Action callback)
    {
        var lvl = LevelSerializer.GetCurrentLevel();

        SharedLevel sharedLevel = new SharedLevel()
        {
            AuthorName = Lambdas.cachedPlayerName,
            LevelName = lvl.levelName,
            personalBest = time,
            LevelJSON = lvl.Serialize()
        };

        Lambdas.AddOrUpdateLevel(sharedLevel, (msg) =>
        {
            Debug.Log(msg);
            callback();
        });

	}
}
