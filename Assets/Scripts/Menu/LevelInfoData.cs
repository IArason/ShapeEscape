using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LevelInfoData : MonoBehaviour {

    [SerializeField]
    Button playButton;
    [SerializeField]
    GetShareableLink linkButton;


    public Sprite[] profileIcons;

    public Image[] fill;
    public Image[] outline;
    public Text[] texts;
    
    public Text playerNameText;
    public Text levelNameText;
    public Text playerTimeText;
    public Text playCountText;
    
    public void SetOnPlay(UnityAction action)
    {
        playButton.onClick.AddListener(action);
    }

    public void SetLevelData(SharedLevel level)
    {
        var mins = (int)(level.personalBest / 60);
        var secs = Mathf.Floor(level.personalBest % 60f);

        playerTimeText.text = string.Format("{0:00}:{0:00}", mins, secs);
        playCountText.text = "" + level.Popularity;
        levelNameText.text = level.LevelName;
        playerNameText.text = "" + level.AuthorName;

        linkButton.gameObject.SetActive(true);
        linkButton.SetLevel(level);
    }

    public void SetLevelData(Level level)
    {
        playerTimeText.text = "00:00";
        playCountText.text = "N/A";
        levelNameText.text = level.levelName;
        playerNameText.text = Lambdas.cachedPlayerName == "N/A" ? "You!" : Lambdas.cachedPlayerName;

        linkButton.gameObject.SetActive(false);
    }

    void Awake()
    {
        foreach (var v in texts)
            MainMenuColors.Instance.ColorText(v);
        foreach (var v in outline)
            MainMenuColors.Instance.ColorOutline(v);
        foreach (var v in fill)
            MainMenuColors.Instance.ColorFill(v);
    }
}
