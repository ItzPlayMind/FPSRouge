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

    #region Physical
    public static void Physical_OnEquip(Effect effect, Item item, WeaponManager manager)
    {
        if (!Utils.IsMagic((manager.MainHandItem as Weapon).DamageType))
        {
            effect.VariableStore["OnChangeDamage"] = new Utils.OnChangeValue<float>((ref float value) =>
            {
                value *= 1 + (effect.Variables[0].Float / 100f);
            });
            (manager.MainHandItem as Weapon).OnChangeDamage += (Utils.OnChangeValue<float>)effect.VariableStore["OnChangeDamage"];
        }
    }
    public static void Physical_OnUnequip(Effect effect, Item item, WeaponManager manager)
    {
        if (!Utils.IsMagic((manager.MainHandItem as Weapon).DamageType))
        {
            (manager.MainHandItem as Weapon).OnChangeDamage -= (Utils.OnChangeValue<float>)effect.VariableStore["OnChangeDamage"];
        }
    }
    #endregion

    #region Magical
    public static void Magical_OnEquip(Effect effect, Item item, WeaponManager manager)
    {
        if (Utils.IsMagic((manager.MainHandItem as Weapon).DamageType))
        {
            effect.VariableStore["OnChangeDamage"] = new Utils.OnChangeValue<float>((ref float value) =>
            {
                value *= 1 + (effect.Variables[0].Float / 100f);
            });
            (manager.MainHandItem as Weapon).OnChangeDamage += (Utils.OnChangeValue<float>)effect.VariableStore["OnChangeDamage"];
        }
    }
    public static void Magical_OnUnequip(Effect effect, Item item, WeaponManager manager)
    {
        if (Utils.IsMagic((manager.MainHandItem as Weapon).DamageType))
        {
            (manager.MainHandItem as Weapon).OnChangeDamage -= (Utils.OnChangeValue<float>)effect.VariableStore["OnChangeDamage"];
        }
    }
    #endregion
}
