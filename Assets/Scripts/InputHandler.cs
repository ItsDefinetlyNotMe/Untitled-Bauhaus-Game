using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    private UpgradeWindow upgradeWindow;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private PlayerAnimator playerAnimator;

    private int whileLoopTracker = 0;

    public bool isOnUpgrade { private get; set; } = false;
    public bool isInPauseMenu { private get; set; } = false;

    private HitablePlayer hitablePlayer;
    private PlayerInput playerInput;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerInput = gameObject.GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (playerInput.currentControlScheme == "Keyboard + Mouse")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (playerInput.currentControlScheme == "Gamepad")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            StartCoroutine(ActivateUI());
        }

        else if (scene.name == "HUB")
        {
            StartCoroutine(DeactivateUI());

            playerMovement = FindObjectOfType<PlayerMovement>();
            playerAnimator = FindObjectOfType<PlayerAnimator>();
            playerAttack = FindObjectOfType<PlayerAttack>();
            hitablePlayer = FindObjectOfType<HitablePlayer>();


            upgradeWindow = FindObjectOfType<UpgradeWindow>();
        }

        else if (scene.name == "Valhalla")
        {
            StartCoroutine(DeactivateUI());

            playerMovement = FindObjectOfType<PlayerMovement>();
            playerAnimator = FindObjectOfType<PlayerAnimator>();
            playerAttack = FindObjectOfType<PlayerAttack>();
            hitablePlayer = FindObjectOfType<HitablePlayer>();

        }
    }
    
    private void OnMovement(InputValue input)
    {
        while (playerMovement == null || playerAnimator == null)
        {
            if (whileLoopTracker > 10)
                return;

            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

            whileLoopTracker++;
        }

        if (playerMovement.currentState != Structs.PlayerState.Dashing)
        {
            playerMovement.movementDirection = input.Get<Vector2>();
            playerAnimator.Movement(input);
        }

        whileLoopTracker = 0;
    }

    private void OnDash(InputValue input)
    {
        if(hitablePlayer.isDying)
            return;
        {
            if (whileLoopTracker > 10)
                return;

            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

            whileLoopTracker++;
        }

        playerMovement.Dash(input);

        whileLoopTracker = 0;
    }

    private void OnAttack(InputValue input)
    {
        if(hitablePlayer.isDying)
            return;

        while (playerAttack == null)
        {
            if (whileLoopTracker > 10)
                return;

            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

            whileLoopTracker++;
        }

        playerAttack.Attack(input);

        whileLoopTracker = 0;
    }

    private void OnStart(InputValue input)
    {
        if (isOnUpgrade)
        {
            while (upgradeWindow == null)
            {
                if (whileLoopTracker > 10)
                    return;

                OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

                whileLoopTracker++;
            }

            if (GameObject.Find("/Upgrader").transform.GetChild(0).gameObject.activeSelf == false)
            {
                upgradeWindow.Interact();
            }
                

            whileLoopTracker = 0;
        }
    }

    private void OnCancel(InputValue input)
    {
        if (isOnUpgrade)
        {
            while (upgradeWindow == null)
            {
                if (whileLoopTracker > 10)
                    return;

                OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

                whileLoopTracker++;
            }

            upgradeWindow.Back();

            whileLoopTracker = 0;
        }

        else if (isInPauseMenu)
        {
            StartCoroutine(DeactivateUI());

            isInPauseMenu = false;

            GameObject pauseCanvas = GameObject.Find("PauseMenuCanvas");
            pauseCanvas.transform.GetChild(1).gameObject.SetActive(false);
            pauseCanvas.transform.GetChild(0).gameObject.SetActive(false);
            Time.timeScale = 1f;
        }

        else if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (GameObject.Find("/Canvas/MainMenu/SlotDeletionConfirmation").activeSelf)
            {
                GameObject.Find("/Canvas/MainMenu/Slots").SetActive(true);
                GameObject.Find("/Canvas/MainMenu/SlotDeletionConfirmation").SetActive(false);
            }
            else
            {
                GameObject.Find("/Canvas/MainMenu/StartButtons").SetActive(true);
                GameObject.Find("/Canvas/MainMenu/Slots").SetActive(false);
                GameObject.Find("/Canvas/MainMenu/Settings").SetActive(false);
                GameObject.Find("/Canvas/MainMenu/SlotDeletionConfirmation").SetActive(false);

            }
        }

    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSpecialAttack(InputValue input)
    {
        while (playerAttack == null)
        {
            if (whileLoopTracker > 10)
                return;

            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

            whileLoopTracker++;
        }
        whileLoopTracker = 0;

        
        if (input.isPressed)
        {
            //chargeAttack
            if(hitablePlayer.isDying)
                return;
            playerAttack.ChargeHeavyAttack();
        }
        else
        {
            //Attack
            if(hitablePlayer.isDying)
                return;
            playerAttack.HeavyAttack();
        }

    }

    private void OnPause()
    {
        if (isOnUpgrade)
            return;
        
        CameraShake.Instance.StopShaking();
        
        StartCoroutine(ActivateUI());

        isInPauseMenu = true;

        GameObject pauseCanvas = GameObject.Find("PauseMenuCanvas").transform.GetChild(0).gameObject;
        pauseCanvas.SetActive(true);
        GameObject controls_controller = GameObject.Find("Controls").transform.GetChild(0).gameObject;
        GameObject controls_mouse = GameObject.Find("Controls").transform.GetChild(1).gameObject;
        if (playerInput.currentControlScheme == "Keyboard + Mouse")
        {
            controls_controller.SetActive(false);
            controls_mouse.SetActive(true);
        }
        if (playerInput.currentControlScheme == "Gamepad")
        {
            controls_controller.SetActive(true);
            controls_mouse.SetActive(false);
        }
        Time.timeScale = 0f;

    }

    private void OnDelete()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
            GameObject.Find("/Canvas/MainMenu").GetComponent<MainMenu>().SlotDeletionConfirmatiion();
    }

    private IEnumerator ActivateUI()
    {
        yield return new WaitForEndOfFrame();

        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        playerInput.actions.FindActionMap("UI").Enable();
        playerInput.actions.FindActionMap("Fighting").Disable();
        playerInput.actions.FindActionMap("Movement").Disable();
    }

    private IEnumerator DeactivateUI()
    {
        yield return new WaitForEndOfFrame();

        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        playerInput.actions.FindActionMap("Fighting").Enable();
        playerInput.actions.FindActionMap("Movement").Enable();
        playerInput.actions.FindActionMap("UI").Disable();
    }
}