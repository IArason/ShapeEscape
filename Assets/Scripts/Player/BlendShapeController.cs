using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class BlendShapeController : MonoBehaviour {

    public Face selectedFace = Face.face1;
    public float speed = 1f;
    public FunctionOptions easingOption;

    public List<SkinnedMeshRenderer> meshRenderers = new List<SkinnedMeshRenderer>();

    private delegate float FunctionDelegate(float t);
    private static FunctionDelegate[] functionDelegates = {
        Easing.Linear,
        Easing.EaseInQuad,
        Easing.EaseOutQuad,
        Easing.EaseInCubic,
        Easing.EaseOutCubic,
        Easing.EaseInOutCubic,
        Easing.EaseInQuart,
        Easing.EaseOutQuart,
        Easing.EaseInOutQuart,
        Easing.EaseInQuint,
        Easing.EaseOutQuint,
        Easing.EaseInOutQuint
    };

    public enum FunctionOptions
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint
    }

    IEnumerator GotoFace(Face face, FunctionDelegate easingOption)
    {
        float time = 1 / speed;
        int numberOfBlendShapes = Enum.GetValues(typeof(Face)).Cast<int>().Last();
        for (float timer = 0; timer < time; timer += Time.deltaTime)
        {
            float lerpVal = easingOption(timer / time);

            foreach(var renderer in meshRenderers)
            {
                for(int i = 0; i < numberOfBlendShapes; i++)
                {
                    if((int)face == i)
                    {
                        renderer.SetBlendShapeWeight(i, lerpVal * 100);
                    }
                    else if(renderer.GetBlendShapeWeight(i) != 0)
                    {
                        renderer.SetBlendShapeWeight(i, Mathf.Abs((1 - lerpVal) * 100));
                    }
                }
            }

            yield return new WaitForEndOfFrame();
        }

        foreach (var renderer in meshRenderers)
        {
            for (int i = 0; i < numberOfBlendShapes; i++)
            {
                if ((int)face == i)
                {
                    renderer.SetBlendShapeWeight(i, 100);
                }
                else if (renderer.GetBlendShapeWeight(i) != 0)
                {
                    renderer.SetBlendShapeWeight(i, 0);
                }
            }
        }
    }

    [BitStrap.Button]
    void CycleFaces()
    {
        int newFace = (int)selectedFace + 1;
        if (Enum.GetValues(typeof(Face)).Cast<int>().Last() < newFace)
            newFace = 0;
        selectedFace = (Face)newFace;
        StopAllCoroutines();
        StartCoroutine(GotoFace(selectedFace, functionDelegates[(int)easingOption]));
    }

    public enum Face
    {
        face1,
        face2,
        face3,
        face4,
        face5,
        face6,
        face7,
        face8,
        face9,
        face10,
    }

    public class Easing
    {
        public static float Linear(float t) { return t; }

        public static float EaseInQuad(float t) { return t * t; }

        public static float EaseOutQuad(float t) { return t * (2 - t); }

        public static float EaseInOutQuad(float t) { return t < .5f ? 2 * t * t : -1 + (4 - 2 * t) * t; }

        public static float EaseInCubic(float t) { return t * t * t; }

        public static float EaseOutCubic(float t) { return (--t) * t * t + 1; }

        public static float EaseInOutCubic(float t) { return t < .5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1; }

        public static float EaseInQuart(float t) { return t * t * t * t; }

        public static float EaseOutQuart(float t) { return 1 - (--t) * t * t * t; }

        public static float EaseInOutQuart(float t) { return t < .5 ? 8 * t * t * t * t : 1 - 8 * (--t) * t * t * t; }

        public static float EaseInQuint(float t) { return t * t * t * t * t; }

        public static float EaseOutQuint(float t) { return 1 + (--t) * t * t * t * t; }

        public static float EaseInOutQuint(float t) { return t < .5 ? 16 * t * t * t * t * t : 1 + 16 * (--t) * t * t * t * t; }
    }
}
