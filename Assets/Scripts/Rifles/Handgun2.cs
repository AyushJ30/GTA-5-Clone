using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handgun2 : MonoBehaviour
{
    [Header("Rifle stats")]
    public Camera cam;
    public float ShootDamage = 10f;
    public float ShootRange = 100f;
    public float FireCharge = 10f;
    private float nextTimetoShoot = 0f;
    public Transform hand;

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
        if (setReload)
            return;

        if (mag == 0)
        {
            StartCoroutine(ShownoAmmo());
            return;
        }

        if (presentammo == 0)
        {
            StartCoroutine(Reload());

            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimetoShoot)
        {
            nextTimetoShoot = Time.time + 1f / FireCharge;
            Shoot();
        }
    }

    void Shoot()
    {
        presentammo--;

        muzzleFlash.Play();

        RaycastHit hitInfo;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, ShootRange))
        {
            Debug.Log(hitInfo.transform.name);

            Object obj = hitInfo.transform.GetComponent<Object>();

            if (obj != null)
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
        yield return new WaitForSeconds(reloadtime);
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
