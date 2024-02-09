using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{

   private AudioSource audioSource;
  public  AudioClip[] clips;
    private AudioSource backGroundAudioSource;

    private void Start()
    {

        audioSource = GetComponent<AudioSource>();
        backGroundAudioSource = gameObject.AddComponent<AudioSource>();

     }

    public void EnemyDMGSound()
    {
        audioSource.clip = clips[1];
        audioSource.Play();
    }

    public void PlayerDMGSound()
    {
        audioSource.clip = clips[0]; 
        audioSource.Play();
    }

    public void PlayThrowSound()
    {
        audioSource.clip = clips[2];
        audioSource.Play();
    }

    public void LevelUp()
    {
        backGroundAudioSource.clip = clips[3];
        backGroundAudioSource.Play();      
    }

}

