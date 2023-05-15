using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
   [SerializeField] private GameObject gameOverScreen;
   private void Start()
   {
      HitablePlayer.onPlayerDeath += SetUp;
   }

   public void SetUp()
   {
      gameOverScreen.SetActive(true);
   }

   public void Respawn()
   {
      
   }

   public void Quit()
   {
      //save Progress
      Application.Quit();
   }
}
