using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSound : MonoBehaviour
{
    public AudioClip[] audioSources1;
    public AudioSource randomSound1;
    public AudioClip[] audioSources2;
    public AudioSource randomSound2;

    public void PlayRandom1()
    {
        randomSound1.clip = audioSources1[Random.Range(0, audioSources1.Length)];
        randomSound1.Play();
        //print("PersonSound");
    }

    public void PlayRandom2()
    {
        randomSound2.clip = audioSources2[Random.Range(0, audioSources2.Length)];
        randomSound2.Play();
        print("PersonSound2");
    }
}
