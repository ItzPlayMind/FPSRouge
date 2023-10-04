using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public System.Action<Item, Transform, CharacterStats> OnUse;

    [SerializeField] private GameObject gfx;
    [SerializeField] private AnimatorOverrideController controller;
    [SerializeField] protected List<Effect> offhandEffects = new List<Effect>();

    public List<Effect> OffHandEffects { get => offhandEffects; }

    private GameObject activeGameObject;
    protected GameObject Active { get => activeGameObject; set
        {
            activeGameObject = value;
        }
    }

    public AnimatorOverrideController AnimatorController { get => controller; }
    protected abstract void _Use(Transform usePoint, CharacterStats user);

    public void Use(Transform usePoint, CharacterStats user, bool withCallback = true)
    {
        _Use(usePoint, user);
        if(withCallback)
            OnUse?.Invoke(this, usePoint, user);
    }

    public HandPoint GetHandPoint(Hands.Hand hand)
    {
        string handPointName = "";
        switch (hand)
        {
            case Hands.Hand.Main:
                handPointName = "RightHandPoint";
                break;
            case Hands.Hand.Off:
                handPointName = "LeftHandPoint";
                break;
        }
        return activeGameObject?.transform.Find(handPointName)?.GetComponent<HandPoint>();
    }

    public void Passive(Transform usePoint, CharacterStats user) => offhandEffects.ForEach(x=>x.Passive(this, usePoint, user));
    public abstract bool CanUse(CharacterStats user);

    public virtual GameObject Instantiate(Transform transform, WeaponManager manager = null)
    {
        offhandEffects.ForEach(x => x.Setup());
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

    public virtual void SetupOnEquip(WeaponManager manager = null)
    {
        offhandEffects.ForEach(x => x.OnEquip(this, manager));
    }

    public void Destroy(WeaponManager manager = null)
    {
        if (manager != null)
            offhandEffects.ForEach(x => x.OnUnequip(this, manager));
        if (activeGameObject != null)
        {
            GameObject.Destroy(activeGameObject);
            activeGameObject = null;
        }
    }

    public virtual object GetMetadata() => null;
    public virtual void FromMetadata(object metaData) { }
}
