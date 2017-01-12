using UnityEngine;
using System.Collections;
//using UnityEditor;

using UnityEngine.SceneManagement;

public class MusicScript : MonoBehaviour {

    public static bool muted = false;

    [Header("SceneType")]
    bool editorScene = false;
    public AudioSource musicSource;
    
    [Header("Files")]
    public AudioClip[] playMusic;
    public AudioClip[] editMusic;
    [Header("Volume hack")]
    public float[] playMusicVol;
    public float[] editMusicVol;


    int currentTrack;
    float currentTrackLength;
    float timer;
    AudioClip[] currentTracks;
    float[] currentVolumes;
    [Header("Master Volume - add to Options")]
    [Range(0.0f, 1.0f)]
    public float masterVolume;
    public bool playInEditor = true;
    public bool playInPlay = true;
    float pauseInbetween = 5.0f;
    bool Allowed = true;

    string currentSceneName;
    // Use this for initialization

    public void ToggleMute()
    {
        muted = !muted;
        foreach(var v in FindObjectsOfType<MusicScript>())
        {
            if (muted)
                v.Stop();
            else
                v.Start();
        }
    }


    void Start () {

        if (muted) return;

        //PrefabUtility.RevertPrefabInstance(this.gameObject);
        currentSceneName = SceneManager.GetActiveScene().name;
    if (!musicSource)
        {
            Debug.Log("no music source found!");
        }
        musicSource.Stop();

        if (currentSceneName == "EmptyPlayPublishPlaytest" || currentSceneName == "EmptyPlayShared" || currentSceneName == "EmptyPlay" || currentSceneName == "MainMenu")
        {
            editorScene = false;
           // Debug.Log("playmodedetect");
            SwitchTracks(false);

        
        }
        else
        {
           // Debug.Log("editormodedetect");
            editorScene = true;
            SwitchTracks(true);
        }

        
    }

    // Update is called once per frame
    void Update () {
       if (Allowed && !muted) { 
            timer -= Time.deltaTime;

            if (timer <= (pauseInbetween * -1))  //dat fucking hack lmao
            {
                /* if (!paused)
                 {
                     paused = true;
                     timer = pauseInbetween;
                 }
                 else
                 {*/
                PlayNext();
            }
        }


    }
    public void SwitchTracks(bool trueIfEdit)
    {
        //hacky
        if (trueIfEdit)
        {
            editorScene = true;
        }
        else
        {
            editorScene = false;
        }
        //
        if (editorScene) {
            currentTracks = editMusic;
            currentVolumes = editMusicVol;
            if (playInEditor)
            {
                Allowed = true;
                musicSource.Pause();
            }
            else
            {
                Allowed = false;
            }
        }
        else
        {
            currentTracks = playMusic;
            currentVolumes = playMusicVol;
            if (playInPlay)
            {
                Allowed = true;
                musicSource.Pause();
            }
            else
            {
                Allowed = false;
            }
        }
        currentTrack = Random.Range(0, currentTracks.Length);
        if (currentSceneName == "MainMenu" && !editorScene && currentTrack == 1)
        {
            currentTrack = Random.Range(0, currentTracks.Length);
        }
        if (Allowed)
        {
            PlayAndUpdate(currentTrack);
        }
    }

    void PlayAndUpdate(int ID)    //handles everything required after changing the track ID
    {
        musicSource.clip = currentTracks[ID];  //sets current clip to play
        currentTrackLength = currentTracks[ID].length;  //gets length of the clip

        musicSource.Play();
        musicSource.volume = currentVolumes[ID]*masterVolume;
        timer = currentTrackLength;
       
    }
    void PlayNext()
    {
        if (currentTrack < currentTracks.Length - 1)
        {
            currentTrack++;
            PlayAndUpdate(currentTrack);
        }
        else
        {
            currentTrack = 0;
            PlayAndUpdate(currentTrack);
        }

    }

    void Stop()
    {
        musicSource.Stop();
    }

}
