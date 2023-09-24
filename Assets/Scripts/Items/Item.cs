using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    [SerializeField] private GameObject gfx;
    [SerializeField] private AnimatorOverrideController controller;
    [SerializeField] private Effect offhandEffect;


    private GameObject activeGameObject;
    protected GameObject Active { get => activeGameObject; }

    public AnimatorOverrideController AnimatorController { get => controller; }
    public abstract void Use(Transform usePoint, CharacterStats user);
    public void Passive(Transform usePoint, CharacterStats user) => offhandEffect?.Passive(this, usePoint, user);
    public abstract bool CanUse(CharacterStats user);

    public GameObject Instantiate(Transform transform, WeaponManager manager = null)
    {
        offhandEffect?.Setup();
        if (activeGameObject != null)
        {
            Destroy(activeGameObject);
            activeGameObject = null;
        }
        activeGameObject = GameObject.Instantiate(gfx, transform);
        OnInstantiate(transform, manager);
        if (manager != null)
            SetupOnEquip(manager);
        return activeGameObject;
    }

    protected virtual void OnInstantiate(Transform transform, WeaponManager manager) { }

    public void SetupOnEquip(WeaponManager manager = null)
    {
        offhandEffect?.OnEquip(this, manager);
    }

    public void Destroy(WeaponManager manager = null)
    {
        if (manager != null)
            offhandEffect?.OnUnequip(this, manager);
        if (activeGameObject != null)
        {
            GameObject.Destroy(activeGameObject);
            activeGameObject = null;
        }
    }
}
