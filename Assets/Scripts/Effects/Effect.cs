using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect", menuName = "Effects/New Effect")]
public class Effect : ScriptableObject
{
    

    [Multiline]
    [SerializeField] private string description;
    [SerializeField] private Utils.CustomValue[] variables;

    public Utils.CustomValue[] Variables { get => variables; }
   

    public string Description { get => DescriptionCreator.Generate(description, Variables); }

    private Dictionary<string, object> variableStore = new Dictionary<string, object>();
    public Dictionary<string,object> VariableStore { get => variableStore; }

    private MethodInfo passiv;
    private MethodInfo onequip;
    private MethodInfo onunequip;

    public void Setup()
    {
        onequip = typeof(EffectMethods).GetMethod(EffectMethods.GetEquipMethodName(name));
        passiv = typeof(EffectMethods).GetMethod(EffectMethods.GetPassiveMethodName(name));
        onunequip = typeof(EffectMethods).GetMethod(EffectMethods.GetUnequipMethodName(name));
    }

    public void Passive(Item item, Transform usePoint, CharacterStats user) => passiv?.Invoke(null, new object[] { this, item, usePoint, user });
    public void OnEquip(Item item, WeaponManager manager) => onequip?.Invoke(null, new object[] { this, item, manager });
    public void OnUnequip(Item item, WeaponManager manager) => onunequip?.Invoke(null, new object[] { this, item, manager });
}
