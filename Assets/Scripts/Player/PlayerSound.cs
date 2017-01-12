using UnityEngine;
using System.Collections;

public class PlayerSound : MonoBehaviour {


    [SerializeField]
    AudioClip[] transformSounds;
    [SerializeField]
    float[] transformVolumes;
    [SerializeField]
    AudioClip[] deathSounds;
    [SerializeField]
    float[] deathVolumes; 
    [SerializeField]
    AudioSource source;
    [SerializeField, Range(0, 1)]


    public string audioGroup;
    float timer = 0;


    // Use this for initialization
    void Start ()
    {
        PlaySound(audioGroup);
    }

	// Update is called once per frame
	void Update () {

        timer -= Time.deltaTime;
        if ( timer <= 0)
        {
            Destroy(this.gameObject);
        }

	}

    public void PlaySound(string type)
    {
        AudioClip[] soundsToUse;
        float[] volumesToUse;

        if (type == "death")
    {
            soundsToUse = deathSounds;
            volumesToUse = deathVolumes;
    }
        else if(type == "transform")
    {
            soundsToUse = transformSounds;
            volumesToUse = transformVolumes;
    }
        else
        {
            Debug.Log("error in sound call");
            Destroy(this.gameObject);
            return;
        }
        int clipID = Random.Range(0, soundsToUse.Length);
        source.clip = soundsToUse[clipID];
        source.volume = volumesToUse[clipID];
        source.Play();
        timer = source.clip.length;
    }
}
