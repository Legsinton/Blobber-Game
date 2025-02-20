using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class CoinScript : MonoBehaviour
{
    ScoreController scoreController;
    private readonly int score = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (scoreController == null)
        {
            scoreController = FindObjectOfType<ScoreController>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            scoreController.AddScore(score);
            Destroy(gameObject);
            SoundFXManager.Instance.PlaySoundFX(SoundType.Coin);
        }
    }
}
