using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class ItemCombination //this is our itemcombinations list
{
    public int total; //the amount total needed to get gameobject result item
    public GameObject resultItem;
}

public class ItemDrop : MonoBehaviour , IInteractable
{
    public List<ItemCombination> itemCombinationsList; //we make this public so we can edit this within the editor
    private Dictionary<int, GameObject> itemCombinations = new Dictionary<int, GameObject>(); //initialize a new dictionary
    public List<GameObject> RecentlyDroppedItems; //this is just recently dropped items list
    private int MaxItems = 3;

    void Awake()
    {
        RecentlyDroppedItems = new List<GameObject>();
        foreach (var combo in itemCombinationsList) //we need to fill this dictionary
        {
            itemCombinations[combo.total] = combo.resultItem;
        }
    }

    void Start()
    {

    }

    public void CheckCombo()
    {
        //if recently dropped items amount is empty, do nothing
        if (RecentlyDroppedItems.Count == 0) return;

        //multiply the amounts in recentlydroppeditems should be a prime number
        int sum = 0;
        foreach (GameObject item in RecentlyDroppedItems)
        {
            if (sum == 0)
            {
                sum += item.gameObject.GetComponent<InteractableObject>().AmountWorth;
            }
            else
            {
                sum *= item.gameObject.GetComponent<InteractableObject>().AmountWorth;
            }
            Debug.Log("total Amount " + sum);
        }
        //check the sum if it correlates to a result item spit it out
        if (itemCombinations.TryGetValue(sum, out GameObject resultItem))
        {
            if (resultItem != null)
            {
                //if the weapons list does not already contain the weapon
                if (!PlayerScript.S.weapons.Contains(resultItem))
                {
                    //add it
                    PlayerScript.S.weapons.Add(resultItem);
                    //equip the weapon
                    PlayerScript.S.EquipWeapon(resultItem);

                    GameManagerScript.S.UpdateWeaponCyclerUI();

                    if (GameManagerScript.S.currentTutorial == "Craft")
                        GameManagerScript.S.AdvanceNextTutorial();
                }

                //give the players current weapon max ammo.
                Weapon playersWeapon = PlayerScript.S.currentWeapon.GetComponent<Weapon>();
                playersWeapon.ammo = playersWeapon.MaxAmmo;
                GameManagerScript.S.UpdateAmmoGUI(playersWeapon.ammo, playersWeapon.MaxAmmo);

                if (!PlayerScript.S.currentProjectile)
                {
                    PlayerScript.S.SetProjectile();
                }


                //start respawning items
                foreach (GameObject item in RecentlyDroppedItems)
                {
                    InteractableObject interactable = item.GetComponent<InteractableObject>();
                    GameManagerScript.S.StartCoroutine(interactable.RespawnItem());
                }
                //clear the list if we created an object
                RecentlyDroppedItems.Clear();
            }
        }
        else if (RecentlyDroppedItems.Count >= MaxItems)
        {
            //SPIT ITEMS BACK OUT x UNITS AWAY FROM THE OBJECT (MAY NEED TO CHANGE THIS DEPENDING ON WHERE THE "FRONT" of the object is.)
            foreach (GameObject item in RecentlyDroppedItems)
            {
                SpitItemsOut(item);
            }

            RecentlyDroppedItems.Clear();

        }
        //else do nothing debug.log was removed

    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;

        //if the item is there and we are using the interactableobject script and we are NOT holding an item
        if (collision.gameObject != null && collision.gameObject.GetComponent<InteractableObject>() != null && PlayerScript.S.currentItemHolding == null)
        {

            //check if the recentlydropped item is not already in the list (by reference number) so we dont duplicate
            if (!RecentlyDroppedItems.Any(item => ReferenceEquals(item, collidedObject)))
            {
                //add the object
                RecentlyDroppedItems.Add(collidedObject);
                Debug.Log("Added :" + collidedObject);
                collidedObject.SetActive(false);
                CheckCombo();
            }
        }
        else
        {
            Debug.Log("Interactable object is null or currentItemHolding is not null");
        }
    }

    public void Interact(bool val)
    {
        PlayerScript.S.inUI = val;

        if (val)
        {
            //show the mouse and stop the player from looking around
            Cursor.lockState = CursorLockMode.None;
            PlayerScript.S.mouseSpeed = 0.0f;
            //set the box UI
            GameManagerScript.S.BoxUI.SetActive(val);
            //set the current item box
            GameManagerScript.S.SetCurrentItemBox(this);
            //update the items
            GameManagerScript.S.UpdateItems();
            //stop the coroutine
            PlayerScript.S.StopCoroutine(PlayerScript.S.CastRaycast());
            return;
        }
       //relock the cursor to the screen
            Cursor.lockState = CursorLockMode.Locked;
            PlayerScript.S.mouseSpeed = 1000.0f;//could make this better so its a more universal mouse speed
            GameManagerScript.S.BoxUI.SetActive(val);
            GameManagerScript.S.SetCurrentItemBox(null);
           PlayerScript.S.StartCoroutine(PlayerScript.S.CastRaycast());

    }


    public void SpitItemsOut(GameObject item)
    {
        item.SetActive(true);
        item.transform.position = transform.position + Vector3.right * -1.5f;
    }


}
