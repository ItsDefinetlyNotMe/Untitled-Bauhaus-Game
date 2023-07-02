using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PostProcessEffects : MonoBehaviour
{
    public Volume volume;
    Bloom bloom;
    Vignette vignette;
    ChromaticAberration chromaticAberration;
    float transitionDuration = 0.3f;
    float delayedTransitionDuration = 1f;
    float maxChromaticAberrationIncrease = 0.1f;
    float currentTargetChromaticAberrationIntensity = 0.1f;

    void Start()
    {
        volume.profile.TryGet<Vignette>(out vignette);
        vignette.intensity.value = 0.2f;
        volume.profile.TryGet<ChromaticAberration>(out chromaticAberration);
        chromaticAberration.intensity.value = 0f;
    }

    public void CharacterHit()
    {
        StopAllCoroutines(); // Stop all running coroutines before starting new ones

        currentTargetChromaticAberrationIntensity = Mathf.Clamp(currentTargetChromaticAberrationIntensity + maxChromaticAberrationIncrease, 0.1f, 0.5f);
        StartCoroutine(ChangeVignetteIntensity(0.4f, transitionDuration));
        StartCoroutine(ChangeChromaticAberrationIntensity(currentTargetChromaticAberrationIntensity, transitionDuration));
        StartCoroutine(DelayedChangeVignetteIntensity(0.2f, delayedTransitionDuration));
        StartCoroutine(DelayedChangeChromaticAberrationIntensity(0.1f, delayedTransitionDuration));
    }

    public void CharacterDeath()
    {
        StopAllCoroutines(); // Stop all running coroutines before starting new ones

        StartCoroutine(ChangeVignetteIntensity(0.6f, transitionDuration));
        StartCoroutine(ChangeChromaticAberrationIntensity(0.4f, transitionDuration));
    }

    public void LoadNewScene(string sceneName)
    {
        StopAllCoroutines(); // Stop all running coroutines before loading a new scene
        SceneManager.LoadScene(sceneName);
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

        yield return null;
    }

    IEnumerator ChangeChromaticAberrationIntensity(float targetIntensity, float duration)
    {
        float startIntensity = chromaticAberration.intensity.value;
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            chromaticAberration.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, t);
            yield return null;
        }

        chromaticAberration.intensity.value = targetIntensity;

        yield return null; 
    }

    IEnumerator DelayedChangeVignetteIntensity(float targetIntensity, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(ChangeVignetteIntensity(targetIntensity, delayedTransitionDuration));
    }

    IEnumerator DelayedChangeChromaticAberrationIntensity(float targetIntensity, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(ChangeChromaticAberrationIntensity(targetIntensity, delayedTransitionDuration));
    }
}
