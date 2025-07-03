using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    [Header("Cam to assign")]
    public GameObject Aimcam;
    public GameObject TPcam;
    public Animator animator;

    private void Update()
    {
        if(Input.GetButton("Fire2") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            animator.SetBool("AimWalk", true);
            animator.SetBool("ShootAim", false);

            TPcam.SetActive(false);
            Aimcam.SetActive(true);
        }
        else if (Input.GetButton("Fire2"))
        {
            animator.SetBool("AimWalk", true);
            animator.SetBool("ShootAim", true);
            
            Aimcam.SetActive(true);
            TPcam.SetActive(false);
        }
        else
        {
            animator.SetBool("AimWalk", false);
            animator.SetBool("ShootAim", false);
            
            TPcam.SetActive(true);
            Aimcam.SetActive(false);
        }
    }
}
