using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Material", menuName = "Materials/New Material")]
public class Material : ScriptableObject
{
    public string description;
    [SerializeField] private GameObject gfx;

    private GameObject activeGameObject;
    protected GameObject Active { get => activeGameObject; }

    public GameObject Instantiate(Transform transform)
    {
        if (activeGameObject != null)
            Destroy(activeGameObject);
        activeGameObject = GameObject.Instantiate(gfx, transform);
        return activeGameObject;
    }
}
