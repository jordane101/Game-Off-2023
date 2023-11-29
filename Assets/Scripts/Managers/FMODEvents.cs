using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Ambience")]
    [field:SerializeField] public EventReference windEventReference;
    [field:SerializeField] public EventReference crowdEventReference;
    [field:SerializeField] public EventReference birdsEventReference;
    [field:SerializeField] public EventReference ambienceEventReference;
    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerFootsteps { get; private set; } 
    [field: Header("Potion SFX")]
    [field: SerializeField] public EventReference potionDrinking { get; private set; }
    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError($"Multiple FMODEvents instances found.");
        }
        instance = this;

    }
}
