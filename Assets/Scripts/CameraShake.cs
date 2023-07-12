using System;
using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }
    private float timeToShakeLerp;
    private float timeToShakeNoLerp;
    
    private Quaternion baseRotation;

    private int shakeCount;
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

    private void Update()
    {
        if (shakeCount <= 0)
        {
            //if (transform.parent.rotation != Quaternion.identity)
            //{
                StopShaking();
            //}
        }
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
        intensity_ = intensity;
        shakeTime_ = shakeTime;
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        
            /*if (timeToShakeLerp > Time.time)
                shakeCount -= 1;*/
        if (shakeCount == 1)
        {
            if (timeToShakeLerp > Time.time)
                CancelInvoke(nameof(ShakeWithLerp));
            else if (timeToShakeNoLerp > Time.time)
                CancelInvoke(nameof(ShakeWithLerp));
        }
        shakeCount = 1;

        if (isLerping)
        {
            timeToShakeLerp = Time.time + shakeTime;
        }
        else if (!isLerping)
        {
            timeToShakeNoLerp = Time.time + shakeTime;
        }
        if (isLerping)
            InvokeRepeating(nameof(ShakeWithLerp), 0.1f, 0.15f);
        else
            InvokeRepeating(nameof(ShakeNoLerp), 0.1f, 0.15f);
    }

    public void StopShaking()
    {
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
        transform.parent.rotation = Quaternion.identity;
    }

    private void ShakeNoLerp()
    {
        if (Time.time > timeToShakeNoLerp)
        {
            var cinemachineBasicMultiChannelPerlin = cinemaschineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            //StopShaking();
            shakeCount -= 1;
            CancelInvoke(nameof(ShakeNoLerp));
        }
    }

    private void ShakeWithLerp()
    {
        var cinemachineBasicMultiChannelPerlin =
            cinemaschineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        float lerpValue = intensity_ * Mathf.Max((timeToShakeLerp - Time.time) / shakeTime_, 0f);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = lerpValue;
        //StopShaking();
        if (lerpValue == 0)
        {
            shakeCount -= 1;
            CancelInvoke(nameof(ShakeWithLerp));
        }

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
}
