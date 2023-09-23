using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectMethods
{
    //public static void NAME__OnPassive(Effect effect, Item item, Transform usepoint, CharacterStats user)
    //{
    //}
    //public static void NAME_OnEquip(Effect effect, Item item, WeaponManager manager)
    //{
    //}
    //public static void NAME_OnUnequip(Effect effect, Item item, WeaponManager manager)
    //{
    //}

    public static string GetEquipMethodName(string name) => name + "_OnEquip";
    public static string GetUnequipMethodName(string name) => name + "_OnUnequip";
    public static string GetPassiveMethodName(string name) => name + "_OnPassive";

    #region Sword
    public static void Sword_OnEquip(Effect effect, Item item, WeaponManager manager)
    {
        effect.VariableStore.Add(new Utils.OnChangeValue<float>((ref float value) =>
        {
            value *= 1.25f;
        }));
        (manager.MainHandItem as Weapon).OnChangeDamage += (Utils.OnChangeValue<float>)effect.VariableStore[0];
    }
    public static void Sword_OnUnequip(Effect effect, Item item, WeaponManager manager)
    {
        (manager.MainHandItem as Weapon).OnChangeDamage -= (Utils.OnChangeValue<float>)effect.VariableStore[0];
    }
    #endregion
}
