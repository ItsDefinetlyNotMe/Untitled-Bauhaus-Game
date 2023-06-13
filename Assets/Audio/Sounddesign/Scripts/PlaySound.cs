using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource randomSound1;
    void playSound() 
    {
        randomSound1.Play();
    }
}
