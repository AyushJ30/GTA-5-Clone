using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    [Header("Object health")]
    public float Objhealth = 100f;

    public void objectHitDamage(float amount)
    {
        Objhealth -= amount;

        if(Objhealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
