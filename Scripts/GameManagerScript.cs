using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;


public class GameManagerScript : Singleton<GameManagerScript>
{
    public TextMeshProUGUI ammoGUI;
    public TextMeshProUGUI pressToGUI; //press whatever to accessbox
    public Button ExitButton;
    public GameObject itemDisplayPrefab;
    public GameObject BoxUI;
    private ItemDrop currentItemBox;
    public Transform horizontalGroup;
    public GameObject ProjectilesPool;
    public int level;               //The current level
    public int levelMax;            //The number of levels
    public GameObject weaponCycler;
    public Transform weaponHorizontalGroupTransform;
    //damage flash
    public Image damageImage;
    //time for timer
    public float time;

    //points in cur level 
    public int levelpoints;
    public int gamepoints;

    public int EnemySpawnTotal;
    public int CurEnemys;

    public int PlayerHealth; 

    //EVENT FOR OBSERVER PATTERN TO NOTIFY DIFFICULTY CHANGE
    public static event Action<DifficultyState> OnDifficultyChange;

    //current difficulty 
    public DifficultyState currentState;

    //object pooling
    public GameObject[] ProjectilesToSpawn;
    public List<GameObject>ProjectileObjectPool;
    public int amountToSpawn;

    //TUTORIAL
    private List<string> tutorials = new List<string> { "Move","Sprint", "PickUp","Drop" , "Craft","ItemBox","ItemBox1","Fire"}; //add more if you have any other ideas
    public string currentTutorial;

    //keeping track of possible food types:
    public List<GameObject> foodTypes;

    //score UI
    public TextMeshProUGUI scoreUI;

   public float medTime;
    public float hardTime;

    public enum DifficultyState
    {
        Easy,
        Medium,
        Hard
    }

