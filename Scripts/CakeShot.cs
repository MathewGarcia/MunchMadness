using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CakeShot : MonoBehaviour
{
    private bool hasHit = false;
    public ParticleSystem shoteffect;

    //when the pop hits something
    void OnCollisionEnter(Collision other)
    {
        shoteffect = GetComponent<ParticleSystem>();
        Debug.Log("cake collided!");

        if ((other.gameObject.CompareTag("enemy")) && (hasHit == false))
        {
            shoteffect.Play();
           //add velocity and throw the enemy back
            hasHit = true;

        }

    }
}
