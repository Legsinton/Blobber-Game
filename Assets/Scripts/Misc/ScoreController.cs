using Firebase.Auth;
using System;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI deathscoreText;
    public TextMeshProUGUI highScore;
    public TextMeshProUGUI newhighScore;
    public GameUI gameUI;
    public PlatformSpawner spawner;
    public FirebaseScript firebaseScript;
    public TMP_Text playerHighScoreText;


    public int score;
    public event Action<int> OnScroreChanged;
    void Start()
    {
        firebaseScript = FindAnyObjectByType<FirebaseScript>();
        score = 0;
        UpdateHighScore();
    }

    public void AddScore(int points)
    {
        score += points;
        scoreText.text = "Score:" + score.ToString();
        OnScroreChanged?.Invoke(score);
        HighScore();
    }

    public void ResetScore()
    {
        score = 0;
        scoreText.text = "Score:" + score.ToString();
    }
    public void DeathScore()
    {

        deathscoreText.text = "Final Score:" + score.ToString();
       // playerHighScoreText.text = "Your highscore " + firebaseScript.playerHighscore.ToString();
        deathscoreText.enabled = true;
    }
    public void PlayerHighscore()
    {
        // First, retrieve the current high score from Firebase
        firebaseScript.GetPlayerHighscoreFromFirebase((firebaseHighScore) =>
        {
            if (score > firebaseHighScore) // Only save if the new score is higher
            {
                Debug.LogWarning("New high score! Saving to Firebase.");
                firebaseScript.SavePlayerHighscore(score);
            }
            else
            {
                Debug.Log("Score is not higher than stored high score.");
            }
        });
    }

    public void HighScore()
    {
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {

            gameUI.NewHighScore();
            newhighScore.text = $"New HighScore:{score}";
            PlayerPrefs.SetInt("HighScore", score);
            highScore.text = $"HighScore:{score}";
            PlayerPrefs.Save(); // Ensure the high score is saved
        }
    }
    public void UpdateHighScore()
    {
        highScore.text = $"HighScore:{PlayerPrefs.GetInt("HighScore", 0)}";
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
           // firebaseScript.SaveHighScoreToFirebase(score);
        }
    }
}