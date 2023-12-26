using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool OptionWindowActive = false;
    public GameObject MenuCanvas;
    public GameObject backgroundCanvas;
    public GameObject optionMenuCanvas;

    void Start()
    {
        MenuCanvas.SetActive(false);
        optionMenuCanvas.SetActive(false);
        backgroundCanvas.SetActive(false);
    }

    void Update()
    {

        if (OptionWindowActive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                optionMenuCanvas.SetActive(false);
                MenuCanvas.SetActive(true);     
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();

                }

            }
        }
    }
    #region 게임 정지


    public void Resume()
    {
        MenuCanvas.SetActive(false);
        backgroundCanvas.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        MenuCanvas.SetActive(true);
        backgroundCanvas.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void OptionMenu()
    {
        MenuCanvas.SetActive(false);
        optionMenuCanvas.SetActive(true);
        OptionWindowActive = true;
    }

    public void QuitOptionWindow()
    {
        OptionWindowActive = false;
        optionMenuCanvas.SetActive(false);
        MenuCanvas.SetActive(true);
    }

    public void DoGam()
    {
        Debug.Log("제발 되어라");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("StartScene");
    }
    #endregion
}
