using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItem : MonoBehaviour
{

    public float speed;
    public Sprite imageSprite;
    private Rigidbody rb;
    public Collider objectCollider;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //throw me.
    public void Throw(Vector3 direction)
    {
        Debug.Log("Throwing projectile", this);

        if (rb != null)
        {
            transform.SetParent(null);
            //turn off iskinematic
            rb.isKinematic = false;
            //throw it
            rb.velocity = direction * speed;
          }
    }

    //whenever we enable the projectile we want to shut off the collider
    private void OnEnable()
    {
        objectCollider = GetComponent<Collider>();
        objectCollider.enabled = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;
        GameManagerScript.S.DeactivateProjectile(gameObject);
    }
   
}