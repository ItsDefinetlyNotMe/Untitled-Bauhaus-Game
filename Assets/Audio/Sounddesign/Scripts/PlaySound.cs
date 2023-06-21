using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource randomSound1;
    public AudioSource randomSound2;
    public AudioSource randomSound3;
    public AudioSource randomSound4;

    void playSound() 
    {
        randomSound1.Play();
    }

    void playSound2()
    {
        randomSound2.Play();
    }

    void playSound3()
    {
        randomSound3.Play();
    }

    void playSound4()
    {
        randomSound4.Play();
    }
}
