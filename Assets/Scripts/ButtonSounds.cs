using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public void hoverSoundFunction()
    {
        audioSource.PlayOneShot(hoverSound);
    }

    public void clickSoundFunction()
    {
        audioSource.PlayOneShot(clickSound);
    }
}
