using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance {get; private set;}
    private List<EventInstance> eventInstances;
    private void Awake() 
    {
        if (instance != null)
        {
            Debug.LogError($"Cannot have more than one AudioManager instance in a scene");
        }
        instance = this;
        eventInstances = new List<EventInstance>();
    }
    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound,worldPos);
    }
    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }
    private void CleanUp()
    {
        foreach(EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }
    private void OnDestroy()
    {
        CleanUp();
    }
}
