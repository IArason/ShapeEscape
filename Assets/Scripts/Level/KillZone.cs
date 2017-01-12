using UnityEngine;
using System.Collections;

public class KillZone : MonoBehaviour {
   

	// Update is called once per frame
	void OnCollisionEnter2D(Collision2D col)
    {
	    if(col.collider.tag == "player")
        {
            col.collider.transform.root.GetComponent<Player>().Die();
            return;
        }
        if(col.collider.tag == "prop")
        {
            col.gameObject.GetComponent<Prop>().Kill();
            return;
        }
	}
}
