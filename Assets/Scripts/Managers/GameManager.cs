using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static float gameTime;
    static GameManager instance;
    private void Awake() 
    {
        if (instance != null)
        {
            Debug.LogError($"Cannot have more than one GameManager instance in a scene");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "MainScene")
        {
            
            //keep track of delta time
            gameTime += Time.deltaTime;
        }
        if(SceneManager.GetActiveScene().name == "EndGame")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
