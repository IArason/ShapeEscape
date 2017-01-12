using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShadowWiggleTest : MonoBehaviour {
    
    Shadow shadowEffect;
    Outline outlineEffect;
    Vector2 shadowStartDist;
    Vector2 startPos;
    Vector2 outlineStartDist; 

  
    float timer;

    WiggleTextControl control { get { return WiggleTextControl.Instance; } }

    float index;

    public bool positionWiggle;

    float randomSeed1;
    float randomSeed2;
    // Use this for initialization
    void Awake () {
        startPos = transform.localPosition;
        shadowEffect = GetComponent<Shadow>();
        shadowStartDist = shadowEffect.effectDistance;
        outlineEffect = GetComponent<Outline>();
        outlineStartDist = outlineEffect.effectDistance;

        randomSeed1 = Random.Range(0.8f, 1.2f);
        randomSeed2 = Random.Range(0.8f, 1.2f);
    }
	
	// Update is called once per frame
	void Update () {
        index += Time.deltaTime;
        timer -= Time.deltaTime;
        if (timer <= 0)
        {

            //DoWobble(shadowStartDist, 1);


            DoWobble(outlineStartDist, 2);
            

            timer = control.updateRate;
        }
        DoTransformWiggle(startPos);

         index += Time.deltaTime;
	}

    void DoWobble(Vector2 src, int toggle)
    {
        Vector2 tempDist = src;
        
        tempDist.x = (control.amplitudeX * Mathf.Cos((control.omegaX*Random.Range(0.8f, 1.2f)) * index))+tempDist.x;
        tempDist.y = (control.amplitudeY * Mathf.Sin((control.omegaY *Random.Range(0.8f, 1.2f)) * index))+tempDist.y;

        //Debug.Log(tempDist);
        if (toggle == 1)
        {
            shadowEffect.effectDistance = tempDist;
        }
        else
        {
            outlineEffect.effectDistance = tempDist;
        }
        

    }
    void DoTransformWiggle(Vector2 pos)
    {
        if (positionWiggle)
        {
            Vector2 tempPos = pos;
            tempPos.x = (control.amplitudeposX * Mathf.Cos((control.omegaposX * randomSeed1) * index)) + tempPos.x;
            tempPos.y = (control.amplitudeposY * Mathf.Sin((control.omegaposY * randomSeed2) * index)) + tempPos.y;
            transform.localPosition = tempPos;
        }
    }
}
