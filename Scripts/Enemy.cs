using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : IEnemy
{

    public IEnumerator Timer()
    {

        yield return new WaitForSeconds(time);
    }

    // Start is called before the first frame update
    void Start()
    {



        rb = GetComponent<Rigidbody>();
        

    }

    // Update is called once per frame
    void Update()
    {



    }


}
