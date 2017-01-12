using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BTN_Community : MonoBehaviour {

    public void Click()
    {
        if(Lambdas.cachedPlayerName == "N/A")
        {
            MenuManager.Instance.OpenMenu("Connecting");
        }
        else
        {
            MenuManager.Instance.OpenMenu("Community");
        }
    }
}
