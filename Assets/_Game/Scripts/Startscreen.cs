using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startscreen : MonoBehaviour
{
    private bool eegRunning;
    private GameObject UDP;

    // TODO:The UDP GameObject must be made DontDestroyOnLoad() so that it persists when changing from start screen to game screen    
    void Start()
    {
        UDP = GameObject.FindWithTag("x");
    }

    // Update is called once per frame
    void Update()
    {
        eegRunning = UDP.GetComponent<udp>().eegActive;

        // if mouse button and eeg signal is active then start game
        if ((Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Space)) && eegRunning)
        {
            SceneManager.LoadScene("Game");
        }


        
    }
}
