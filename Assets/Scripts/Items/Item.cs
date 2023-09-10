using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string UID { get
        {
            return name.ToLower().Replace(" ", "_");
        }
    }

    [SerializeField] private GameObject gfx;
    [SerializeField] private AnimatorOverrideController controller;

    private GameObject activeGameObject;
    protected GameObject Active { get => activeGameObject; }


    public AnimatorOverrideController AnimatorController { get => controller; }
    public abstract void Use(Transform usePoint, CharacterStats user);

    public abstract bool CanUse(CharacterStats user);

    public GameObject Instantiate(Transform transform)
    {
        if (activeGameObject != null)
            Destroy(activeGameObject);
        activeGameObject = GameObject.Instantiate(gfx, transform);
        return activeGameObject;
    }

    public void Destroy()
    {
        if (activeGameObject != null)
            GameObject.Destroy(activeGameObject);
    }
}
