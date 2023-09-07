using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    [SerializeField] private GameObject gfx;
    [SerializeField] private AnimatorOverrideController controller;

    private GameObject activeGameObject;
    protected GameObject Active { get => activeGameObject; }


    public AnimatorOverrideController AnimatorController { get => controller; }
    public abstract void Use(Transform usePoint, CharacterStats user);

    public abstract bool CanUse(CharacterStats user);

    public GameObject Instantiate(Transform transform)
    {
        activeGameObject = GameObject.Instantiate(gfx, transform);
        return activeGameObject;
    }

    public void Destroy()
    {
        if (activeGameObject != null)
            GameObject.Destroy(activeGameObject);
    }
}
