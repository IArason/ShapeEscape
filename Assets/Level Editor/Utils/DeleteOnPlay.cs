using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DeleteOnPlay : MonoBehaviour {

	void Start()
    {
        if(LevelSerializer.levelPlayable)
        {
            Destroy(gameObject);
        }
    }
}
