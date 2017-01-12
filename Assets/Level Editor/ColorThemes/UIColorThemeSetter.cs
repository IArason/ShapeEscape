using UnityEngine;
using UnityEngine.UI;

public class UIColorThemeSetter : ColorThemeSetter
{
    [SerializeField]
    SpriteAttributePair[] changeableSprites;
    
    protected override void SetColor(ColorTheme theme)
    {
        foreach(var p in changeableSprites)
        {
            foreach (Image s in p.sprites)
            {
                if (s == null) continue;

                s.color = theme.GetColor(p.colorType) + new Color(0, 0, 0,
                -1 + s.color.a);
            }
        }
    }
}

[System.Serializable]
public class SpriteAttributePair
{
    public Image[] sprites;
    public ColorTheme.EntityType colorType;
}