using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class LoadShareableLink : MonoBehaviour {

    [SerializeField]
    InputField input;

    public void Click()
    {
        int result;
        if (!int.TryParse(input.text, out result))
        {
            input.text = "Invalid!";
            return;
        }
        
        Lambdas.GetLevelFromShareableLink(input.text, (lv) =>
        {
            if(lv == null)
            {
                input.text = "Invalid!";
                return;
            }
            Debug.Log("Got level");
            MenuManager.Instance.OpenMenu("LoadingLevel");
            LoadingLevel.Instance.SetLoadMessage(lv);
            Debug.Log("Did stuff");
            lv.ToLevel((level) =>
            {
                StartCoroutine(LevelSerializer.Instance.PlaySharedLevel(level));
            });
        });
    }
}
