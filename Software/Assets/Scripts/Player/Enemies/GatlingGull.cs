using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GatlingGull : Enemy
{
    [Header("Enemy Property")]
    Rigidbody2D rb;
    Collider2D coll;
    Animator enemyAnim;
    SpriteRenderer spritecompo;

    float attackTimer = 0f;
    bool isCoroutine = false;
    bool isAlive = true;
    
    Transform playertrans;

    [Header("Gun Property")]
    //[SerializeField] GameObject gunPrefab;
    [SerializeField] GameObject muzzle;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject gunPivot;
    [SerializeField] Vector2 muzzleDirection;
    Vector2 muzzleInitDir;
    [SerializeField] GameObject BossUI;
    GameObject BossUIHP;
    Image BossHP;
    Vector2 target;
    float angle;

    int bulletCount = 30;
    int angleInterval;
    bool isPatternStart = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider2D>();
        spritecompo = GetComponent<SpriteRenderer>();
        
        BossUIHP = Instantiate(BossUI);
        BossHP = BossUIHP.transform.Find("BossHP").GetComponent<Image>();
        playertrans = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }
    // 정의할 상태
    // 열거형(Enum)

    private void Start()
    {
        muzzleInitDir = muzzle.transform.right;
    }
    public enum EnemyState
    {
        Idle,
        Chasing,
        Attacking,
        Dead
    }

    private EnemyState currentState = EnemyState.Idle;

    private void Update()
    {
        if (!isCoroutine && isAlive)
        {
            attackTimer += Time.deltaTime;
            BossHP.fillAmount = health / maxHealth;

            distance = Vector3.Distance(playertrans.position, this.transform.position);

            if (rb.transform.position.x < playertrans.position.x)
                spritecompo.flipX = true;
            else spritecompo.flipX = false;
            FollowPlayer();


            if (currentState != EnemyState.Attacking && distance <= moveRange) StartChasing();
            else ChangeState(EnemyState.Idle);

            if (currentState == EnemyState.Chasing && attackTimer >= attackCooldown)
                StartAttacking();

            if (health <= 0)
            {
                BossUIHP.SetActive(false);
                Die();
            }

            switch (currentState)
            {
                case EnemyState.Idle:
                    UpdateIdleState();
                    break;

                case EnemyState.Chasing:
                    UpdateChasingState();
                    break;

                case EnemyState.Attacking:
                    StartCoroutine(UpdateAttackingState());
                    break;

                case EnemyState.Dead:
                    enemyAnim.SetTrigger("Die");
                    coll.enabled = false;
                    isAlive = false;
                    break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (health > 0)
        {
            if (collision.gameObject.tag == "PlayerBullet")
            {
                /*
                float getDamage = collision.gameObject.GetComponent<Enemy>().damage;
                health -= getDamage;
                */
                health -= 1;
            }
        }
    }

    private void UpdateIdleState()
    {

        enemyAnim.SetBool("isWalking", false);

        //idle 상태에서는 정지
        rb.velocity = Vector2.zero;
    }

    private void UpdateChasingState()
    {

        enemyAnim.SetBool("isWalking", true);

        Vector2 playerDir = playertrans.position - rb.transform.position;
        Vector2 nextVec = playerDir.normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + nextVec);
        rb.velocity = Vector2.zero;
    }

    IEnumerator UpdateAttackingState()
    {
        isCoroutine = true;

        rb.velocity = Vector2.zero;

        enemyAnim.SetBool("isShooting", true);
        enemyAnim.SetBool("isWalking", false);

        attackTimer = 0f;

        int randAction = Random.Range(0, 4);

        switch (randAction)
        {
            case 0:
                StartCoroutine(Pattern1());
                break;
            case 1:
                StartCoroutine(Pattern2());
                break;
            case 2:
                {
                    StartCoroutine(CirclePattern(30, Random.Range(4, 6)));
                    break;
                }                
            case 3:
                {
                    StartCoroutine(CircleRotatePattern(30, Random.Range(1, 3)));
                    break;
                }
            default:
                break;                
        }

        yield return null;
    }

    IEnumerator Pattern1()
    {

        coll.enabled = false;
        enemyAnim.SetBool("isShooting", false);
        enemyAnim.SetBool("pattern1", true);

        attackTimer = 0f;
        yield return new WaitForSeconds(1f);

        float pattern1Cool = attackCooldown / 2;

        while (attackTimer < pattern1Cool)
        {
            Vector2 playerDir = playertrans.position - rb.transform.position;
            Vector2 nextVec = playerDir * speed * Time.deltaTime * 3f;
            rb.MovePosition(rb.position + nextVec);

            attackTimer += Time.deltaTime;

            yield return new WaitForSeconds(0.01f);
        }
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        coll.enabled = true;

        attackTimer = 0f;
        isCoroutine = false;
        enemyAnim.SetBool("pattern1", false);

        yield return null;
    }

    IEnumerator Pattern2()
    {

        isCoroutine = true;

        rb.velocity = Vector2.zero;

        enemyAnim.SetBool("isShooting", true);

        attackTimer = 0f;
        yield return new WaitForSeconds(1f);
        int count = 0;

        while (attackTimer < attackCooldown)
        {

            Vector2 playerDir = playertrans.position - rb.transform.position;
            Vector2 nextVec = playerDir * speed * Time.deltaTime * 0.3f;
            rb.MovePosition(rb.position + nextVec);

            if (rb.transform.position.x < playertrans.position.x)
                spritecompo.flipX = true;
            else spritecompo.flipX = false;

            attackTimer += Time.deltaTime;

            count++;
            if(count % 100 ==0)
                Shooting();

            FollowPlayer();

            if (health <= 0)
            {
                attackTimer += attackCooldown;
            }

            yield return new WaitForSeconds(0.01f);
        }
        rb.velocity = Vector2.zero;

        attackTimer = 0f;
        isCoroutine = false;
        enemyAnim.SetBool("isShooting", false);

        yield return null;
    }

    IEnumerator Pattern3()
    {
        yield return null;
    }

    public void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }

    public void StartChasing()
    {
        ChangeState(EnemyState.Chasing);
    }

    public void StartAttacking()
    {
        ChangeState(EnemyState.Attacking);
    }

    public void Die()
    {
        ChangeState(EnemyState.Dead);
        this.GetComponentInParent<Spawner>().CheckEnemyCount();
    }

    void FollowPlayer() //조준
    {
        if(!isPatternStart)
        {
            // 몬스터의 위치에서 플레이어의 위치를 향하는 벡터를 계산
            Vector3 directionToPlayer = playertrans.position - gunPivot.transform.position;

            float angleToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            gunPivot.transform.rotation = Quaternion.AngleAxis(angleToPlayer, Vector3.forward);
        }       
    }

    void Shooting()
    {
        muzzleDirection = muzzle.transform.right;
        GameObject _projectile = ProjectilePooler.Instance.GetProjectile(ProjectileType.ENEMY_BASIC);
        _projectile.GetComponent<EnemyBasicProjectile>().SetProjectileProperty("EnemyBasic", 1, 10, 40, 0, 0, 0, muzzleDirection);
        _projectile.transform.position = muzzle.transform.position;
        _projectile.gameObject.SetActive(true);
    }


