using UnityEngine;

public class ColorThemePlayerSetter : ColorThemeSetter
{
    [Range(0, 1), SerializeField]
    float intensity = 0.20f;

    protected override void SetColor(ColorTheme theme)
    {
        if (this == null) return;

        var color = theme.GetColor(ColorTheme.EntityType.levelBackgroundColor);
        HSLColor lighter = HSLColor.FromRGBA(color);
        lighter.l = Mathf.Max(lighter.l, 1-intensity);

        GetComponent<MeshRenderer>().material.color = lighter;
    }
}
