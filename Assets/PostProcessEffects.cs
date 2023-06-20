using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessEffects : MonoBehaviour
{
    public Volume volume;
    Bloom bloom;
    Vignette vignette;
    float transitionDuration = 0.3f;
    float delayedTransitionDuration = 1f;

    void Start()
    {
        volume.profile.TryGet<Vignette>(out vignette);
        vignette.intensity.value = 0.2f;
    }

    public void CharacterHit()
    {
        StartCoroutine(ChangeVignetteIntensity(0.4f, transitionDuration));
        StartCoroutine(DelayedChangeVignetteIntensity(0.2f, delayedTransitionDuration));
    }

    IEnumerator ChangeVignetteIntensity(float targetIntensity, float duration)
    {
        float startIntensity = vignette.intensity.value;
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, t);
            yield return null;
        }

        vignette.intensity.value = targetIntensity;
    }

    IEnumerator DelayedChangeVignetteIntensity(float targetIntensity, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(ChangeVignetteIntensity(targetIntensity, delayedTransitionDuration));
    }
}