#region //LSH CODE//

    IEnumerator CirclePattern(int _bulletCount, int patternCount)//Pattern 2 or higher -> cross pattern, bulletSpeed = 8f
    {
        isCoroutine = true;

        rb.velocity = Vector2.zero;

        enemyAnim.SetBool("isShooting", true);

        isPatternStart = true;
        if (patternCount <= 1)
        {
            bulletCount = _bulletCount;
            angleInterval = 360 / bulletCount;
            for (int i = 0; i < patternCount; i++)
            {
                for (int j = 1; j <= bulletCount; j++)
                {
                    muzzleDirection = muzzle.transform.right;
                    GameObject _projectile = ProjectilePooler.Instance.GetProjectile(ProjectileType.ENEMY_ANOMALY);
                    _projectile.GetComponent<AnomalyProjectile>().SetProjectileProperty("EnemyBasic", 1, 10, 40, 0, 0, 0, muzzleDirection);
                    _projectile.transform.position = muzzle.transform.position;
                    _projectile.gameObject.SetActive(true);

                    muzzle.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleInterval * j));
                }
            }
        }
        else if (patternCount > 1)
        {
            bulletCount = _bulletCount;
            angleInterval = 360 / bulletCount;

            for (int i = 0; i < patternCount; i++)
            {
                for (int j = 1; j <= bulletCount; j++)
                {
                    if (i % 2 == 0)
                    {
                        muzzle.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleInterval * (j + 0.5f)));
                    }
                    else
                    {
                        muzzle.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleInterval * j));
                    }
                    muzzleDirection = muzzle.transform.right;
                    GameObject _projectile = ProjectilePooler.Instance.GetProjectile(ProjectileType.ENEMY_ANOMALY);
                    _projectile.GetComponent<AnomalyProjectile>().SetProjectileProperty("EnemyBasic", 1, 10, 40, 0, 0, 0, muzzleDirection);
                    _projectile.transform.position = muzzle.transform.position;
                    _projectile.gameObject.SetActive(true);
                }

                yield return new WaitForSeconds(.3f);

            }
        }
        isPatternStart = false;
        muzzleDirection = muzzleInitDir;
        attackTimer = 0f;
        isCoroutine = false;
        enemyAnim.SetBool("isShooting", false);
    }

    IEnumerator CircleRotatePattern(int _bulletCount, int patternCount)
    {
        isCoroutine = true;

        rb.velocity = Vector2.zero;

        enemyAnim.SetBool("isShooting", true);
        isPatternStart = true;
        bulletCount = _bulletCount;
        angleInterval = 360 / bulletCount;

        for (int i = 0; i < patternCount; i++)
        {
            for (int j = 1; j <= bulletCount; j++)
            {
                yield return new WaitForSeconds(0.05f);

                muzzleDirection = muzzle.transform.right;
                GameObject _projectile = ProjectilePooler.Instance.GetProjectile(ProjectileType.ENEMY_BASIC);
                _projectile.GetComponent<EnemyBasicProjectile>().SetProjectileProperty("EnemyBasic", 1, 10, 40, 0, 0, 0, muzzleDirection);
                _projectile.transform.position = muzzle.transform.position;
                _projectile.gameObject.SetActive(true);

                muzzle.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleInterval * j));

            }
        }
        isPatternStart = false;
        muzzleDirection = muzzleInitDir;
        attackTimer = 0f;
        isCoroutine = false;
        enemyAnim.SetBool("isShooting", false);
    }
 #endregion
}