    public IEnumerator Timer()
    {

        yield return new WaitForSeconds(time);
    }

 
    // Start is called before the first frame update
    void Start()
    {
        PlayerHealth = 500;
        level = 0;

        SetDifficulty(DifficultyState.Easy);

        BoxUI.SetActive(false);
        ExitButton.onClick.AddListener(ExitButtonClicked);

        ProjectileObjectPool = new List<GameObject>();

        damageImage.enabled = false;

        foreach (GameObject projectile in ProjectilesToSpawn)
        {
            for (int i = 0; i < amountToSpawn; i++)
            {
                GameObject spawnedObject =Instantiate(projectile, ProjectilesPool.transform);
                spawnedObject.name = projectile.name;
                ProjectileObjectPool.Add(spawnedObject);
                spawnedObject.SetActive(false);

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        //gameTimer.text = string.Format("{0:00}:{1:00} Multiplier: x{2}", minutes, seconds, currentMultiplier);

        if ((time >= hardTime) && (currentState == DifficultyState.Medium))
        {

            SetDifficulty(DifficultyState.Hard);
        }
        else if ((time >= medTime) && (currentState != DifficultyState.Hard) && (currentState != DifficultyState.Medium))
        {
            SetDifficulty(DifficultyState.Medium);
        }


        if((EnemySpawnTotal == 0 ) && (CurEnemys == 0))
        {
        gamepoints += levelpoints;
        levelpoints = 0; 
        level++;
        time = 0;
            EnemySpawnTotal = EnemySpawnTotal + level * 2; 
        }

        if(PlayerHealth <= 0)
        {
            SceneManager.LoadScene(3);
        }


    }

    //updating the current difficulty 

    public void SetDifficulty(DifficultyState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            AudioManager.S.LevelUp();
            OnDifficultyChange?.Invoke(newState);
        }
    }


    public void UpdateAmmoGUI(int currentAmmo, int maxAmmo)
    {
        ammoGUI.text = $"Ammo: {currentAmmo}/{maxAmmo}";
    }

    public void UpdatePressToGUI(string newText)
    {
        pressToGUI.text = newText;
    }
    public void ExitButtonClicked()
    {
        foreach(GameObject item in currentItemBox.RecentlyDroppedItems)
        {
           currentItemBox.SpitItemsOut(item);
        }
        currentItemBox.RecentlyDroppedItems.Clear();
        BoxUI.SetActive(false);
    }

    public void SetCurrentItemBox(ItemDrop itemBox)
    {
        currentItemBox = itemBox;
    }

    public void UpdateItems()
    {
        //works the same as update weapon cycler ui
        foreach (Transform child in horizontalGroup)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("Adding new items to UI. Item count: " + currentItemBox.RecentlyDroppedItems.Count);

        foreach (GameObject item in currentItemBox.RecentlyDroppedItems)
        {
            GameObject imageObject = Instantiate(itemDisplayPrefab, horizontalGroup);
            Image itemImage = imageObject.GetComponent<Image>();
            InteractableObject interactableObject = item.GetComponent<InteractableObject>();

            if (interactableObject != null && itemImage != null)
            {
                itemImage.sprite = interactableObject.imageSprite;
                Debug.Log("Item sprite set for: " + item.name);
            }
            else
            {
                Debug.Log("InteractableObject or Image component is null for item: " + item.name);
            }
        }
    }
    //PROJECTILE OBJECT POOLING
    public GameObject GetProjectileFromPool(GameObject projectilePrefab)
    {
        foreach (var projectile in ProjectileObjectPool)
        {
            if (!projectile.activeInHierarchy && projectile.name.Equals(projectilePrefab.name))
            {
                return projectile;
            }
        }
        //we want to return null because we should not be able to fire if we have no ammo.
        return null;
    }

    //deactivate the projectile. RB.iskinematic  = true happens in the projectile itself, this is CRUCIAL or it wont work...
    public void DeactivateProjectile(GameObject projectile)
    {
        projectile.transform.SetParent(ProjectilesPool.transform);
        projectile.SetActive(false);
    }

    public void UpdateWeaponCyclerUI()
    {
        //if there are children attached to our horizontal group transform get rid of them
        foreach (Transform child in weaponHorizontalGroupTransform)
        {
            Destroy(child.gameObject);
        }
        //for each weapon in our weapons list in our player
        foreach (GameObject weapon in PlayerScript.S.weapons)
        {
            //instantiate a new image and change the image to the image we have on our projectile
            GameObject imageObject = Instantiate(itemDisplayPrefab, weaponHorizontalGroupTransform);
            Image weaponImage = imageObject.GetComponent<Image>();
            GameObject weaponProjectile = weapon.GetComponent<Weapon>().currentItem;
            ThrowableItem weaponProjectileScript = weaponProjectile.GetComponent<ThrowableItem>();
            if(weaponProjectileScript != null && weaponImage != null)
            {
                weaponImage.sprite = weaponProjectileScript.imageSprite;
                //if our weapon is equipped (weapon == currentweapon) then we want to highlight it to yellow, else change it white
                //this is because if it was previously equipped it should be reset.
                if(weapon == PlayerScript.S.currentWeapon)
                {
                    weaponImage.color = Color.yellow;
                }
                else
                {
                    weaponImage.color = Color.white;
                }

            }

          
        }
    }

    public void SetTutorial(string key)
    {
        //switch statement to update the GUI
        currentTutorial = key;
        switch (key)
        {
            case "Move":
                UpdatePressToGUI("Press W A S D to move");
                break;

            case "Sprint":
                UpdatePressToGUI("Press Left Shift to Sprint");
                break;

            case "PickUp":
                UpdatePressToGUI("To pick up an item set your crosshair on a craftable item and left click. (strawberry & bread)");
                break;

            case "Drop":
                UpdatePressToGUI("Press left click again to drop the item");
                break;

            case "Craft":
                UpdatePressToGUI("Grab an item and set it in/on the oven. Grab a bread and strawberry to create a throwable cake");
                break;

            case "ItemBox":
                UpdatePressToGUI("You can access the microwave by going up to it and left clicking");
                break;

            case "ItemBox1":
                    UpdatePressToGUI("If you mess up an combination it will either spit the food back out or if you find out ahead of time, you can enter the microwave and select Empty&Exit");
                break;
            case "Fire":
                UpdatePressToGUI("Now that you have crafted an item, press left click to fire. If you throw the correct food at enemies and complete the combination. They will leave faster!");
                break;

            default:
                UpdatePressToGUI("");
                break;
                

        }
    }
    public void AdvanceNextTutorial()
    {
        //get the current index of the current tutorial
     int currentIndex = tutorials.IndexOf(currentTutorial);
        //check if we are not out of bounds
        if (currentIndex != -1 && currentIndex < tutorials.Count - 1)
        {
            //advance to the next tutorial
            SetTutorial(tutorials[currentIndex + 1]);
        }
        else
        {
            // Tutorials are finished.
            SetTutorial("");
        }
    }
    public IEnumerator DamageFlash()
    {
        damageImage.enabled = true;
        yield return new WaitForSeconds(0.1f);
        damageImage.enabled = false;
    }

    public void UpdateScoreGUI()
    {
        scoreUI.text = "Score: " + levelpoints.ToString();
    }
}
