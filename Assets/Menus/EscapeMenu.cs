using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EscapeMenu : MonoBehaviour {

    public static bool open = false;

    public GameObject escapeMenu;

	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Esc pressed");

            // Don't activate it in main menu
            if (SceneManager.GetActiveScene().buildIndex == 0) return;

            escapeMenu.SetActive(!escapeMenu.activeSelf);
            open = escapeMenu.activeSelf;

            PausePhysics(escapeMenu.activeSelf);

        }
	}

    public void Disable()
    {
        escapeMenu.SetActive(false);
        open = false;
    }

    void PausePhysics(bool val)
    {
        Time.timeScale = val ? 0 : 1;
    }
}
