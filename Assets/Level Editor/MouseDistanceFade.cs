using UnityEngine;
using BitStrap;
using System.Collections;
using System;
using System.Collections.Generic;

public class MouseDistanceFade : LevelEntityAttribute {

    [SerializeField]
    AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1, 0, 0), new Keyframe(1, 0, 0, 0) });

    [SerializeField]
    float minDist = 0.5f;
    [SerializeField]
    float maxDist = 5f;

    [SerializeField, Range(0, 1)]
    float maxAlpha = 0.5f;
    [SerializeField, Range(0, 1)]
    float minAlpha = 0.1f;

    [SerializeField, Range(0, 1)]
    float hoverAlpha = 0.8f;
    [SerializeField, Range(0,1)]
    float selectedAlpha = 0.8f;

    [SerializeField]
    Renderer[] renderers;

    bool hover = false;
    bool selected = false;

    protected override void Awake()
    {
        base.Awake();
        if (renderers.Length == 0)
            renderers = new Renderer[] { GetComponent<Renderer>() }; 
    }

	// Update is called once per frame
	void LateUpdate ()
    {
        if(hover || selected)
        {
            SetAlpha(hover ? hoverAlpha : selectedAlpha);
            return;
        }

        float mouseDist = Vector3.Distance(LevelEditorUtils.ScreenTo2DWorldPlane(Input.mousePosition), transform.position);
        mouseDist = Mathf.Clamp(mouseDist, minDist, maxDist);
        float normalizedValue = (mouseDist - minDist) / (maxDist - minDist);
        normalizedValue = curve.Evaluate(normalizedValue);
        float alphaValue = (maxAlpha - minAlpha) * normalizedValue + minAlpha;

        SetAlpha(alphaValue);
    }

    protected override void OnPointerEnter() { hover = true; }
    protected override void OnPointerExit() { hover = false; }
    protected override void OnSelect() { selected = true; hover = false; }
    protected override void OnDeselect() { selected = false; hover = false; }

    void SetAlpha(float alpha)
    {
        foreach (Renderer mr in renderers)
        {
            mr.material.color = mr.material.color * new Color(1, 1, 1, 0) + new Color(0, 0, 0, alpha);
        }
    }
}
