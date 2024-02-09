using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    public int ammo;
    public GameObject currentItem;
    public int MaxAmmo;
    // Start is called before the first frame update
    void Start()
    {
        ammo = MaxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
