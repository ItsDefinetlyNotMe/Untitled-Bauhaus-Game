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
      HitablePlayer.OnPlayerDeath += SetUp;
   }

   public void SetUp()
   {
      gameOverScreen.SetActive(true);
   }
}
