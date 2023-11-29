using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
public class PotionScript : MonoBehaviour
{
    public GameObject container;
    private EventInstance potionSound;
    void Awake()
    {
        //potionSound = AudioManager.instance.CreateEventInstance(FMODEvents.instance.potionDrinking); 
    }
    public void PlayPotionSound()
    {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.potionDrinking,this.transform.position);
            // PLAYBACK_STATE playbackState;
            // potionSound.getPlaybackState(out playbackState);
            // if(playbackState.Equals(PLAYBACK_STATE.STOPPED))
            // {
            //     potionSound.start();
            // } 
    }
    public void StopPotionSound()
    {
        potionSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
