using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "EndCard"){
            UpdateEndText();
        }
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void BackToMain()
    {
        SceneManager.LoadScene("MainMenu");
        
    }
    public void UpdateEndText()
    {

        gameObject.GetComponent<TMP_Text>().SetText("Merry Christmas! You made it out in:" + GameManager.gameTime.ToString());
    }
}
