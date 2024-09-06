using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool pauseMenuIsOpen = false;

    public GameObject pauseMenu;

    void Start()
    {
        pauseMenuIsOpen = false;
        Debug.Log("Pause menu is initially closed.");
    }

    public bool GetPauseMenuIsOpen()
    {
        return pauseMenuIsOpen;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (pauseMenuIsOpen)
            {
                pauseMenu.gameObject.SetActive(false);
                pauseMenuIsOpen = false;
                Debug.Log("Pause menu closed.");
            }
            else
            {
                pauseMenu.gameObject.SetActive(true);
                pauseMenuIsOpen = true;
                Debug.Log("Pause menu opened.");
            }
        }
    }
}

