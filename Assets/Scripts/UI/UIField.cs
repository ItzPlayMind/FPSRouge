using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIField : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI text;
    [SerializeField] private TMPro.TextMeshProUGUI value;

    public string Text { get => text.text;
        set
        {
            text.text = value;
        }
    }

    public string Value
    {
        get => value.text;
        set
        {
            this.value.text = value;
        }
    }
}
