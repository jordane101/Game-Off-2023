using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionScript : MonoBehaviour
{
    public GameObject container;

    public void PlayPotionSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.potionDrinking,this.transform.position); 
    }
}
