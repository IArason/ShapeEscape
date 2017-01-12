using UnityEngine;
using System.Collections;

/// <summary>
/// Offsets an animator by changing its speed by +-25% for a few seconds,
/// then moves it to +- 5% indefinitely.
/// 
/// Requires the animator to have a float named Speed set as a multiplier
/// for each state which needs to be sped up.
/// </summary>
public class AnimatorOffsetter : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(Offset());
	}

    IEnumerator Offset()
    {
        var animator = GetComponent<Animator>();
        animator.speed = Random.Range(0.75f, 1.25f);
        yield return new WaitForSeconds(Random.Range(1, 5));
        animator.speed = Random.Range(0.95f, 1.05f);
    }
}
