using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableExplanation : MonoBehaviour
{
    public void OnEnable()
    {
        Invoke(nameof(SetCollectableInvisible), 2.0f);
    }

    private void SetCollectableInvisible()
    {
        gameObject.SetActive(false);
    }
}
