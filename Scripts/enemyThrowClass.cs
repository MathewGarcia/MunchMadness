using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyThrowClass : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(destroyProjectile());
    }

    private IEnumerator destroyProjectile()
    {
        //wait half a second and destroy this projectile
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
