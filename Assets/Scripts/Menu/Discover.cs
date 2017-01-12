using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
public class Discover : MonoBehaviour {

    [SerializeField]
    Text loadingText;
    [SerializeField]
    Transition transition;

    Level level;

	public void DiscoverLevel()
    {
        loadingText.text = "Getting Level...";
        foreach (var v in transform.parent.GetComponentsInChildren<Button>())
        {
            v.interactable = false;
        }

        MenuManager.Instance.OpenMenu("LoadingLevel");
        Lambdas.Discover((lv) =>
        {
            if (lv.LevelID == null)
            {
                loadingText.text = "Something broke!!!!!";
                MenuManager.Instance.OpenMenu("Main");
                return;
            }
            loadingText.text = "Loading...";
            Lambdas.GetLevelData(lv, () =>
            {
                loadingText.text = "Loaded!\nStarting level \"" + lv.LevelName + "\" by\n\""
                + lv.AuthorName + "\"";
                lv.ToLevel((level) =>
                {
                    transition.OutroTransition();
                    StartCoroutine(LevelSerializer.Instance.PlaySharedLevel(level));
                });
            });
        }
        );
    }
}
