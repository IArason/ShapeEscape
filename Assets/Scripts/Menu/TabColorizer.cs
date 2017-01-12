using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TabColorizer : MonoBehaviour {
    public Color Tab1OutlineColor;
    public Color Tab1FillColor;
    public Color Tab2OutlineColor;
    public Color Tab2FillColor;
    public Color Tab3OutlineColor;
    public Color Tab3FillColor;
    //
    public Color Tab1ButtonFill;
    public Color Tab1ButtonOutline;
    public Color Tab2ButtonFill;
    public Color Tab2ButtonOutline;
    public Color Tab3ButtonFill;
    public Color Tab3ButtonOutline;

    public Image[] OutlinesButtons1;
    public Image[] FillsButtons1;
    public Image[] OutlinesButtons2;
    public Image[] FillsButtons2;
    public Image[] OutlinesButtons3;
    public Image[] FillsButtons3;
    //
    public Image[] OutlinesTab1;
    public Image[] FillsTab1;
    public Image[] OutlinesTab2;
    public Image[] FillsTab2;
    public Image[] OutlinesTab3;
    public Image[] FillsTab3;
    // Use this for initialization
    void Start () {
     
        SetColors();

    }
	

    
    void SetColors()
    {
        foreach (Image a in OutlinesTab1)
        {
            if(a != null)
                a.color = Tab1OutlineColor;
        }
        foreach (Image a in FillsTab1)
        {
            a.color = Tab1FillColor;
        }
        //
        foreach (Image a in OutlinesTab2)
        {
            if(a != null)
            a.color = Tab2OutlineColor;
        }
        foreach (Image a in FillsTab2)
        {
            a.color = Tab2FillColor;
        }
        //
        foreach (Image a in OutlinesTab3)
        {if(a != null)
            a.color = Tab3OutlineColor;
        }
        foreach (Image a in FillsTab3)
        {
            a.color = Tab3FillColor;
        }
        //
        //
        foreach (Image a in OutlinesButtons1)
        {
            a.color = Tab1ButtonOutline;
        }
        foreach (Image a in FillsButtons1)
        {
            a.color = Tab1ButtonFill;
        }
        foreach (Image a in OutlinesButtons2)
        {
            a.color = Tab2ButtonOutline;
        }
        foreach (Image a in FillsButtons2)
        {
            a.color = Tab2ButtonFill;
        }
        foreach (Image a in OutlinesButtons3)
        {
            a.color = Tab3ButtonOutline;
        }
        foreach (Image a in FillsButtons3)
        {
            a.color = Tab3ButtonFill;
        }
    }
}
