using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OiledCylinder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerSpecification ps = GameObject.Find("Player").GetComponent<PlayerSpecification>();
        ps.weapon_reload_buf *= 1.20f;
        Inven inven = GameObject.Find("Player").GetComponent<Inven>();
        foreach (GameObject weapon in inven.weapons)
        {
            weapon.GetComponent<GunBase>().WeaponUpdateReload(1.20f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlayerSpecification ps = GameObject.Find("Player").GetComponent<PlayerSpecification>();
        ps.weapon_reload_buf /= 1.20f;
        Inven inven = GameObject.Find("Player").GetComponent<Inven>();
        foreach (GameObject weapon in inven.weapons)
        {
            weapon.GetComponent<GunBase>().WeaponRollbackReload(1.20f);
        }
    }
}