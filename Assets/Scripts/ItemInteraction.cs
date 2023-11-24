using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    public PlayerMovement pm;
    [Header("Item Handling")]
    public Transform cameraPos;
    private RaycastHit itemHit;
    public LayerMask whatIsItem;
    public float pickUpTime;
    public float pickUpTimeCounter;
    private bool lookingAtItem;

    public Slider mSlider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckForItem()
    {
        Physics.Raycast(cameraPos, cameraPos.forward,pm.playerHeight);
    }
}
