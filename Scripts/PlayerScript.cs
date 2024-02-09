using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 

public class PlayerScript : Singleton<PlayerScript>
{

    float movementSpeed = 10f;
    public float mouseSpeed = 250f;
    float verticalRotation = 0f;
    //camera stuff
    public Transform cameraTransform;
    public GameObject cameraRoot;
    //weapons
    public List<GameObject> weapons;
    public GameObject currentWeapon;
    //items
    public GameObject itemToPickUp;
    public GameObject currentItemHolding;
    //projectile
    public GameObject currentProjectile;
    //raycast stuff
    private float timeToraycast = 0.1f;
    private float maxDistance = 8f;
    private float spawnDistance = 5f;
    public bool isRaycastingActive = true;
    //check is in UI
    public bool inUI = false;
    //animation
    public Animator animator;
    private float baseMovementSpeed = 10f; // Normal walking speed (this is for animation)
    private float animationMultiplier = 1.5f;
    private float currentSpeed;
    //arms for fps
    public GameObject FPSArms;
    public GameObject FPSArmsCamera;
    public Transform ProjectileSpawnTransform;

    private LayerMask interactableLayer;
    //make a factory to get the item we need


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraTransform = Camera.main.transform; 
        cameraTransform.SetParent(cameraRoot.transform);
        StartCoroutine(CastRaycast());
        interactableLayer = LayerMask.GetMask("Interactable");
        weapons = new List<GameObject>();
        animator = GetComponent<Animator>();
        FPSArms.transform.SetParent(FPSArmsCamera.transform);
        animator = GetComponent<Animator>();
        GameManagerScript.S.SetTutorial("Move");
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the input direction vector and magnitude
        Vector3 inputDirection = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = inputDirection.magnitude;

        if(inputMagnitude > 0f && GameManagerScript.S.currentTutorial == "Move")
        {
            GameManagerScript.S.AdvanceNextTutorial();
        }
        
        transform.Translate(inputDirection * movementSpeed * Time.deltaTime);


        float horizontalMouse = Input.GetAxis("Mouse X");
        float verticalMouse = Input.GetAxis("Mouse Y");

        // Rotate the player for horizontal look
    transform.Rotate(0, horizontalMouse * mouseSpeed * Time.deltaTime, 0);

