using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ColorPreview : MonoBehaviour {

    [SerializeField]
    List<Image> colorBoxes;
    [SerializeField]
    Text text;

    ColorTheme theme;

    public void SetColors(ColorTheme theme)
    {
        this.theme = theme;
        for(int i = 0; i < colorBoxes.Count; i++)
        {
            colorBoxes[i].color = theme.GetColor((ColorTheme.EntityType)i);
        }
        text.text = theme.name;
        text.color = theme.GetColor(ColorTheme.EntityType.levelColorComplimentary);
        text.GetComponent<Outline>().effectColor = theme.GetColor(ColorTheme.EntityType.levelColorMid);
    }

    public void OnSelect()
    {
        ColorManager.Instance.selectedTheme = theme;
    }
}
