using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager itemManager;
    private Inven inven = GameObject.Find("Player").GetComponent<Inven>();
    private PlayerSpecification ps = GameObject.Find("Player").GetComponent<PlayerSpecification>();
    private bool[] CursedItems = new bool[3];
    private void Awake()
    {
        itemManager = this;
    }

    public void Cursed_Changed(int cursed)
    {
        if (cursed > 0)
        {
            if (CursedItems[0])
            {
                ps.coolness += cursed * 2;
            }
            if (CursedItems[1])
            {
                for (int i = 0; i < cursed; i++)
                {
                    ps.weapon_dmg_buf *= 1.1f;
                    foreach (GameObject weapon in inven.weapons)
                    {
                        for (int j = 0; j < ps.cursed; j++)
                        {
                            weapon.GetComponent<GunBase>().WeaponUpdateDamage(1.1f);
                        }
                    }
                }
            }
            if (CursedItems[2])
            {
                //
            }
        }
        else
        {
            if (CursedItems[0])
            {
                ps.coolness -= cursed * 2;
            }
            if (CursedItems[1])
            {
                for (int i = 0; i < cursed; i++)
                {
                    ps.weapon_dmg_buf /= 1.1f;
                    foreach (GameObject weapon in inven.weapons)
                    {
                        for (int j = 0; j < ps.cursed; j++)
                        {
                            weapon.GetComponent<GunBase>().WeaponRollbackDamage(1.1f);
                        }
                    }
                }
            }
            if (CursedItems[2])
            {
                //
            }
        }
        
    }

    public void SetExist(int i, bool is_exist)
    {
        CursedItems[i] = is_exist;
    }
}
