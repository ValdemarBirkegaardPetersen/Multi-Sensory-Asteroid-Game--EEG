using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startscreen : MonoBehaviour
{
    private bool eegRunning;
    private GameObject UDP;
    public TextMeshProUGUI waitingForSignalText;

    // TODO:The UDP GameObject must be made DontDestroyOnLoad() so that it persists when changing from start screen to game screen    
    void Start()
    {
        UDP = GameObject.FindWithTag("x");
    }

    // Update is called once per frame
    void Update()
    {
        eegRunning = UDP.GetComponent<udp>().eegActive;

        if(eegRunning == true)
        {
            waitingForSignalText.color = new Color(0, 1, 0, 1);
            waitingForSignalText.text = "Signal found! Press 'space' to continue...";
        }
        else
        {
            waitingForSignalText.color = new Color(1, 0, 0, 1);
            waitingForSignalText.text = "Waiting for signal...";
        }

        // if mouse button and eeg signal is active then start game
        if (Input.GetKeyDown(KeyCode.Space) && eegRunning)
        {
            SceneManager.LoadScene("Game");
        }


        
    }
}
