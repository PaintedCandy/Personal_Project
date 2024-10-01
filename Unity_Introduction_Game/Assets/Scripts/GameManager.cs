using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public bool isPaused = false;
    
    public GameObject PauseMenu;
    public PlayerController playerData;

    public Image HealthBar;
    public TextMeshProUGUI clipCounter;
    public TextMeshProUGUI ammoCounter;

    // Start is called before the first frame update
    void Start()
    {
       playerData = GameObject.Find("Player").GetComponent<PlayerController>(); 
    }

    // Update is called once per frame
    void Update()
    {
        HealthBar.fillAmount = Mathf.Clamp(((float)playerData.health / (float)playerData.maxHealth), 0, 1);

        if (playerData.weaponID < 0)
        {
            clipCounter.gameObject.SetActive(false);
            ammoCounter.gameObject.SetActive(false);
        }

        else
        {
            clipCounter.gameObject.SetActive(true);
            clipCounter.text = "Clip: " + playerData.currentClip + "/" + playerData.clipSize;

            ammoCounter.gameObject.SetActive(true);
            ammoCounter.text = "Ammo: " + playerData.currentAmmo;
        }

        if(!isPaused && Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = true;
            
            PauseMenu.SetActive(true);

            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        else if(isPaused && Input.GetKeyDown(KeyCode.Escape))
            Resume();
    }

    public void Resume()
    {
        isPaused = false;

        PauseMenu.SetActive(false);

        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void LoadLevel(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
}
