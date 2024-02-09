using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class IEnemy : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 100;
    public float health = 100;

    public float shotCooldown = 2.0f;
    private float lastShotTime; 

    // Timer and aggression time
    public float time;
    public float aggrotime;

    // NavMeshAgent reference
    private NavMeshAgent agent;

    // Player reference
    public Transform player;

    // Layer masks
    public LayerMask ground, p;

    // Patrol points and range
    public Vector3 walkPoint;
    public bool WalkPointSet;
    public float walkRange;

    // Cafe entrance coordinates
    Vector3 cafe = new Vector3(8.28f, 1.22f, -21.33f);

    // Sight and attack ranges
    public float sightRange, attackRange;
    public bool playerInSight, playerInAttackRange;

    // Animator
    public Animator animator;
    private float animationMultiplier = 1.5f;

    public ParticleSystem deathPoof;
    public GameObject coffeecup;

    //types of food
    public List<GameObject> types;
    public int amountOfFoods;
    public Transform enemyHorizontalGroup;

    public int amountOfScore;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = PlayerScript.S.transform;
        rb = GetComponent<Rigidbody>();
        attackRange = 4.0f;
    }

    private void Start()
    {
        time = 0;

        //increase the amount of current enemies
        GameManagerScript.S.CurEnemys++;
        //also reduce the amount of enemy spawn total on spawn enemy.(we would call this in spawner but it makes more sense here since the enemy is actually already spawned)
        GameManagerScript.S.EnemySpawnTotal--;

        // Set destination to cafe
        agent.SetDestination(cafe);
    }

    private void Update()
    {
        // animation speed adjustment
        animator.SetFloat("velocity", (agent.velocity.magnitude / agent.speed) * animationMultiplier);

        time += Time.deltaTime;


        //TEMPORARILY DISABLE***************************************************
        if (time > aggrotime)
        {
            //Debug.Log("enemy is attacking");
            Attack(); 
        }

        // Example patrol and attack logic
        // Patroling();
        // Attack();

        if( health <= 0)
        {
            Die();
        }
    }

    public void Patroling()
    {
        if (!WalkPointSet)
        {
            SearchWalkPoint();
        }

        if (WalkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distance = transform.position - walkPoint;
        if (distance.magnitude < 1f)
        {
            WalkPointSet = false;
        }
    }

    public void Attack()
    {
        agent.SetDestination(player.position);
        agent.stoppingDistance = attackRange;

        if (Time.time - lastShotTime >= shotCooldown)
        {
            if (Vector3.Distance(transform.position, player.position) <= 5f)
            {
                StartCoroutine(PerformAttack());
                lastShotTime = Time.time;
            }
        }
  

        animator.SetTrigger("attacking");
    }

    private IEnumerator PerformAttack()
    {
        GameObject throwableObject = Instantiate(coffeecup, transform.position + new Vector3(0, -0.2f, 0), Quaternion.identity); // Adjust Y position
        Rigidbody throwableRb = throwableObject.GetComponent<Rigidbody>();

        Vector3 direction = (player.position - transform.position).normalized;
        Debug.DrawLine(transform.position, transform.position + direction * 10, Color.red, 2.0f); // Draws a line in the scene view

        throwableRb.AddForce(direction * 1000f);

        yield return new WaitForSeconds(0.5f);  // Adjust the delay as needed
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkRange, walkRange);
        float randomX = Random.Range(-walkRange, walkRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, ground))
        {
            WalkPointSet = true;
        }
    }

    private void Die()
    {
        Debug.Log("enemy died");
        Instantiate(deathPoof, transform.position, Quaternion.identity);
        GameManagerScript.S.levelpoints += amountOfScore;
        GameManagerScript.S.CurEnemys--;
        GameManagerScript.S.UpdateScoreGUI();
        Destroy(gameObject);

    }
    //get a random food and set it for that enemy and give it an image
    public void SetRandomFoodType()
    {
        for (int i = 0; i < amountOfFoods; i++)
        {
            int randomNum = Random.Range(0, GameManagerScript.S.foodTypes.Count);
            GameObject foodToEat = GameManagerScript.S.foodTypes[randomNum];
            types.Add(foodToEat);

            if (enemyHorizontalGroup)
            {
                GameObject image = Instantiate(GameManagerScript.S.itemDisplayPrefab, enemyHorizontalGroup);
                Image foodToEatImage = image.GetComponent<Image>();

                ThrowableItem throwable = foodToEat.GetComponent<ThrowableItem>();

                if (throwable != null && foodToEatImage != null)
                {
                    foodToEatImage.sprite = throwable.imageSprite;
                }
            }
           
        }
    }


    public void SetDifficulty(GameManagerScript.DifficultyState difficulty)
    {

        switch (difficulty)
        {
            case GameManagerScript.DifficultyState.Easy:
                {
                    amountOfFoods = 1;
                    Debug.Log("Spawned in Easy");

                }
                break;
            case GameManagerScript.DifficultyState.Medium:
                {
                    amountOfFoods = 2;
                    //20% less aggrotime
                    aggrotime = aggrotime-(aggrotime * 0.2f);
                    //increase score by 30%
                    amountOfScore = (int)(amountOfScore + (amountOfScore * 0.3f));
                    Debug.Log("Spawned in Med");

                }
                break;
            case GameManagerScript.DifficultyState.Hard:
                {
                    amountOfFoods = 3;
                    //30% less aggrotime
                    aggrotime = aggrotime - (aggrotime * 0.5f);
                    //increase score by 40%
                    amountOfScore = (int)(amountOfScore + (amountOfScore * 0.4f));

                    Debug.Log("Spawned in Hard");

                }
                break;
        }

        if (GameManagerScript.S.EnemySpawnTotal == 1)
        {
            amountOfFoods = 10;
            Debug.Log("Spawned BOSS");
        }

        //initialize the list and set it.
        types = new List<GameObject>();
        SetRandomFoodType();
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.S.EnemyDMGSound();
            bool matchFound = false;
            //check what hit the enemy and if it matches enemy type 
            //stawberry cake cookie
            Debug.Log("IENEMY: COLLISION DETECTED");
            if (collision.gameObject.GetComponent<ThrowableItem>() != null)
            {
                ThrowableItem throwableItem = collision.gameObject.GetComponent<ThrowableItem>();
                //AudioManager.Instance.PlayHit();

                //search the list of foods the enemy wants to eat.
                foreach (GameObject item in types)
                {
                    if (item.name == throwableItem.name)
                    {
                        health -= 40; //deal the critical health damage
                        aggrotime += 10;
                        Debug.Log("CRIT" + health);
                        matchFound = true;
                    //remove the item
                    RemoveImageFromList(types.IndexOf(item));
                    types.Remove(item);
                    CheckList();
                        break;
                    }

                }

                if (!matchFound)
                {
                    // if not the right order take some health but decrease time till agro 
                    {
                        health -= 10;
                        time += 10;
                    }
                }


            }

    }

    private void RemoveImageFromList(int index)
    {
        Destroy(enemyHorizontalGroup.GetChild(index).gameObject);
    }

    private void CheckList()
    {
        if(types.Count <= 0)
        {
            health = 0;
        }
        return;
    }
}
