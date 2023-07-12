using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    private float targetDirectionTimeStamp;
    private float haltTime;
    private float backDirectionTimeStamp;
    
    private float targetDirectionTime;
    private float backDirectionTime;
    private Vector3 startBackPosition;
    private Vector3 endBackPosition;
    private float startTime;

    private bool animationStarted;
    private float tTarget;

    private Animator thorAnimator;
    private PlayerMovement playerMovement;
    private float tBack;
    // Start is called before the first frame update
    void Start()
    {
        thorAnimator = GameObject.FindWithTag("Thor").GetComponent<Animator>();
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        playerMovement.canMove = false;
        startBackPosition = new Vector3(-4.3f,0f,-5.6f);
        endBackPosition = new Vector3(-4.5f,-4.15f,-10f);
        transform.position = endBackPosition;
        cinemachineVirtualCamera.Priority = 10;
        haltTime = 2f;
        targetDirectionTime = 1f;
        backDirectionTime = 2f;
        targetDirectionTimeStamp = Time.time + targetDirectionTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time < targetDirectionTimeStamp)
        {
            tTarget += Time.deltaTime / targetDirectionTime;
            transform.position = Vector3.Lerp(endBackPosition, startBackPosition, tTarget);
        }
        else if (!animationStarted)
        {
            animationStarted = true;
            thorAnimator.Play("ThorDestroysChair");
        }

        if (Time.time > targetDirectionTimeStamp + haltTime)
        {
            tBack += Time.deltaTime / backDirectionTime;
            transform.position = Vector3.Lerp(startBackPosition, endBackPosition, tBack);
            if (tBack >= 1)
            {
                cinemachineVirtualCamera.Priority = 0;
                playerMovement.canMove = true;
                Destroy(this);
            }

        }
    }
}
