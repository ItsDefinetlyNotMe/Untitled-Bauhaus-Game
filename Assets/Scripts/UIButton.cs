using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    private UpgradeWindow upgradeWindow;

    public void SelectThisButton()
    {
        GetComponent<Button>().Select();
    }

    public void IncreaseMaxHealth()
    {
        upgradeWindow.IncreaseMaxHealth();
    }

    public void IncreaseDamageMultiplier()
    {
        upgradeWindow.IncreaseDamageMultiplier();
    }

    private void Awake()
    {
        upgradeWindow = FindObjectOfType<UpgradeWindow>();
    }
}