    // Tilt the camera for vertical look
     verticalRotation -= verticalMouse * mouseSpeed * Time.deltaTime;
    verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
    cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);



        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Check if the mouse is over a UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // The mouse is over a UI element, do not perform game world interactions
                return;
            }

            if (currentItemHolding != null)
            {
                currentItemHolding.GetComponent<InteractableObject>().Interact(false);
                if (GameManagerScript.S.currentTutorial == "Drop")
                    GameManagerScript.S.AdvanceNextTutorial();

            }
              else if (itemToPickUp != null)
            {
                if (itemToPickUp.GetComponent<InteractableObject>())
                {
                    itemToPickUp.GetComponent<InteractableObject>().Interact(true);
                    if (GameManagerScript.S.currentTutorial == "PickUp")
                        GameManagerScript.S.AdvanceNextTutorial();
                }
                else if (inUI) //are we in the UI?
                {
                    itemToPickUp.GetComponent<ItemDrop>().Interact(false);
                    if (GameManagerScript.S.currentTutorial == "ItemBox1")
                        GameManagerScript.S.AdvanceNextTutorial();

                }
                else if (itemToPickUp.GetComponent<ItemDrop>())
                {
                    itemToPickUp.GetComponent<ItemDrop>().Interact(true);
                    if (GameManagerScript.S.currentTutorial == "ItemBox")
                        GameManagerScript.S.AdvanceNextTutorial();
                }
            }
            else if (currentWeapon != null)
            {
                Fire();
            }
            
        }

        //sprint
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            movementSpeed = movementSpeed * 2;
            //we only want to change the tutorial once... not over and over
            if(GameManagerScript.S.currentTutorial == "Sprint")
            GameManagerScript.S.AdvanceNextTutorial();
        }
        //go back to walk speed.
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            movementSpeed = movementSpeed / 2;
        }
        //animation stuff such as making the animation faster in th animation controller, etc
        currentSpeed = inputMagnitude*movementSpeed;
        float animationSpeed = movementSpeed / baseMovementSpeed;
        animator.speed = animationSpeed * animationMultiplier;
        animator.SetFloat("velocity", currentSpeed);

        if(currentWeapon != null && !animator.GetBool("currentWeapon"))
        {
            animator.SetBool("currentWeapon", true);
        }
        else if(currentWeapon == null && animator.GetBool("currentWeapon"))
        {
            animator.SetBool("currentWeapon", false);
        }

        if (Input.GetKeyDown(KeyCode.E)) 
        {
            WeaponSwap(true);
        }
        else if (Input.GetKeyDown(KeyCode.Q)) 
        {
            WeaponSwap(false);
        }


    }

    public void Fire()
{
        Weapon itemToSpawn = currentWeapon.GetComponent<Weapon>();
        //if we have no ammo or we have no projectile to throw return
        if (itemToSpawn.ammo <= 0 || !currentProjectile)   return;

        AudioManager.S.PlayThrowSound();
        //set the collider to enabled
        currentProjectile.GetComponent<ThrowableItem>().objectCollider.enabled = true;

        Vector3 rayStart = cameraTransform.position;
        Vector3 rayDirection = cameraTransform.forward;
        RaycastHit hit;
        ThrowableItem throwableProjectile = currentProjectile.GetComponent<ThrowableItem>();
        Vector3 direction;

        if (Physics.Raycast(rayStart,rayDirection,out hit, 1000))
        {
            direction = (hit.point - currentProjectile.transform.position).normalized;

        }
        else
        {
            // If it didn't hit anything, use a point far in front of the player
            Vector3 target = rayStart + rayDirection*spawnDistance;
            direction = (target- currentProjectile.transform.position).normalized;            
        }
        throwableProjectile.Throw(direction);


        animator.SetTrigger("fire");
        currentProjectile = null;

        itemToSpawn.ammo -= 1;
        GameManagerScript.S.UpdateAmmoGUI(itemToSpawn.ammo, itemToSpawn.MaxAmmo);

        Debug.Log("FIRING");
        SetProjectile();
      
        if (GameManagerScript.S.currentTutorial == "Fire")
            GameManagerScript.S.AdvanceNextTutorial();
    }


    public IEnumerator CastRaycast()
    {
        while (isRaycastingActive)
        {
            yield return new WaitForSeconds(timeToraycast);

            Vector3 rayStart = cameraTransform.position;
            Vector3 rayDirection = cameraTransform.forward;
            RaycastHit hit;
            // Perform the raycast
            if (Physics.Raycast(rayStart, rayDirection, out hit, maxDistance, interactableLayer))
            {
                // Get the distance from the hit point
                float distanceToHit = (hit.point - rayStart).magnitude;

                // Adjust the ray start offset based on the distance and clamp it
                float adjustedOffset = Mathf.Clamp(distanceToHit - 0.1f, 0.1f, 3f);
                rayStart = cameraTransform.position + cameraTransform.forward * adjustedOffset;

                // Create a second raycast because we can't detect the object we're in front of if we're too close
                if (Physics.Raycast(rayStart, rayDirection, out hit, maxDistance, interactableLayer))
                {
                    GameObject hitObject = hit.collider.gameObject;

                    // Remove highlight from the previous item if it exists
                    if (itemToPickUp != null && itemToPickUp.GetComponent<InteractableObject>() != null)
                    {
                        itemToPickUp.GetComponent<InteractableObject>().RemoveHighlight();
                    }

                    itemToPickUp = hitObject;

                    // Highlight the new item if it's interactable
                    if (itemToPickUp != null && itemToPickUp.GetComponent<InteractableObject>() != null)
                    {
                        itemToPickUp.GetComponent<InteractableObject>().Highlight();
                    }

                }
                else
                {
                    // Remove highlight if no interactable object is hit
                    if (itemToPickUp != null && itemToPickUp.GetComponent<InteractableObject>() != null)
                    {
                        itemToPickUp.GetComponent<InteractableObject>().RemoveHighlight();
                    }
              
                    itemToPickUp = null;
                }
            }
            else
            {
                // Remove highlight if the raycast hits nothing
                if (itemToPickUp != null && itemToPickUp.GetComponent<InteractableObject>() != null)
                {
                    itemToPickUp.GetComponent<InteractableObject>().RemoveHighlight();
                }
                itemToPickUp = null;
            }
        }
    }


    public void WeaponSwap(bool nextWeapon)
    {
        if (weapons.Count > 1)
        {

            //get the index of the current weapon this is so we dont have to add a new variable to this class
            int currentIndex = weapons.IndexOf(currentWeapon);
            int newIndex;

            if (nextWeapon)
            {
                //set the new index to the next one, we add one and then mod it so we can loop through once we hit the max size of the weapon
                newIndex = (currentIndex + 1) % weapons.Count;
            }
            else
            {
                // same here but we just go backwards
                newIndex = (currentIndex - 1 + weapons.Count) % weapons.Count;
            }

            //equip the weapon of the new index
            EquipWeapon(weapons[newIndex]);
            GameManagerScript.S.UpdateWeaponCyclerUI();
        }
    }


    public void EquipWeapon(GameObject weapon)
    {
        //if there is already a projectile in our hand when equipping, get rid of it we dont want it to stack.
        if (currentProjectile != null)
        {
            GameManagerScript.S.DeactivateProjectile(currentProjectile);
        }

        //set the currentWeapon
        currentWeapon = weapon;

        Weapon weaponScript = currentWeapon.GetComponent<Weapon>();
        //Set the projectile to the hand.
        SetProjectile();
        //update the ammo UI
        GameManagerScript.S.UpdateAmmoGUI(weaponScript.ammo, weaponScript.MaxAmmo);


    }

    public void SetProjectile()
    {
        //this sets the projectile to the hand socket we created in the editor.
        Weapon weaponScript = currentWeapon.GetComponent<Weapon>();
        //if we have no more ammo, return because we dont want to show we have ammo when we really dont.
        if (weaponScript.ammo <= 0) return;
        currentProjectile = GameManagerScript.S.GetProjectileFromPool(weaponScript.currentItem);
        currentProjectile.transform.position = ProjectileSpawnTransform.position;
        currentProjectile.transform.SetParent(ProjectileSpawnTransform);
        currentProjectile.SetActive(true);
    }

    //when something hits the enemy 
    void OnCollisionEnter(Collision other)
    {
     
        if (other.gameObject.CompareTag("EnemyThrow"))
        {
            //AudioManager.Instance.PlayHit();

        
             Debug.Log("player hit and read");
            GameManagerScript.S.PlayerHealth -= 10;

            AudioManager.S.PlayerDMGSound();

            StartCoroutine(GameManagerScript.S.DamageFlash());
        }

    }

}
