using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    private UpgradeWindow upgradeWindow;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private PlayerAnimator playerAnimator;
    private bool sp;

    private int whileLoopTracker = 0;

    public bool isOnUpgrade { private get; set; } = false;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {

        }
        
        else if (scene.name == "HUB")
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
            playerAnimator = FindObjectOfType<PlayerAnimator>();
            playerAttack = FindObjectOfType<PlayerAttack>();

            upgradeWindow = FindObjectOfType<UpgradeWindow>();
        }

        else if (scene.name == "Valhalla")
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
            playerAnimator= FindObjectOfType<PlayerAnimator>();
            playerAttack = FindObjectOfType<PlayerAttack>();
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

        playerMovement.movementDirection = input.Get<Vector2>();
        playerAnimator.Movement(input);

        whileLoopTracker = 0;
    }

    private void OnDash(InputValue input)
    {
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

            upgradeWindow.Interact();

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
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSpecialAttack()
    {
        while (playerAttack == null)
        {
            if (whileLoopTracker > 10)
                return;

            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

            whileLoopTracker++;
        }
        whileLoopTracker = 0;

        if (!sp)
        {
            //chargeAttack
            sp = true;
            playerAttack.ChargeHeavyAttacK();
        }
        else
        {
            sp = false;
            playerAttack.HeavyAttacK();            
        }
        
    }
}
