using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectManager : MonoBehaviour
{
    private static ScriptableObjectManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    public static ScriptableObjectManager Instance { get => instance; }

    [SerializeField] private ScriptableObjectList scriptableObjectList;

    public T Get<T>(string uid, string metaData = "") where T : ScriptableObject
    {
        var type = typeof(T);
        if (type == typeof(Material))
            return (T)(ScriptableObject)scriptableObjectList.materials.Find(x => x.UID() == uid).Clone();
        if (type == typeof(Item))
        {
            var item = scriptableObjectList.items.Find(x => x.UID() == uid).Clone();
            if(item is Weapon)
                item.FromMetadata(JsonUtility.FromJson<Weapon.Metadata>(metaData));
            return (T)(ScriptableObject)item.Clone();
        }
        if (type == typeof(Effect))
            return (T)(ScriptableObject)scriptableObjectList.effects.Find(x => x.UID() == uid).Clone();
        return null;
    }

    public T GetRandom<T>() where T : ScriptableObject
    {
        var type = typeof(T);
        if (type == typeof(Item))
            return (T)(ScriptableObject)scriptableObjectList.items[Random.Range(0, scriptableObjectList.items.Count)];
        if (type == typeof(Material))
            return (T)(ScriptableObject)scriptableObjectList.materials[Random.Range(0, scriptableObjectList.materials.Count)];
        if (type == typeof(Effect))
            return (T)(ScriptableObject)scriptableObjectList.effects[Random.Range(0, scriptableObjectList.effects.Count)];
        return null;
    }
}
