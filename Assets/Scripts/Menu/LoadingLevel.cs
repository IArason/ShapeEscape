using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LoadingLevel : Singleton<LoadingLevel> {
    [SerializeField]
    Text text;
    [SerializeField]
    Transition transition;

    public void SetLoadMessage(SharedLevel level)
    {
        text.text = "Loading \"" + level.LevelName + "\" \nby \"" +
                level.AuthorName + "\"";
        transition.OutroTransition();
    }
}
