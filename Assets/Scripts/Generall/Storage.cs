using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage<T>
{
    public System.Action<T> OnAdd;
    public System.Action<T> OnRemove;

    List<T> storage = new List<T>();

    public void Add(T t)
    {
        storage.Add(t);
        OnAdd?.Invoke(t);
    }

    public void Remove(T t)
    {
        storage.Remove(t);
        OnRemove?.Invoke(t);
    }
    
    public void Clear() => storage.Clear();

    public bool Contains(T t) => storage.Contains(t);
}
