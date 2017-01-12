using UnityEngine;
using UnityEngine.UI;

public class ColorThemeTextSetter : ColorThemeSetter
{
    [SerializeField]
    TextAttributePair[] changeableTextFields;

    protected override void SetColor(ColorTheme theme)
    {
        foreach (var p in changeableTextFields)
        {
            foreach (Text s in p.sprites)
                s.color = theme.GetColor(p.colorType) + new Color(0, 0, 0,
                -1 + s.color.a);
        }
    }
}

[System.Serializable]
public class TextAttributePair
{
    public Text[] sprites;
    public ColorTheme.EntityType colorType;
}