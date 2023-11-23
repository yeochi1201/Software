using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityBelt : PassiveItem
{

    override public void OnEquip()
    {
        //active item slot ++
        PlayerSpecification ps = GameObject.Find("Player").GetComponent<PlayerSpecification>();
        ps.ammo_size_buf *= 1.20f;
        Inven inven = GameObject.Find("Player").GetComponent<Inven>();
        foreach (GameObject weapon in inven.weapons)
        {
            weapon.GetComponent<GunBase>().WeaponUpdateAmmoSize(1.20f);
        }
    }


    override public void UnEquip()
    {
        PlayerSpecification ps = GameObject.Find("Player").GetComponent<PlayerSpecification>();
        ps.ammo_size_buf /= 1.20f;
        Inven inven = GameObject.Find("Player").GetComponent<Inven>();
        foreach (GameObject weapon in inven.weapons)
        {
            weapon.GetComponent<GunBase>().WeaponRollbackAmmoSize(1.20f);
        }
    }
}