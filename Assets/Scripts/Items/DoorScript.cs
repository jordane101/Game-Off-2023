using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] public int doorScale;
    [SerializeField] private Animator anim;
    private GameObject doorHinge;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(doorScale,doorScale,doorScale);
        doorHinge = GameObject.Find("Pivot");
    }
    public void OpenDoorAnimation()
    {
        Debug.Log("Open door");
        transform.RotateAround(doorHinge.transform.position, Vector3.up, 20 * Time.deltaTime);
    }
    public void LockedDoorAnimation()
    {
        Debug.Log("Closed door");
        anim.Play("LockedDoor");
    }
}
