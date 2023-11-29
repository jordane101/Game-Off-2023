using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance {get; private set;}
    private EventInstance windEventInstance;
    private EventInstance crowdEventInstance;
    private EventInstance birdsEventInstance;
    private EventInstance ambienceEventInstance;
    private List<EventInstance> eventInstances;
    void Start()
    {
        InitializeAmbience(FMODEvents.instance.ambienceEventReference);
    }
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
    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateEventInstance(ambienceEventReference);
        ambienceEventInstance.start();
        // windEventInstance = CreateEventInstance(windEventReference);
        // crowdEventInstance = CreateEventInstance(crowdEventReference);
        // birdsEventInstance = CreateEventInstance(birdsEventReference);
        // windEventInstance.start();
        // crowdEventInstance.start();
        // birdsEventInstance.start();
    }
    private void SetAmbienceParameter(string parameterName, float parameterValue)
    {
        ambienceEventInstance.setParameterByName(parameterName,parameterValue);
    }
}
