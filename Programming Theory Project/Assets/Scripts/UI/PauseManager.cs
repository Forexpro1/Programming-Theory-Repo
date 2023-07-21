using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// adds functions to the pause menu mini panel

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    private Board board;
    public bool paused = false;
    public Image SoundButtonImage;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    private SoundManager soundManager;
    
    // Start is called before the first frame update
    void Start()
    {
        pausePanel.SetActive(false);
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
        soundManager = FindObjectOfType<SoundManager>();
        
        // In Player Prefs, the "Sound" key is for sound
        // If sound == 0, then mute, if sound == 1, then Unmute

        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                SoundButtonImage.sprite = musicOffSprite;
            }
            else
            {
                SoundButtonImage.sprite = musicOnSprite;
            }
        }
        else
        {
            SoundButtonImage.sprite = musicOnSprite;
        }
        
    }

    public void SoundButton()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                SoundButtonImage.sprite = musicOnSprite;
                PlayerPrefs.SetInt("Sound", 1);
                soundManager.AdjustVolume();
            }
            else
            {
                SoundButtonImage.sprite = musicOffSprite;
                PlayerPrefs.SetInt("Sound", 0);
                soundManager.AdjustVolume();
            }
        }
        else
        {
            SoundButtonImage.sprite = musicOnSprite;
            PlayerPrefs.SetInt("Sound", 1);
            soundManager.AdjustVolume();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (paused && !pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(true);
            board.currentState = GameState.pause;
        }
        if (!paused && pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            board.currentState = GameState.move;
        }
    }

    public void PauseGame()
    {
        paused = !paused;
    }

    public void ExitLevel()
    {
        SceneManager.LoadScene("Level Select");
    }
}
