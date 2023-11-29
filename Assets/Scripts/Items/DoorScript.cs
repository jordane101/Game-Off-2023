using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] public int doorScale;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(doorScale,doorScale,doorScale);
    }
    public void OpenDoorAnimation()
    {
        Debug.Log("Open door");
        //Do open door stuff
    }
    public void LockedDoorAnimation()
    {
        Debug.Log("Closed door");
        //play locked door sound
    }
}
