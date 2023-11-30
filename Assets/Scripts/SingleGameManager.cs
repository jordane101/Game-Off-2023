using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGameManager : MonoBehaviour
{
    public static SingleGameManager instance;
    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
    }
}
