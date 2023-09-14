using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Objective : NetworkBehaviour
{
    public System.Action OnComplete;

    public bool Finished { get; private set; }

    public void Complete()
    {
        Finished = true;
        OnComplete?.Invoke();
    }

    public abstract void Setup();
}
