using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] GameObject gameOver;
    [SerializeField] GameObject[] backgrounds;
    [SerializeField] GameObject howToPlay;
    [SerializeField] GameObject howToJump;
    [SerializeField] ScoreController scoreController;
    [SerializeField] Canvas canvas;
    Movement player;
    private const float PLAyer_DISTANCE_LEVEL_PART = 70F;
    private const float PLAyer_DISTANCE_LEVEL_PARTJump = 10F;
    private Vector3 startPosition;
    [SerializeField] Transform levelpart_Start;

    void Start()
    {
        player = FindAnyObjectByType<Movement>();
        startPosition = levelpart_Start.Find("StartPlatform").position;
        gameOver.SetActive(false);
        scoreController = FindAnyObjectByType<ScoreController>();
    }

    private void LateUpdate()
    {
        if (Vector3.Distance(player.transform.position, startPosition) > PLAyer_DISTANCE_LEVEL_PART)
        {
            // Loop through each background and deactivate it
            foreach (GameObject bg in backgrounds)
            {
                bg.SetActive(false);
            }
            howToPlay.SetActive(false);
            howToJump.SetActive(false);
            canvas.GetComponent<Canvas>().enabled = false;
        }

        if (Vector3.Distance(player.transform.position, howToJump.transform.position) < PLAyer_DISTANCE_LEVEL_PARTJump)
        {
            howToJump.SetActive(true);
        }
    }
    public void NewHighScore()
    {
        scoreController.newhighScore.enabled = true;
        scoreController.highScore.enabled = false;
        scoreController.scoreText.enabled = false;
    }

    public void GameOver()
    {
        gameOver.SetActive(true);
        scoreController.highScore.enabled = true;
        scoreController.scoreText.enabled = false;
        scoreController.newhighScore.enabled = false;
        scoreController.deathscoreText.enabled = true;
        scoreController.DeathScore();
        scoreController.PlayerHighscore();

    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Main_Menu");
    }
}

