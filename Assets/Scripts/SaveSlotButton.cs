using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveSlotButton : MonoBehaviour
{
    [SerializeField] private int slotNumber;


    public void UpdateSaveInfo()
    {
        transform.GetChild(1).GetComponent<TMP_Text>().text = "Total Mynt: " + PlayerPrefs.GetInt("totalMynt" + slotNumber);
        transform.GetChild(2).GetComponent<TMP_Text>().text = "Highest Room: " + PlayerPrefs.GetInt("highestRoom" + slotNumber);
        transform.GetChild(3).GetComponent<TMP_Text>().text = "Enemies killed: " + PlayerPrefs.GetInt("enemiesKilled" + slotNumber);
    }

    private void OnEnable()
    {
        UpdateSaveInfo();
    }
}
