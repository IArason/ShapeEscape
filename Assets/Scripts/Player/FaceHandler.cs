using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FaceHandler : MonoBehaviour {

    public int totalBlendShapes = 9;

    public float speed = 2f;

    public SkinnedMeshRenderer[] rectFaces;
    public SkinnedMeshRenderer[] cubeFaces;
    public SkinnedMeshRenderer[] sphereFaces;

    Faces cubeFaceIndex = Faces.sad;
    Faces sphereFaceIndex = Faces.resigned;
    Faces rectFaceIndex = Faces.giggle;

    float c_airTimer = 0;
    float c_groundTimer = 0;

    void Awake()
    {
        StartCoroutine(CubeFaces());
        StartCoroutine(SphereFaces());
        StartCoroutine(RectFaces());
    }

    void OnCollisionStay2D(Collision2D col)
    {
        c_airTimer = 0;
        cubeFaceIndex = Faces.sad;
        c_groundTimer += Time.deltaTime;
    }

    IEnumerator CubeFaces()
    {
        // collisionstay > 0.5s - relieved 

        while (true)
        {
            foreach (var v in cubeFaces)
            {
                for (int i = 0; i < totalBlendShapes; i++)
                {
                    v.SetBlendShapeWeight(
                        i,
                        Mathf.Clamp(v.GetBlendShapeWeight(i) + (i == (int)cubeFaceIndex ? 1 : -1)
                        * Time.deltaTime * speed * 100
                        , 0, 100));
                }
            }
            c_airTimer += Time.deltaTime;

            if (c_airTimer > 0.3f) c_groundTimer = 0;

            yield return new WaitForEndOfFrame();

            if (c_airTimer > 1f) cubeFaceIndex = Faces.scared;

            //if (c_groundTimer > 1f) cubeFaceIndex = Faces.nervous;

        }
        // in air - screaming 

        // low motion - content 
    }

    IEnumerator SphereFaces()
    {
        // Reduced speed - sad
        while (true)
        {
            foreach (var v in sphereFaces)
            {
                for (int i = 0; i < totalBlendShapes; i++)
                {
                    v.SetBlendShapeWeight(
                        i,
                        Mathf.Clamp(v.GetBlendShapeWeight(i) + (i == (int)sphereFaceIndex ? 1 : -1)
                        * Time.deltaTime * speed * 100
                        , 0, 100));
                }
            }
            yield return new WaitForEndOfFrame();
            
            if (GetComponent<Rigidbody2D>().velocity.magnitude > 5f)
                sphereFaceIndex = Faces.happy;
            else if (Mathf.Abs(GetComponent<Rigidbody2D>().angularVelocity) > 50f)
                sphereFaceIndex = Faces.smug;
            else
                sphereFaceIndex = Faces.sad;
        }
    }
    

    IEnumerator RectFaces()
    {
        while (true)
        {
            foreach(var v in rectFaces)
            {
                for (int i = 0; i < totalBlendShapes; i++)
                {
                    v.SetBlendShapeWeight(
                        i,
                        Mathf.Clamp(v.GetBlendShapeWeight(i) + (i == (int)rectFaceIndex ? 1 : -1)
                        * Time.deltaTime * speed * 100
                        , 0, 100));
                }
            }
            yield return new WaitForEndOfFrame();


            if (GetComponent<Rigidbody2D>().velocity.magnitude > 3f)
                sphereFaceIndex = Faces.laugh;
            else
                sphereFaceIndex = Faces.giggle;
        }
    }

    public void Hurt()
    {

        cubeFaceIndex = Faces.hurt;
        sphereFaceIndex = Faces.hurt;
        rectFaceIndex = Faces.hurt;
        StopAllCoroutines();

        foreach (var v in rectFaces)
        {
            for (int i = 0; i < totalBlendShapes; i++)
            {
                v.SetBlendShapeWeight(
                    i,
                    Mathf.Clamp(v.GetBlendShapeWeight(i) + (i == (int)rectFaceIndex ? 1 : -1)
                    * Time.deltaTime * speed * 100
                    , 0, 100));
            }
        }
        foreach (var v in sphereFaces)
        {
            for (int i = 0; i < totalBlendShapes; i++)
            {
                v.SetBlendShapeWeight(
                    i,
                    Mathf.Clamp(v.GetBlendShapeWeight(i) + (i == (int)rectFaceIndex ? 1 : -1)
                    * Time.deltaTime * speed * 100
                    , 0, 100));
            }
        }
        foreach (var v in cubeFaces)
        {
            for (int i = 0; i < totalBlendShapes; i++)
            {
                v.SetBlendShapeWeight(
                    i,
                    Mathf.Clamp(v.GetBlendShapeWeight(i) + (i == (int)rectFaceIndex ? 1 : -1)
                    * Time.deltaTime * speed * 100
                    , 0, 100));
            }
        }

        Invoke("Awake", 0.5f);
    }

    enum Faces
    {
        skeptical = 0,
        glad = 1,
        resigned = 2,
        nervous = 3,
        happy = 4,
        smug = 5,
        scared = 6,
        sad = 7,
        hurt = 8,
        giggle = 9,
        laugh = 10
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
