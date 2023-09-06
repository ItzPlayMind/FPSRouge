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

    [SerializeField] private TMPro.TextMeshProUGUI debugHealthText;



    public void SetDebugHealthText(float value)
    {
        debugHealthText.text = "Health: " + value;
    }
}
