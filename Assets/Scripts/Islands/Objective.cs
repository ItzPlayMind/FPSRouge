using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Objective : NetworkBehaviour
{
    public System.Action OnComplete;

    public NetworkVariable<bool> Finished { get; private set; } = new NetworkVariable<bool>(false);

    public void Complete()
    {
        Finished.Value = true;
        OnComplete?.Invoke();
    }

    public abstract void Setup();
}
