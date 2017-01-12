using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.UI;
using BitStrap;

public class ColorManager : Singleton<ColorManager> {
    
    [SerializeField]
    [TextArea]
    string help = "1 - Color order:\n" +
        "2 - polygonFrontColor\n" +
        "3 - polygonSideColor\n" +
        "4 - polygonTopColor\n" +
        "5 - levelColorDark\n" +
        "6 - levelColorMid\n" +
        "7 - levelColorLight\n" +
        "8 - levelColorComplimentary\n" +
        "9 - levelBackgroundColor";

    [SerializeField]
    ColorTheme[] colorThemes;
    
    public ColorTheme selectedTheme
    {
        get { return _selectedTheme; }
        set { _selectedTheme = value; UpdateColors(); }
    }
    ColorTheme _selectedTheme;

    [SerializeField]
    GameObject colorPreviewPrefab;

    [SerializeField]
    Transform colorPreviewRoot;

    

    void Start()
    {
        LoadColorThemes();

        if (LevelSerializer.levelPlayable) return;

        for (int i = 0; i < colorThemes.Length; i++)
        {
            var preview = Instantiate(colorPreviewPrefab);
            preview.transform.SetParent(colorPreviewRoot, false);
            preview.GetComponent<RectTransform>().anchoredPosition += Vector2.down * i * 25f;
            preview.GetComponent<ColorPreview>().SetColors(colorThemes[i]);
        }

        if (selectedTheme == null)
            selectedTheme = colorThemes[0];
    }

    [Button]
    void TestColor()
    {
        UpdateColors();
    }

    [Button]
    void SaveColorThemes()
    {
        var collection = new SerializedThemeCollection();
        collection.themes = new ColorTheme.Serialized[colorThemes.Length];
        for(int i = 0; i < colorThemes.Length; i++)
        {
            collection.themes[i] = colorThemes[i].Serialize();
        }

        var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };
        serializer.NullValueHandling = NullValueHandling.Ignore;

        var json = JsonConvert.SerializeObject(collection, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });

        FileUtils.StoreColorThemes(json);
    }

    [Button]
    void LoadColorThemes()
    {
        var json = FileUtils.LoadColorThemes();
        var themes = JsonConvert.DeserializeObject<SerializedThemeCollection>
            (json,
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }).themes;

        colorThemes = themes.Select(x => x.Deserialize()).ToArray();

    }

    // Old stuff
    public void UpdateHSLColors()
    {
        /*
        HSLColor mainColor = new HSLColor(hueSlider.value, saturationSlider.value, valueSlider.value);
        HSLColor opposite = new HSLColor(Mathf.Abs(mainColor.h + 0.5f) % 1, mainColor.s, mainColor.l);

        var o = mainColor.h - 0.1f > 0 ? (mainColor.h - 0.1f) : mainColor.h + 0.9f;
        var t = mainColor.h + 0.1f < 1 ? (mainColor.h + 0.1f) : mainColor.h - 0.9f;

        HSLColor complementaryOne = new HSLColor(o, mainColor.s, mainColor.l);
        HSLColor complementaryTwo = new HSLColor(t, mainColor.s, mainColor.l);

        var n = new ColorTheme();
        n.SetColors(new Color[]
        {
            mainColor.HSVToRGB(),
            opposite.HSVToRGB(),
            complementaryOne.HSVToRGB(),
            complementaryTwo.HSVToRGB(),
            mainColor.HSVToRGB(),
            mainColor.HSVToRGB()
        });

        Debug.Log(mainColor.ToRGBA());
        selectedTheme = n;
        */
    }

    void UpdateColors()
    {
        ColorThemeSetter.UpdateColors(_selectedTheme);
    }

    public class SerializedThemeCollection
    {
        public ColorTheme.Serialized[] themes;
    }
}
