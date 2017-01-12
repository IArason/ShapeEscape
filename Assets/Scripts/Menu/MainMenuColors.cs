using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuColors : Singleton<MainMenuColors> {
    public Color textCol;
    public Color shadowCol;

    public Image[] outlineImgs;
    public Image[] fillImgs;

  
    


	// Use this for initialization
	void Start () {
        foreach (Text text in FindObjectsOfType<Text>())
        {
            ColorText(text);
        }

        foreach (Image r in outlineImgs)
        {
            r.color = shadowCol;
        }
        foreach (Image r in fillImgs)
        {
            r.color = textCol;
        }
    }

    public void ColorText(Text text)
    {
        text.color = textCol;
        var shadow = text.GetComponent<Shadow>();
        var outline = text.GetComponent<Outline>();
        if (shadow != null) shadow.effectColor = shadowCol;
        if (outline != null) outline.effectColor = shadowCol;
    }

    public void ColorFill(Image image)
    {
        image.color = textCol;
    }

    public void ColorOutline(Image image)
    {
        image.color = shadowCol;
    }
}
