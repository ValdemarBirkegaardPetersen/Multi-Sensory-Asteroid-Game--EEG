using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startscreen : MonoBehaviour
{
    private bool eegRunning;

    // TODO:The UDP GameObject must be made DontDestroyOnLoad() so that it persists when changing from start screen to game screen
    public GameObject UDP;
    
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        eegRunning = UDP.GetComponent<udp>().eegActive;
        Debug.Log(eegRunning);

        // if mouse button and eeg signal is active then start game
        if (Input.GetMouseButton(0) && eegRunning)
        {
            SceneManager.LoadScene("Game");
        }


        
    }
}
