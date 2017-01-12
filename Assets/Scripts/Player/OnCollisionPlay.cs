using UnityEngine;
using System.Collections;

public class OnCollisionPlay : MonoBehaviour {
    
    public AudioClip[] clips;

    public float minTimeBetweenSounds = 0.2f;
    public float volume = 0.3f;
    public float minImpact = 1f;
    float soundCooldown = 0;

    void Update()
    {
        if (soundCooldown > 0) soundCooldown -= Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (clips.Length > 0 && soundCooldown <= 0 && col.relativeVelocity.magnitude > minImpact)
        {
            var volume = this.volume * Mathf.Clamp(1 + col.relativeVelocity.magnitude - minImpact, 1, 3);
            AudioSource.PlayClipAtPoint(clips[Random.Range(0, clips.Length)], col.contacts[0].point, volume);
            soundCooldown = minTimeBetweenSounds;
        }
    }
}
