using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject winElements;
    [SerializeField] private GameObject loseElements;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject howToPlay;
    
    private static CanvasManager instance;

    void Awake()
    {
        instance = this;
    }

    public static void ShowWin()
    {
        instance.winElements.SetActive(true);
    }
    
    public static void ShowLose()
    {
        instance.loseElements.SetActive(true);
    }

    public static void loadGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("GameScene");
    }
    
    public static void loadWeirdGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("GameScene_ai");
    }

    public static void loadMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }

    public void swapToMenu()
    {
        settings.SetActive(false);
        howToPlay.SetActive(false);
        mainMenu.SetActive(true);
    }
    
    public void swapToHowToPlay()
    {
        howToPlay.SetActive(true);
        mainMenu.SetActive(false);
    }
    
    public void swapToSettings()
    {
        settings.SetActive(true);
        mainMenu.SetActive(false);
    }

    public static void Quit()
    {
        Application.Quit();
    }
}
