using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour {

	// Use this for initialization
	public void LoadScene (string scene){
        SceneManager.LoadScene(scene);
	}
}
