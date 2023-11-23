using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : PlayerSpecification
{
    Rigidbody2D playerRigidbody;
    SpriteRenderer spritecompo;
    //[SerializeField] GameObject gunPrefab;
    Vector3 mouse;
    Animator playerAnim;
    Collider2D coll;


    [Header("Slide Property")]
    Vector2 slideDirection;
    [SerializeField]float slideSpeed = 5f;
    float slideDuration = 0.5f;
    float slideCooldown = 0.5f;
    float slideTimer = 0f;
    bool isSlide = false;


    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        spritecompo = GetComponent<SpriteRenderer>();
        coll = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        if(currentHP<=0) Die();
        
        FollowMouse();
        PlayerMove();

        if (Input.GetMouseButtonDown(0))
        {
            Shooting();
        }
    }

    public void PlayerMove()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        if (inputX < 0) spritecompo.flipX = true;
        else spritecompo.flipX = false;

        Vector2 playerVelocity = new Vector2(inputX, inputY);
        //playerVelocity.Normalize();

        // Player Move
        playerRigidbody.velocity = playerVelocity * speed;

        if (Input.GetKeyDown(KeyCode.Space) && !isSlide)
        {
            if (Mathf.Abs(inputX) > 0.1f || Mathf.Abs(inputY) > 0.1f)
            {
                slideDirection = playerVelocity.normalized;

                StartCoroutine(Slide());
            }
        }
    }

    void FollowMouse() //CrossHair
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 calc = mouse - this.transform.position;

        float angle = Mathf.Atan2(calc.y, calc.x) * Mathf.Rad2Deg;
        gunPivot.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Shooting()
    {
        muzzleDirection = muzzle.transform.right;

        /*
        GameObject _bullet = BulletPooler.Instance.GetBullet(BulletOwner.PLAYER);
        _bullet.transform.position = muzzle.transform.position;
        _bullet.GetComponent<Bullet>().SetDirection(muzzleDirection);
        _bullet.SetActive(true);
        */
    }

    IEnumerator Slide()
    {
        isSlide = true;
        playerAnim.SetBool("isRolling", true);
        slideTimer = 0f;
        coll.enabled = false;

        while (slideTimer < slideDuration)
        {
            playerRigidbody.velocity = slideDirection * slideSpeed;

            slideTimer += Time.deltaTime;
            yield return null;
        }

        playerRigidbody.velocity = Vector2.zero;
        isSlide = false;
        playerAnim.SetBool("isRolling", false);
        coll.enabled = true;

        yield return new WaitForSeconds(slideCooldown);
    }

    void Die()
    {
        playerAnim.SetTrigger("Die");

        // Scene Gameover�� �̵�
    }
}
