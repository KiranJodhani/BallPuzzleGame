using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void OnClickPlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame");
    }
}
