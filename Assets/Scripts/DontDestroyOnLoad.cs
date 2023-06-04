using System.Collections;
using System.Collections.Generic;
using TestRandomWorldGeneration;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoad : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);

        SceneManager.sceneLoaded += OnSceneLoaded;

        // Find all objects with the same name in the scene
        GameObject[] objectsWithSameName = GameObject.FindGameObjectsWithTag(gameObject.tag);

        // If there is more than one object with the same name, delete this instance
        if (objectsWithSameName.Length > 1)
        {
            //Make sure that the wrong player instance doesn't do anything
            if (CompareTag("Player"))
            {
                HitablePlayer hitablePlayer = GetComponent<HitablePlayer>();
                hitablePlayer.isAlreadyDestroyed = true;
            }

            else if (CompareTag("MainCamera"))
            {
                CameraShake cameraShake = GetComponentInChildren<CameraShake>();
                cameraShake.isAlreadyDestroyed = true;
            }
            
            else if (CompareTag("GameManager"))
            {
                GameManager gameManager = GetComponent<GameManager>();
                gameManager.isAlreadyDestroyed = true;
            }
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            if (CompareTag("MainCamera")) //Here we have to put everything that can be in the Main Menu
                return;
                    //Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
