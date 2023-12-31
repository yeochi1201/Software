using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponItem : MonoBehaviour
{
    [SerializeField]
    public WeaponAsset weapon;
    private bool is_reload = false;
    private bool is_delay = false;
    public Bullet ammo;
    private GameObject muzzle;
    private Vector2 muzzle_direction;
    private bool is_equip = false;
    private GameObject gunPivot;

    public int magazine; //źâ
    public int ammo_count; //ź�˼�
    public float reload; //reload time
    public int ammo_speed;
    public int knock_back;
    public float damage; //weapon damage
    public float delay; //delay at fire
    public int range;
    public int ammo_degree;

    public int current_magazine;
    public int current_ammo_count;
    
    public void Start()
    {
        magazine = weapon.magazine;
        ammo_count = weapon.ammo_count;
        reload = weapon.reload;
        ammo_speed = weapon.ammo_speed;
        knock_back = weapon.knock_back;
        damage = weapon.damage;
        delay = weapon.delay;
        range = weapon.range;
        ammo_degree = weapon.ammo_degree;

        current_ammo_count = ammo_count;
        current_magazine = magazine;
    }

    public void Update()
    {
        if(!ItemManager.itemManager.invenUI.IsPaused && SceneManager.GetActiveScene().name != "BaseCamp")
        {
            if (is_equip && !is_delay && !is_reload && Input.GetMouseButtonDown(0) && current_magazine > 0)
            {
                Shooting();
                StartCoroutine(DelayTime());
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
                StartCoroutine(ReloadTime());
            }
        } 
    }

    public void OnTriggerStay2D(Collider2D collision)
    {

        if (Input.GetKeyDown(KeyCode.E) && collision.CompareTag("Player"))
        {
            Inven inven = collision.GetComponentInParent<Inven>();
            if (inven.weapons.Count < 6)
            {
                PlayerSpecification ps = collision.GetComponentInParent<PlayerSpecification>();
                WeaponUpdateDegree(ps.ammo_degree_buf);
                WeaponUpdateAmmoCount(ps.ammo_count_buf);
                WeaponUpdateAmmoSize(ps.ammo_size_buf);
                WeaponUpdateDamage(ps.weapon_dmg_buf);
                WeaponUpdateDelay(ps.weapon_delay_buf);
                WeaponUpdateReload(ps.weapon_reload_buf);
                inven.GetWeapon(this.gameObject);
                PlayerGetGun();
            }
        }
    }

    public void PlayerGetGun()
    {
        gunPivot = GameObject.FindWithTag("Player").transform.GetChild(0).gameObject;
        this.gameObject.transform.SetParent(gunPivot.transform);
        this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 4;
        this.gameObject.transform.position = gunPivot.transform.position;
        this.gameObject.transform.rotation = gunPivot.transform.rotation;
        this.gameObject.SetActive(false);
    }
    public void OnEquip()
    {
        is_equip = true;
        this.gameObject.SetActive(true);
        gunPivot = GameObject.FindWithTag("Player").transform.GetChild(0).gameObject;
        muzzle = gunPivot.transform.GetChild(0).gameObject;
        ProjectilePooler.Instance.type = (ProjectileType)weapon.proejctileType;
        muzzle.transform.localPosition = weapon.muzzle_loc;
    }
    public void Unequip()
    {
        is_equip = false;
        this.gameObject.SetActive(false);
    }

    public void Shooting()
    {
        muzzle_direction = muzzle.transform.right;
        GameObject _projectile = ProjectilePooler.Instance.GetProjectile(ProjectilePooler.Instance.type);
        _projectile.GetComponent<Projectile>().SetProjectileProperty(weapon.name, damage, ammo_speed, weapon.range, 0, 0, 0, muzzle_direction);
        _projectile.transform.position = muzzle.transform.position;
        _projectile.gameObject.SetActive(true);
        current_magazine -= 1;
        is_delay = true;
    }

    public void Reload()
    {
        is_reload = true;
        if (weapon.ammo_count == -1)
        {
            current_magazine = magazine;
        }
        else
        {
            int need_magazine = magazine - current_magazine;
            if (current_ammo_count < current_ammo_count - need_magazine)
            {
                current_magazine += current_ammo_count;
                current_ammo_count = 0;
            }
            else
            {
                current_ammo_count -= need_magazine;
                current_magazine = magazine;
            }
        }
    }


    IEnumerator DelayTime()
    {
        yield return new WaitForSeconds(weapon.delay);
        is_delay = false;
    }
    IEnumerator ReloadTime()
    {
        yield return new WaitForSeconds(weapon.reload);
        is_reload = false;
    }

    public void WeaponUpdateDegree(float ammo_degree_buf)
    {
        ammo_degree = (int)(weapon.ammo_degree * ammo_degree_buf);
    }

    public void WeaponUpdateDamage(float weapon_damage_buf)
    {
        damage = weapon.damage * weapon_damage_buf;
    }

    public void WeaponUpdateAmmoSize(float ammo_size_buf)
    {
        magazine = (int)(weapon.magazine * ammo_size_buf);
    }

    public void WeaponUpdateReload(float weapon_reload_buf)
    {
        reload = (weapon.reload * weapon_reload_buf);
    }

    public void WeaponUpdateDelay(float weapon_delay_buf)
    {
        delay = (weapon.delay * weapon_delay_buf);
    }

    public void WeaponUpdateAmmoCount(float ammo_count_buf)
    {
        current_ammo_count = (int)(current_ammo_count * ammo_count_buf);
    }
}
