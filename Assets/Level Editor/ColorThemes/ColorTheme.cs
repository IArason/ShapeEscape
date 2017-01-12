using UnityEngine;

[System.Serializable]
public class ColorTheme
{
    public string name;

    [SerializeField]
    Color[] colors;

    [SerializeField]
    string polygonMaterial;
    
    Material loadedMat;

    public Color GetColor(EntityType type)
    {
        if(colors.Length <= (int)type)
        {
            Debug.Log("Color selected of bounds " + colors.Length);
            return Color.green;
        }
        return colors[(int)type];
    }

    public Material GetPolyMaterial()
    {
        if(loadedMat == null)
            loadedMat = Resources.Load<Material>("PolygonMaterials/" + polygonMaterial);
        return loadedMat;
    }

    public void SetColors(Color[] colors)
    {
        this.colors = colors;
    }

    public enum EntityType
    {
        polygonFrontColor = 0,
        polygonSideColor = 1,
        polygonTopColor = 2,

        levelColorDark = 3,
        levelColorMid = 4,
        levelColorLight = 5,

        levelColorComplimentary = 6,

        levelBackgroundColor = 7
    }

    public Serialized Serialize()
    {
        return new Serialized(colors, polygonMaterial, name);
    }

    public class Serialized
    {
        public SerializedVector3[] colors;
        public string material;
        public string name;

        public Serialized() { }

        public Serialized(Color[] colors, string material, string name)
        {
            this.colors = new SerializedVector3[colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                this.colors[i] = new SerializedVector3(
                    new Vector3(colors[i].r, colors[i].g, colors[i].b));
            }
            this.material = material;
            this.name = name;
        }

        public ColorTheme Deserialize()
        {
            var theme = new ColorTheme();

            Color[] colors = new Color[this.colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(
                    this.colors[i].xPos,
                    this.colors[i].yPos,
                    this.colors[i].zPos
                    );
            }


            theme.polygonMaterial = material;
            theme.SetColors(colors);
            theme.name = name;

            return theme;
        }
    }
}