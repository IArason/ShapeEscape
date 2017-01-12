using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BTN_LevelEditor : MonoBehaviour
{
    public void LoadLevelEditor()
    {
        SceneManager.LoadScene("LevelEditorNew");
    }
}
