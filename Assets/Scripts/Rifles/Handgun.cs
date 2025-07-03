using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handgun : MonoBehaviour
{
    [Header("Player Movement")]
    public float playerspeed = 1.9f;
    public float playerSprint = 5F;

    [Header("Player Animator & Gravity")]
    public CharacterController cC;
    public float gravity = -9.81f;
    public Animator animator;


    [Header("Player Script Camera")]
    public Transform playerCamera;

    [Header("Player Jumping & Velocity")]
    public float jumpRange = 1f;
    public float turnCalmTime = 0.1f;
    float turnCalmVelocity;
    Vector3 velocity;
    public Transform surfaceCheck;
    bool onSurface;
    public float surfaceDistance = 0.4f;
    public LayerMask surfaceMask;


    [Header("Rifle stats")]
    public Camera cam;
    public float ShootDamage = 10f;
    public float ShootRange = 100f;
    public float FireCharge = 10f;
    private float nextTimetoShoot = 0f;
    public Transform hand;
    public Transform PlayerTransform;

    [Header("Ammo")]
    private int maxammo = 10;
    public int mag = 6;
    public float reloadtime = 2.6f;
    private int presentammo;
    private bool setReload = false;

    [Header("Sound & UI")]
    public GameObject NoammoUI;

    [Header("Rifle FX")]
    public ParticleSystem muzzleFlash;
    public GameObject metalImpact;

    private void Awake()
    {   
        transform.SetParent(hand);
        Cursor.lockState = CursorLockMode.Locked;
        presentammo = maxammo;
    }

    private void Update()
    {
        onSurface = Physics.CheckSphere(surfaceCheck.position, surfaceDistance, surfaceMask);

        if (onSurface && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //gravity
        velocity.y += gravity * Time.deltaTime;
        cC.Move(velocity * Time.deltaTime);

        playerMove();
        Jump();
        Sprint();


        if (setReload)
        return;

        if (mag == 0)
        {
            StartCoroutine(ShownoAmmo());
            return;
        }

        if (presentammo ==0)
        {
            StartCoroutine(Reload());
            
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine (Reload());
            return;
        }
        
        if (Input.GetButton("Fire1") && Time.time >= nextTimetoShoot)
        {
            animator.SetBool("Shoot", true);
            nextTimetoShoot = Time.time + 1f / FireCharge;
            Shoot();
        }
        else
        {
            animator.SetBool("Shoot", false);
        }
    }

    void playerMove()
    {
        float horizontal_axis = Input.GetAxisRaw("Horizontal");
        float vertical_axis = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis).normalized;

        if (direction.magnitude >= 0.1f)
        {
            animator.SetBool("WalkForward", true);
            animator.SetBool("RunForward", false);

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(PlayerTransform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);
            PlayerTransform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            cC.Move(moveDirection.normalized * playerspeed * Time.deltaTime);

            jumpRange = 0f;
        }
        else
        {
            animator.SetBool("WalkForward", false);
            animator.SetBool("RunForward", false);

            jumpRange = 1f;
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && onSurface)
        {
            animator.SetBool("IdleGun", false);
            animator.SetTrigger("Jump");

            velocity.y = Mathf.Sqrt(jumpRange * -2 * gravity);
        }
        else
        {
            animator.SetBool("IdleGun", true);
            animator.ResetTrigger("Jump");
        }
    }

    void Sprint()
    {
        if (Input.GetButton("Sprint") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) && onSurface)
        {
            float horizontal_axis = Input.GetAxisRaw("Horizontal");
            float vertical_axis = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis).normalized;

            if (direction.magnitude >= 0.1f)
            {
                animator.SetBool("WalkForward", false);
                animator.SetBool("RunForward", true);

                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(PlayerTransform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);
                PlayerTransform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                cC.Move(moveDirection.normalized * playerSprint * Time.deltaTime);

                jumpRange = 0f;
            }
            else
            {
                animator.SetBool("WalkForward", false);
                animator.SetBool("RunForward", false);

                jumpRange = 1f;
            }
        }
    }

    void Shoot()
    { 
        presentammo--;
        
        muzzleFlash.Play();

        RaycastHit hitInfo;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, ShootRange))
        {
            Debug.Log(hitInfo.transform.name);

            Object obj = hitInfo.transform.GetComponent<Object>();

            if(obj != null)
            {
                obj.objectHitDamage(ShootDamage);
                GameObject metalImpactGo = Instantiate(metalImpact, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(metalImpactGo, 1f);
            }
        }
    }

    IEnumerator Reload()
    {
        setReload = true;
        animator.SetBool("Reload", true);
        yield return new WaitForSeconds(reloadtime);
        animator.SetBool("Reload", false);
        mag--;
        presentammo = maxammo;
        setReload = false;
    }

    IEnumerator ShownoAmmo()
    {
        NoammoUI.SetActive(true);
        yield return new WaitForSeconds(5f);
        NoammoUI.SetActive(false);

    }
}
