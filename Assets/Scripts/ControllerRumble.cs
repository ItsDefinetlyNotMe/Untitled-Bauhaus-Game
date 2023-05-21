using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum RumblePattern
{
    Constant,
    Pulse,
    Linear
}

public class ControllerRumble : MonoBehaviour
{
    public Gamepad gamepad;
    private PlayerInput _playerInput;

    private RumblePattern activeRumblePattern;
    private float rumbleDurration;
    private float pulseDurration;
    private float lowA;
    private float lowStep;
    private float highA;
    private float highStep;
    private float rumbleStep;
    private bool isMotorActive = false;


    public void RumbleConstant(float low, float high, float durration)
    {
        activeRumblePattern = RumblePattern.Constant;
        lowA = low;
        highA = high;
        rumbleDurration = Time.time + durration;
    }

    public void RumblePulse(float low, float high, float burstTime, float durration)
    {
        activeRumblePattern = RumblePattern.Pulse;
        lowA = low;
        highA = high;
        rumbleStep = burstTime;
        pulseDurration = Time.time + burstTime;
        rumbleDurration = Time.time + durration;
        isMotorActive = true;
        var g = gamepad;
        g?.SetMotorSpeeds(lowA, highA);
    }

    public void RumbleLinear(float lowStart, float lowEnd, float highStart, float highEnd, float durration)
    {
        activeRumblePattern = RumblePattern.Linear;
        lowA = lowStart;
        highA = highStart;
        lowStep = (lowEnd - lowStart) / durration;
        highStep = (highEnd - highStart) / durration;
        rumbleDurration = Time.time + durration;
    }

    public void StopRumble() { if (gamepad != null) { gamepad.SetMotorSpeeds(0, 0); } }

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        gamepad = Gamepad.all.FirstOrDefault(g => _playerInput.devices.Any(d => d.deviceId == g.deviceId));

        HittableObject.onObjectDeath += StartEnemyDeathRumble;
    }

    private void Update()
    {
        if (Time.time > rumbleDurration)
        {
            StopRumble();
            return;
        }

        if (gamepad == null) return;

        switch (activeRumblePattern)
        {
            case RumblePattern.Constant:
                gamepad.SetMotorSpeeds(lowA, highA);
                break;

            case RumblePattern.Pulse:

                if (Time.time > pulseDurration)
                {
                    isMotorActive = !isMotorActive;
                    pulseDurration = Time.time + rumbleStep;
                    if (!isMotorActive) { gamepad.SetMotorSpeeds(0, 0); }
                    else { gamepad.SetMotorSpeeds(lowA, highA); }
                }

                break;
            case RumblePattern.Linear:
                gamepad.SetMotorSpeeds(lowA, highA);
                lowA += (lowStep * Time.deltaTime);
                highA += (highStep * Time.deltaTime);
                break;
            default:
                break;
        }
    }

    private void StartEnemyDeathRumble()
    {
        RumblePulse(2, 2, 0.5f, 0.5f);
    }

    private void OnDestroy()
    {
        HittableObject.onObjectDeath -= StartEnemyDeathRumble;

        StopAllCoroutines();
        StopRumble();
    }
}
