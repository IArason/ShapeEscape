using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KeepUpright : MonoBehaviour {
    
	// Update is called once per frame
	void LateUpdate () {
        var rot = transform.eulerAngles;
        rot.z = 0;
        transform.eulerAngles = rot;
	}
}
