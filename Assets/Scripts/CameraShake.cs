using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }
    private float timeToShake;
    private bool isShaking = false;
    private float intensity_;
    private float shakeTime_;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    private CinemachineVirtualCamera cinemaschineVirtualCamera;
    void Awake()
    {
        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("HIER");
        Instance = this;
        cinemaschineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannelPerlin = cinemaschineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    public void ShakeCamera(float shakeTime, float intensity,bool isLerping = false)
    {
        intensity_ = intensity;
        shakeTime_ = shakeTime;
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        
        timeToShake = Time.time + shakeTime;
        if(isLerping)
            InvokeRepeating(nameof(Shake2),0.1f,0.1f);
        else
            InvokeRepeating(nameof(Shake1),0.1f,0.1f);
            // CinemaschineBasicMultiChannelPerlin cinemaschineBasicMultiChannelPerlin
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

}
