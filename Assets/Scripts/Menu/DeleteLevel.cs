using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeleteLevel : MonoBehaviour {

    [SerializeField]
    GameObject confirmButton;

    public void AreYouSure()
    {
        confirmButton.SetActive(!confirmButton.activeSelf);
    }

    public void Confirm()
    {
        confirmButton.SetActive(false);
        LevelSerializer.Instance.DeleteLevel();
    }
}
