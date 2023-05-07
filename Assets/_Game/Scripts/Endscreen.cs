using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Endscreen : MonoBehaviour
{    
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        // if mouse button and eeg signal is active then start game
        if ((Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            SceneManager.LoadScene("StartScene");
        }


        
    }
}
