using UnityEngine;
using System.Collections;

public class WinZone : MonoBehaviour {
    
    bool activated = false;

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.transform.tag == "player" && !activated)
        {
            activated = true;
            FindObjectOfType<WinMenu>().Win();
        }
    }
}
