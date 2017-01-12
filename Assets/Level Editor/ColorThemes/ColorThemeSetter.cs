using UnityEngine;
using System;

public class ColorThemeSetter : MonoBehaviour
{
    static event Action<ColorTheme> setColors;
    protected static ColorTheme currentTheme;

    public static void UpdateColors(ColorTheme theme)
    {
        currentTheme = theme;
        setColors.Invoke(theme);
    }

    [SerializeField]
    RendererAttributePair[] changeableAttributes = 
        new RendererAttributePair[] { new RendererAttributePair() };

    void OnEnable()
    {
        setColors += SetColor;

        if (currentTheme != null)
            SetColor(currentTheme);
    }

    void OnDestroy()
    {
        setColors -= SetColor;
    }

    protected virtual void SetColor(ColorTheme theme)
    {
        foreach(var pair in changeableAttributes)
        {
            var color = theme.GetColor(pair.colorType);
            foreach(string s in pair.attributes)
            {
                foreach(Material m in pair.renderer.materials)
                {
                    // Preserve alpha
                    m.SetColor(s, color + new Color(0, 0, 0,
                        (-1 + m.GetColor(s).a))); // - 1 + because adding color with 1 A, so (1 - 1 + alpha)
                }

            }
        }
    }
}

[Serializable]
public class RendererAttributePair
{
    public Renderer renderer;
    public ColorTheme.EntityType colorType;
    public string[] attributes = new string[] { "_Color" };
}

