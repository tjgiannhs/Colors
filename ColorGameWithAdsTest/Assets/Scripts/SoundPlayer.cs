using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] AudioClip buttonClip;
    [SerializeField] AudioClip swipeClip;
    [SerializeField] AudioClip combineClip;
    [SerializeField] AudioClip deleteClip;
    [SerializeField] AudioClip highscoreClip;
    [SerializeField] AudioClip endGameClip;
    AudioSource myAudioSource;

    void Start()
    {
        myAudioSource = GetComponent<AudioSource>();
    }

    public void PlayButtonClip()
    {
        if(PlayerPrefs.GetInt("SoundsOn",1)>0.5f)
        {
            myAudioSource.PlayOneShot(buttonClip, 0.14f);
        }
    }
    public void PlaySwipeClip()
    {
        if(PlayerPrefs.GetInt("SoundsOn",1)>0.5f)
        {
            myAudioSource.PlayOneShot(swipeClip, 0.4f);
        }
    }
    public void PlayCombineClip()
    {
        if(PlayerPrefs.GetInt("SoundsOn",1)>0.5f)
        {
            myAudioSource.PlayOneShot(combineClip, 0.14f);
        }
    }
    public void PlayDeleteClip()
    {
        if(PlayerPrefs.GetInt("SoundsOn",1)>0.5f)
        {
            myAudioSource.PlayOneShot(deleteClip, 0.14f);
        }
    }
    public void PlayHighscoreClip()
    {
        if(PlayerPrefs.GetInt("SoundsOn",1)>0.5f)
        {
            myAudioSource.PlayOneShot(highscoreClip, 0.14f);
        }
    }
    public void PlayGameEndClip()
    {
        if(PlayerPrefs.GetInt("SoundsOn",1)>0.5f)
        {
            myAudioSource.PlayOneShot(endGameClip, 0.14f);
        }
    }
}
