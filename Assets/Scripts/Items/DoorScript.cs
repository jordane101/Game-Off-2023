using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (this.name== "ExitDoor")
        {
            //end game or whatever
            SceneManager.LoadScene("EndGame");
        }
        //Do open door stuff
    }
    public void LockedDoorAnimation()
    {
        Debug.Log("Closed door");
        //play locked door sound
    }

}
