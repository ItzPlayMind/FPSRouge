using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] private AnimatorOverrideController controller;

    public AnimatorOverrideController AnimatorController { get => controller; }
    public abstract void Use(Transform user);
}
