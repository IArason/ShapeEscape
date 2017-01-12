using UnityEngine;

public class ColorThemePolygonSetter : ColorThemeSetter
{
    [SerializeField]
    MeshRenderer polygonRenderer;

    protected override void SetColor(ColorTheme theme)
    {
        var themeMat = theme.GetPolyMaterial();
        
        if(themeMat != null && themeMat.shader.name == "Unlit/UnlitTriplanar")
            polygonRenderer.material = themeMat;

        base.SetColor(theme);
    }
}
