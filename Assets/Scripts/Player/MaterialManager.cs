using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MaterialManager : NetworkBehaviour
{
    private Storage<Material> materials = new Storage<Material>();

    private void Start()
    {
        if (!IsOwner)
            return;
        materials.OnAdd += (Material mat) =>
        {
            Debug.Log("Added " + mat.name);
        };
    }

    public void AddMaterial(Material mat) => materials.Add(mat);
}
