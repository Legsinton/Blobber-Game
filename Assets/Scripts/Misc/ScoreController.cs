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
        firebaseScript = FirebaseScript.Instance; // Use singleton instead of FindAnyObjectByType
        score = 0;
        if (firebaseScript == null)
        {
            Debug.LogError("FirebaseScript not found! Ensure it's present in the scene.");
        }
       
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
        highScore.text = $"HighScore:{PlayerPrefs.GetInt("HighScore", 0)}";
        deathscoreText.enabled = true;
    }
    public void PlayerHighscore()
    {
        // First, retrieve the current high score from Firebase

        // value saved properly, Invoke.ScoreUpdated(score)
        

        firebaseScript.GetPlayerHighscoreFromFirebase((firebaseHighScore) =>
        {
            Debug.Log($"Retrieved high score from Firebase: {firebaseHighScore}");
            if (score > firebaseHighScore) // Only save if the new score is higher
            {
                Debug.LogWarning("New high score! Saving to Firebase.");
                firebaseScript.SavePlayerHighscore(score);
                playerHighScoreText.text = "Your highscore " + score.ToString();

            }
            else
            {
                Debug.Log("Score is not higher than stored high score.");
                playerHighScoreText.text = "Your highscore " + firebaseHighScore.ToString();

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
            UpdateHighScore();
        }
    }
    public void UpdateHighScore()
    {
        highScore.text = $"HighScore:{PlayerPrefs.GetInt("HighScore", 0)}" ;
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
           firebaseScript.SaveHighScoreToFirebase(PlayerPrefs.GetInt("HighScore", 0));
        }
    }
}