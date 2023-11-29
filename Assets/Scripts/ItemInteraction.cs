using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemInteraction : MonoBehaviour
{

    public PlayerMovement pm;

    public KeyCode pickUpKey = KeyCode.F;

    [Header("Item Handling")]
    public Transform cameraPos;
    private RaycastHit itemHit;
    public LayerMask whatIsItem;
    public float pickUpTime;
    public float pickUpTimeCounter;
    private bool lookingAtItem;
    private GameObject item;
    [Header("Interactable")]
    private bool lookingAtInteractable;
    public LayerMask whatIsInteractable;
    private RaycastHit interactableHit;
    public Slider mSlider;
    public TMP_Text drinkPrompt;
    public TMP_Text interactPrompt;

    public Camera playerCam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lookingAtItem = Physics.Raycast(cameraPos.transform.position, playerCam.transform.forward , out itemHit, pm.scaledPlayerHeight * 1.3f ,whatIsItem);
        lookingAtInteractable = Physics.Raycast(cameraPos.transform.position, playerCam.transform.forward , out interactableHit, pm.scaledPlayerHeight * 1.3f ,whatIsInteractable);
        //Debug.DrawRay(cameraPos.transform.position, playerCam.transform.forward * pm.playerScale, new Color(255,0,0));

        CheckForItem();
        CheckForInteractable();
    }

    void CheckForItem()
    {
        if(lookingAtItem && !Input.GetKey(pickUpKey))
        {
            drinkPrompt.gameObject.SetActive(true);
        }
        else
        {
            drinkPrompt.gameObject.SetActive(false);
        }
        if(Input.GetKey(pickUpKey) && lookingAtItem)
        {
            item = itemHit.collider.gameObject;
            if(pickUpTimeCounter == 0)
            {
                //audioData = itemHit.collider.gameObject.GetComponent<AudioSource>();
                //audioData.Play(0);
                item.GetComponent<PotionScript>().PlayPotionSound();
            }
            pickUpTimeCounter += Time.deltaTime;

            //potion drinking animation
            GameObject fluid = item.GetComponent<PotionScript>().container;
            fluid.GetComponent<Renderer>().material.SetFloat("_Fill", (pickUpTime-pickUpTimeCounter)/pickUpTime);
           
            // update slider 
            mSlider.gameObject.SetActive(true);
            mSlider.normalizedValue = pickUpTimeCounter/pickUpTime;

            PickUpItem();
        }
        else
        {
            mSlider.gameObject.SetActive(false);
            pickUpTimeCounter = 0;
            // if(item != null)
            // {
            //     item.GetComponent<PotionScript>().StopPotionSound();
            // }
        }
    }

    void CheckForInteractable()
    {

        if(lookingAtInteractable)
        {
            //show interact prompt
            interactPrompt.gameObject.SetActive(true);
        }
        else
        {
            interactPrompt.gameObject.SetActive(false);
            //deactivate interact prompt
        }
        if(lookingAtInteractable && Input.GetKey(pickUpKey))
        {
            //interact with item
            if(interactableHit.collider.gameObject.tag == "Door")
            {
                DoorScript ds = interactableHit.collider.gameObject.GetComponent<DoorScript>();
                if(ds.doorScale == pm.playerScale)
                {
                    ds.OpenDoorAnimation();
                }
                else
                {
                    ds.LockedDoorAnimation();
                }
            }
        }
    }
    void PickUpItem()
    {
        if(pickUpTimeCounter >= pickUpTime)
        {
            //item type checks
            if(itemHit.collider.gameObject.tag == "Shrink Potion")
            {
                //subtract uniform scale
                pm.ScalePlayer(pm.playerScale-1);
            }
            if(itemHit.collider.gameObject.tag == "Grow Potion")
            {
                //add uniform scale
                pm.ScalePlayer(pm.playerScale+1);
            }
            // if(audioData.isPlaying)
            // {
            //     audioData.Stop();
            // }
            Destroy(itemHit.collider.gameObject);
        }
    }
}
