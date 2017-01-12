using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Connect : MonoBehaviour {

    [SerializeField]
    Text connectionText;

    void OnOpened()
    {
        if (ConnectionManager.cognitoID == null)
            TryConnect();
        else
            OnConnected();
    }

	// Use this for initialization
	public void TryConnect ()
    {
        connectionText.text = "Connecting...";
        ConnectionManager.Connect( (successful) =>
            {
                if(!successful)
                {
                    connectionText.text = "Failed to connect :(";
                    Invoke("BackToMainMenu", 1f);
                }
                else
                {
                    OnConnected();
                }
            }
        );
	}

    void OnConnected()
    {
        Lambdas.GetPlayerName((name) =>
        {
            connectionText.text = "Connected!";
            if (name == null) Debug.LogError("Name null??");

            if (name == "N/A")
            {
                MenuManager.Instance.OpenMenu("Name");
            }
            else
            {
                MenuManager.Instance.OpenMenu("Community");
            }
        });
    }

    void BackToMainMenu()
    {
        MenuManager.Instance.OpenMenu("Main");
    }
}
