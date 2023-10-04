using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Scriptable Object List", menuName = "ScriptableObjects/Scriptable Object List")]
public class ScriptableObjectList : ScriptableObject
{
    public List<Item> items = new List<Item>();
    public List<Material> materials = new List<Material>();
    public List<Effect> effects = new List<Effect>();
}

