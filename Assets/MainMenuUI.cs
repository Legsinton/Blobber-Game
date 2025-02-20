﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI highScore;
    //Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;
    public TMP_Text playerHighScoreText;
    FirebaseScript firebaseScript;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        highScore.text = $"HighScore:{PlayerPrefs.GetInt("HighScore", 0)}";
    }

    private void Start()
    {
      
        firebaseScript = FindAnyObjectByType<FirebaseScript>();
        playerHighScoreText.text = "Your highscore " + firebaseScript.playerHighscore.ToString();
        Debug.LogWarning("saved highscore");
        
        if(firebaseScript == null)
        {
            Debug.LogWarning("Did not saave");
        }


    }

    //Functions to change the login screen UI
    public void LoginScreen() //Back button
    {
        loginUI.SetActive(true);
        registerUI.SetActive(false);
    }
    public void RegisterScreen() // Regester button
    {
        loginUI.SetActive(false);
        registerUI.SetActive(true);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
