using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    private static PlayerUI instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(instance);
        instance = this;
    }

    public static PlayerUI Instance { get => instance; }

    [SerializeField] private UIBar healthBar;
    [SerializeField] private UIBar staminaBar;



    public void SetHealthBar(float value) => healthBar.SetBar(value);
    public void SetStaminaBar(float value) => staminaBar.SetBar(value);
}
