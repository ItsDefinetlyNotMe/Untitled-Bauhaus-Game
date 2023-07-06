using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }
    private float timeToShake;
    private int shakeCount;
    private Quaternion baseRotation;
    // private bool isShaking = false;
    private float intensity_;
    private float shakeTime_;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    public bool isAlreadyDestroyed { private get; set; } = false;

    private CinemachineVirtualCamera cinemaschineVirtualCamera;
    void Awake()
    {
        //Instance = this;
        baseRotation = Quaternion.identity;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isAlreadyDestroyed)
            return;

        Instance = this;
        cinemaschineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannelPerlin = cinemaschineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    public void ShakeCamera(float shakeTime, float intensity,bool isLerping = false)
    {
        shakeCount += 1;
        intensity_ = intensity;
        shakeTime_ = shakeTime;
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        
        timeToShake = Time.time + shakeTime;
        if(isLerping)
            InvokeRepeating(nameof(Shake2),0.1f,0.1f);
        else
            InvokeRepeating(nameof(Shake1),0.1f,0.1f);
        shakeCount -= 1;
        if (shakeCount == 0)
        {
            //StartCoroutine(ResetCameraRotation(0.1f));
            //Invoke("ResetRotation", 0.1f);
        }
        // CinemaschineBasicMultiChannelPerlin cinemaschineBasicMultiChannelPerlin

    }

    public void StopShaking()
    {
        //CancelInvoke(nameof(Shake2));
        //CancelInvoke(nameof(Shake1));
        
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
        ResetCameraRotation(0f);
    }

    private void Shake1()
    {
        if (Time.time > timeToShake)
        {
            var cinemachineBasicMultiChannelPerlin = cinemaschineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            CancelInvoke(nameof(Shake1));
        }
    }

    private void Shake2()
    {
        var cinemachineBasicMultiChannelPerlin = cinemaschineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        float lerpValue = intensity_ * Mathf.Max((timeToShake - Time.time) / shakeTime_, 0f); 
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = lerpValue;
        if(lerpValue == 0)
            CancelInvoke(nameof(Shake2));
    }
    
    private IEnumerator ResetCameraRotation(float duration)
    {
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            transform.rotation = Quaternion.Slerp(startRotation, baseRotation, t);
            yield return null;
        }

        transform.rotation = baseRotation;
    }
    public void ResetRotation()
    {
        transform.rotation = baseRotation;
    }

}
