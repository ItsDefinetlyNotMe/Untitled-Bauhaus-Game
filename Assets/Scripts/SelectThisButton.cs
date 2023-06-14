using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectThisButton : MonoBehaviour
{
    private void OnEnable()
    {
        Slider slider = GetComponent<Slider>();

        if (slider != null)
            slider.Select();

        else
            GetComponent<Button>().Select();
    }    
}
