using UnityEngine;
using System.Collections;

public class destroypart : MonoBehaviour {
    float destroyTime;
    float emitTime;
    public bool alternative = false;
	// Use this for initialization
	void Start () {
        destroyTime = this.GetComponent<ParticleSystem>().startLifetime;
        emitTime = this.GetComponent<ParticleSystem>().duration;

        if (alternative)
        {
            ParticleSystem ps = GetComponent<ParticleSystem>();
            var em = ps.emission;
            em.enabled = false;
            new ParticleSystem.Burst(emitTime, 20);
            em.enabled = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (!alternative)
        {
            destroyTime -= Time.deltaTime;
            if (destroyTime <= 0)
            {
                Destroy(this.gameObject);
            }
        }
        if (alternative)
        {
            destroyTime -= Time.deltaTime;
            if ((emitTime + destroyTime)  <= 0)
            {
                ParticleSystem ps = GetComponent<ParticleSystem>();
                var em = ps.emission;
                em.enabled = false;
                Destroy(this.gameObject);
            }
        }
	}
}
