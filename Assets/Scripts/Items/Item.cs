using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] private AnimatorOverrideController controller;

    protected bool canUse = false;
    public bool CanUse { get => canUse; }

    public AnimatorOverrideController AnimatorController { get => controller; }
    public abstract bool Use(Transform usePoint, CharacterStats user);
}
