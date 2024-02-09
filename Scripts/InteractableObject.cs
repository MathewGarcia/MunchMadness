using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InteractableObject : MonoBehaviour, IInteractable
{
    bool isPickedUp = false;
    public int AmountWorth;
    private MeshRenderer meshRenderer;
    private Color originalColor;
    public Color highlightColor = Color.yellow;
    public Sprite imageSprite;
    public Vector3 ogPosition;
    public float respawnTime;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
        ogPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //hold the item 5 units out in front of the player.
        if (isPickedUp)
        {
            transform.position = PlayerScript.S.cameraTransform.position + PlayerScript.S.cameraTransform.forward * 3;
        }
    }


    public void Highlight()
    {
        meshRenderer.material.color = highlightColor;
    }

    public void RemoveHighlight()
    {
        meshRenderer.material.color = originalColor;
    }

    public void Interact(bool val)
    {
        isPickedUp = val;
        PlayerScript.S.itemToPickUp = null;
        //set is raycasting to false because we dont want to spam raycasting when we already have an item in our hand.
        PlayerScript.S.isRaycastingActive = false;
        PlayerScript.S.currentItemHolding = gameObject;
        RemoveHighlight();

        if (!isPickedUp)
        {
            //drop the item and reset the raycast ienumerator
            PlayerScript.S.currentItemHolding = null;
            PlayerScript.S.isRaycastingActive = true;
            PlayerScript.S.StartCoroutine(PlayerScript.S.CastRaycast());
            return;
        }
    }
    //sort of like object pooling(?) just set the objects location back to the original location and activate it again after x seconds
    public IEnumerator RespawnItem()
    {
        yield return new WaitForSeconds(respawnTime);
        transform.position = ogPosition;
        gameObject.SetActive(true);
    }
}
