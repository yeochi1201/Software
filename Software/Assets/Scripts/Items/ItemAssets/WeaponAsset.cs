using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemObject/Weapon")]
public class WeaponAsset : ItemAsset
{
    [SerializeField]
    public int magazine; //źâ
    public int ammo_count; //ź�˼�
    public float reload; //reload time
    public int ammo_speed;
    public int knock_back;
    public float damage; //weapon damage
    public float delay; //delay at fire
    public int range;
    public int ammo_degree;

    public Vector2 muzzle_loc;
    public enum weapontype 
    { 
        half_auto,
        auto,
        charge,
        beam,
        spread
    }
    public weapontype type;

    public enum projectiletype
    {
        VELOCITY,
        HELIX,
        HOMING,
        COLOR,
        TRAIL,
        RAY,
        SPLIT,
        MAX_SIZE
    }
    public projectiletype proejctileType;
}
