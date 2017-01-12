using UnityEngine;
using System.Collections;

public class puff : MonoBehaviour {

    public GameObject puffPrefab;
    public Transform pos;

	public void Puff()
    {
        Instantiate(puffPrefab, pos.transform.position, Quaternion.identity);
    }
}
