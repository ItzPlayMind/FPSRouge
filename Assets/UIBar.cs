using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    [SerializeField] Image fill;

    float newValue;

    private void Start()
    {
        newValue = 1;
    }

    private void Update()
    {
        fill.fillAmount = Mathf.Lerp(fill.fillAmount, newValue, 0.1f);
    }

    public void SetBar(float amount)
    {
        newValue = amount;
    }
}
