using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineDrawer : MonoBehaviour {
    
    static List<LineDrawerLine> lines = new List<LineDrawerLine>();

    public static void DrawLine(Vector3 from, Vector3 to, Color color, float width, bool antiAlias)
    {
        from = Camera.main.WorldToScreenPoint(from);
        to = Camera.main.WorldToScreenPoint(to);
        from.y = Mathf.Abs(from.y - Screen.height);
        to.y = Mathf.Abs(to.y - Screen.height);
        lines.Add(new LineDrawerLine(from, to, color, width, antiAlias));
    }

    void OnGUI()
    {
        if (Event.current.type != EventType.Repaint) return;
        foreach(LineDrawerLine line in lines)
        {
            ScreenDraw.DrawLine(line.from, line.to, line.color, line.width, line.antiAlias);
        }
        lines.Clear();
    }

    struct LineDrawerLine
    {
        public LineDrawerLine(Vector2 from, Vector2 to, Color color, float width, bool antiAlias)
        {
            this.from = from;
            this.to = to;
            this.color = color;
            this.width = width;
            this.antiAlias = antiAlias;
        }

        public Vector2 from;
        public Vector2 to;
        public Color color;
        public float width;
        public bool antiAlias;
    }

}
