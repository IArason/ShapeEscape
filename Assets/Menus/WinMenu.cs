using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class WinMenu : MonoBehaviour {

    public GameObject menu;
    public GameObject backToMenuButton;
    public Text timeDisplayGame;
    public Text timeDisplayMenu;
    float timeSinceStart = 0;

    public GameObject recordTime;
    
    bool timeRunning = true;

    public void Win()
    {
        timeRunning = false;
        var mins = (int)(timeSinceStart / 60);
        var secs = Mathf.Floor(timeSinceStart % 60f);
        timeDisplayMenu.gameObject.SetActive(true);
        timeDisplayMenu.text = string.Format("{0:00}:{1:00}", mins, secs);

        menu.SetActive(true);
        FindObjectOfType<Player>().enabled = false;
        timeDisplayGame.gameObject.SetActive(false);

        // If doing publish test, use Publish function instead
        var pub = FindObjectOfType<LevelPublisher>();
        if (pub != null)
        {
            pub.Publish(timeSinceStart, () =>
            {
                // Enables back to menu button only when publishing is done
                backToMenuButton.SetActive(true);
            }
            );
        }

        var level = LevelSerializer.GetCurrentLevel();
        var shlvl = GameManager.cachedLevels.Any<SharedLevel>(x => x.LevelName == level.levelName);
        if (shlvl)
        {
            GameManager.cachedLevels.First(x => x.LevelName == level.levelName).personalBest = timeSinceStart;
        }

        if (backToMenuButton != null)
            backToMenuButton.SetActive(true);
        StartCoroutine(DelayedBackToMenu());
    }

    IEnumerator DelayedBackToMenu()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            if (Input.anyKeyDown)
            {
                BackToMenu();
            }
            yield return new WaitForEndOfFrame();
        }
    }
    
    public void BackToMenu()
    {
        bool record = GameManager.CompleteLevel(timeSinceStart);
        if(recordTime != null)
            recordTime.SetActive(record);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!timeRunning) return;
        timeSinceStart += Time.deltaTime;
        if (timeSinceStart > 60 * 60 - 1)
        {
            timeSinceStart = 60 * 60 - 1;
        }
        
        var mins = (int)(timeSinceStart / 60);
        var secs = Mathf.Floor(timeSinceStart % 60f);
        timeDisplayGame.text = string.Format("{0:00}:{1:00}", mins, secs);
	}
}
