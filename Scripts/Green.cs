using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Green : IEnemy
{



    public void Spawn()
    {

    }

    //when something hits the enemy 
    void OnCollisionEnter(Collision other)
    {
        //check what hit the enemy and if it matches enemy type 
        //stawberry cake cookie 
        if (other.gameObject.GetComponent<ThrowableItem>())
        {
            ThrowableItem throwableItem = other.gameObject.GetComponent<ThrowableItem>();
            //AudioManager.Instance.PlayHit();

            //if projectile matches enemy type
            //if (other.gameObject.CompareTag("MATCHES ENEMY TYPE****************"))
            if ("Sandwich" == throwableItem.name) 
            {
                Debug.Log(throwableItem.name + " hit and read");
                health -= 40;
                aggrotime += 10;
            }
            else // if not the right order take some health but decrease time till agro 
            {
                health -= 10;
                time += 10;
            }


        }



    }
}



