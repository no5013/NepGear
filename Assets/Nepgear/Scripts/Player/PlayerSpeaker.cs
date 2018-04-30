using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeaker : MonoBehaviour {

    private AudioSource audioSource;
    public const int ONCE = 1;
    public const int LOOP = 2;
    public const int DELAY = 3;
    public const int SCHEDULED = 4;



    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;

    }

    // Update is called once per frame
    void Update () {
		
	}

    public void Play(AudioClip clip, int playMode, float time = 0)
    {
        switch (playMode)
        {
            case ONCE:
                PlayOnce(clip);
                break;
            case LOOP:
                PlayLoop(clip);
                break;
            case DELAY:
                PlayDelay(clip, time);
                break;
            case SCHEDULED:
                PlayScheduled(clip, time);
                break;
        }
    }

    private void PlayOnce(AudioClip clip)
    {
        if (audioSource.isPlaying)
            Stop();
        audioSource.PlayOneShot(clip);
    }

    private void PlayLoop(AudioClip clip)
    {
        if (audioSource.isPlaying)
            Stop();
        audioSource.loop = true;
        audioSource.clip = clip;
        audioSource.Play();
    }
    private void PlayDelay(AudioClip clip, float time)
    {
        if (audioSource.isPlaying)
            Stop();
        audioSource.clip = clip;
        audioSource.PlayDelayed(time);
    }

    private void PlayScheduled(AudioClip clip, float time)
    {
        if (audioSource.isPlaying)
            Stop();
        audioSource.clip = clip;
        audioSource.PlayScheduled(time);
    }

    public void Stop()
    {
        audioSource.Stop();
        audioSource.loop = false;
    }

    public void Pause()
    {
        if (audioSource.isPlaying)
            audioSource.Pause();
    }
    public void Resume()
    {
        if (!audioSource.isPlaying)
            audioSource.UnPause();
    }


}
