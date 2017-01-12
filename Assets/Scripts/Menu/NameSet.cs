using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NameSet : MonoBehaviour {

    [SerializeField]
    Text message;

    [SerializeField]
    InputField nameField;

    public void OnReadyOpen()
    {
        GetComponentInChildren<Button>().interactable = true;
        Lambdas.GetPlayerName((name) =>
        {
            if(name == "N/A")
            {
                message.text = "You don't have a name yet!";
            }
            else
            {
                message.text = "What's wrong with \"" + name + "\"?";
            }
        });
    }

    public void SetName()
    {
        if(nameField.text == "")
        {
            message.text = "Anonymous? I don't think so >:^)";
            return;
        }
        
        if (nameField.text.ToLower().Contains("nigger"))
        {
            nameField.text = nameField.text.Replace("nigger", "roodypoo");
        }

        if(nameField.text.ToLower().Contains("faggot"))
        {
            nameField.text = nameField.text.Replace("faggot", "candy-ass");
        }


        if (nameField.text.ToLower().Contains("cuck"))
        {
            nameField.text = nameField.text.Replace("cuck", "kek");
        }
        

        if (nameField.text.ToLower() == "vine" || nameField.text.ToLower() == "ghor")
        {
            message.text = "Piss off go work on cavern kings";
        }

        if (nameField.text.ToLower() == "thumbtack" || nameField.text.ToLower() == "thumb")
        {
            message.text = "WHAP";
        }
        
        if (nameField.text.ToLower() == "framk")
        {
            message.text = "[youtube link to bad music]";
        }

        if (nameField.text.ToLower() == "brum")
        {
            message.text = "Nice dev";
        }

        if (nameField.text.ToLower() == "mung" || nameField.text.ToLower() == "liquorish" || nameField.text.ToLower() == "liqo" || nameField.text.ToLower() == "liq"
            || nameField.text.ToLower() == "tjern" || nameField.text.ToLower() == "synnro")
        {
            message.text = "Min svävare är full av ålar";
        }

        if (nameField.text.ToLower().Contains("strato"))
        {
            message.text = "There are no pantsu here";
        }
        
        if (nameField.text.ToLower().Contains("soi"))
        {
            message.text = "tfw";
        }

        if (nameField.text.ToLower().Contains("wabba"))
        {
            message.text = "i!i!i! jajajajaJA AYAYAYi!";
        }

        if (nameField.text.ToLower() == "lab" || nameField.text.ToLower() == "labyrinth")
        {
            message.text = "u gonna heat this stale gameplay\non a frying pan too?";
        }

        if (nameField.text.ToLower() == "keeps" || nameField.text.ToLower() == "keepee")
        {
            message.text = "100% vegan gameplay";
        }

        if (nameField.text.ToLower() == "chao" || nameField.text.ToLower() == "chaoclypse")
        {
            message.text = "Singaporean Special";
        }

        if(nameField.text.ToLower() == "sev" || nameField.text.ToLower() == "pugdev")
        {
            message.text = "more like DEVenteenuncles amirite lol";
        }

        if (nameField.text.ToLower().Contains("seventeen"))
        {
            message.text = "sorry sev I knew you'd\nrun into the name length restriction";
        }

        if (nameField.text.Length > 11)
        {
            message.text = "A bit long, don't you think?";
            return;
        }

        GetComponentInChildren<Button>().interactable = false;
        Lambdas.SetOrUpdatePlayerName(nameField.text, (msg) =>
        {
            if(msg == "Success")
            {
                Invoke("Continue", 1f);
            }
            else
            {
                message.text = msg;
                if (msg != "Name already in use")
                    Invoke("BackToMain", 1f);
                else
                    GetComponentInChildren<Button>().interactable = true;
            }
        });
    }

    void BackToMain()
    {
        MenuManager.Instance.OpenMenu("Main");
    }

    void Continue()
    {
        MenuManager.Instance.OpenMenu("Community");
    }
}
