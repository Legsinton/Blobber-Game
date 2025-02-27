﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI highScore;
    //Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;
    public TextMeshProUGUI playerHighScoreText;
    public FirebaseScript firebaseScript;
    public AuthManager authManager;

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

        firebaseScript = FirebaseScript.Instance; // Use singleton instead of FindAnyObjectByType
        authManager = AuthManager.Instance;
        if(firebaseScript == null)
        {
            Debug.LogWarning("Did not saave");
        }
        else
        {
       // playerHighScoreText.text = "Your highscore " + firebaseScript.PlayerHighScore.ToString();
        Debug.LogWarning("saved highscore");
        }

        StartCoroutine(StartHighScore());
    
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

    public IEnumerator StartHighScore()
    {
        yield return new WaitForSeconds(0.2f);
        firebaseScript.GetPlayerHighscoreFromFirebase((firebaseHighScore) =>
        {


            Debug.Log("Found Score");
            playerHighScoreText.text =  "Your highscore " + firebaseHighScore.ToString();
            if (authManager.User != null)
            {
                //Access the user's display name (username)
                string userName = authManager.User.DisplayName; // Get the user's display name (username)

                // Ensure the username is not null or empty, fallback to a default name if it is
                userName = string.IsNullOrEmpty(userName) ? "Guest" : userName;

                playerHighScoreText.text = $"{userName}'s highscore: {firebaseHighScore.ToString()}";
                Debug.Log("Found User");

            }
            else
            {
                // Handle case where the user is not logged in
                Debug.Log("Did not Find User");

                playerHighScoreText.text = $"Your highscore: {firebaseHighScore.ToString()}";
            }

        });
    }
}
