using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Indicators : MonoBehaviour {

    [SerializeField]
    MeshRenderer indicatorRight;
    [SerializeField]
    MeshRenderer indicatorLeft;

    [Range(0, 1)]
    public float maxIndicatorOpacity = 1;
    [Range(0, 1)]
    public float minIndicatorOpacity = 0;

	// Update is called once per frame
	void Update ()
    {
        var maxDist = Vector3.Distance(indicatorLeft.transform.localPosition, indicatorRight.transform.localPosition);
        var verticalDist = Mathf.Abs(indicatorRight.transform.position.y - indicatorLeft.transform.position.y);

        float bottomWeight = (verticalDist / 2 + maxDist / 2) / maxDist;
        float topWeight = Mathf.Abs(1 - bottomWeight);
        bottomWeight = ((maxIndicatorOpacity - minIndicatorOpacity) * bottomWeight) + minIndicatorOpacity;
        topWeight = ((maxIndicatorOpacity - minIndicatorOpacity) * topWeight) + minIndicatorOpacity;

        bool rightOnTop = indicatorRight.transform.position.y > indicatorLeft.transform.position.y;

        indicatorLeft.material.SetColor("_Color", new Color(1, 1, 1, rightOnTop ? bottomWeight : topWeight));
        indicatorRight.material.SetColor("_Color", new Color(1, 1, 1, rightOnTop ? topWeight : bottomWeight));
    }
}
