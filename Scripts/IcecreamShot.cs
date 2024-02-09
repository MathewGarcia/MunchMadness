
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcecreamShot : MonoBehaviour
{
    //spawns scoops
    public GameObject scoop;

    private bool hasSpawned = false;


    //when the cake hits something
    void OnCollisionEnter(Collision other)
    {
        Debug.Log("icecream collided!");

        if ((other.gameObject.CompareTag("enemy")) && (hasSpawned == false))
        {
            //AudioManager.Instance.PlayHit();

            //spawning objects left and right 
            Instantiate(scoop, this.transform.position + Vector3.right, Quaternion.identity);
            Instantiate(scoop, this.transform.position + Vector3.left, Quaternion.identity);

            hasSpawned = true;
        }


    }


}