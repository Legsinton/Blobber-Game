using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class BouncePlatform : MonoBehaviour
{
    private bool hasBounced = false;
    readonly int score = 1;
    public ScoreController scoreController;
    public Movement movement;
    private HashSet<GameObject> scoredObjects;
    // Start is called before the first frame update
    void Start()
    {
        scoreController = FindAnyObjectByType<ScoreController>();
        movement = FindAnyObjectByType<Movement>();
        scoredObjects = new HashSet<GameObject>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 collisionNormal = collision.contacts[0].normal;
            Debug.Log("Collision Normal: " + collisionNormal);
            Debug.Log("Hello");
            if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
            {
                Debug.Log("Another Hello");
                if (!hasBounced)
                {
                    Debug.Log("Yet another Hello");
                    if (collisionNormal.x > 0.1f) // RightSide of the platform
                    {    
                        hasBounced = true;
                        Bounce(-collision.relativeVelocity.x, -collision.relativeVelocity.y);
                        Invoke(nameof(ResetBounce), 0.5f);
                        Debug.Log("RightBounce");


                        if (!scoredObjects.Contains(collision.gameObject))
                        {
                            scoredObjects.Add(collision.gameObject);
                            scoreController.AddScore(score);
                        }
                    }
                    else if (collisionNormal.x < -0.1f) // Leftside of the platform
                    {
                        hasBounced = true;
                        Bounce(-collision.relativeVelocity.x, -collision.relativeVelocity.y);
                        Invoke(nameof(ResetBounce), 0.1f);
                        if (!scoredObjects.Contains(collision.gameObject))
                        {
                            scoredObjects.Add(collision.gameObject);
                            scoreController.AddScore(score);
                        }
                        Debug.Log("LeftBounce");
                    }

                }
            }
            else if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
            {
                if (collisionNormal.y > 0.1f) // Top of the platform
                {
                    hasBounced = true;
                    BounceSmall(-collision.relativeVelocity.x, -collision.relativeVelocity.y);
                    Invoke(nameof(ResetBounce), 0.1f);

                    Debug.Log("TopBounce");
                }
                else if (collisionNormal.y < -0.1f) // Bottom of the platform
                {
                    hasBounced = true;
                    BounceSmall(-collision.relativeVelocity.x, -collision.relativeVelocity.y);
                    Invoke(nameof(ResetBounce), 0.1f);

                    Debug.Log("Bottombounce");
                }
            }
        }
    }

    private void ResetBounce()
    {
        hasBounced = false;
    }

    public void Bounce(float collisionVelocityX, float CollisionVelocityY)
    {
        SoundFXManager.Instance.PlaySoundFX(SoundType.Boing);
        float bounceScale = 2f;
        float maxBounceforce = 8;
        float upBounceforce = Mathf.Abs(CollisionVelocityY) * bounceScale;
        upBounceforce = Mathf.Clamp(upBounceforce, 0f, maxBounceforce);
        movement.rb.AddForce(new Vector2(collisionVelocityX * 1f, upBounceforce), ForceMode2D.Impulse);
        Debug.Log(collisionVelocityX);
    }
    public void BounceSmall(float collisionVelocityX, float collisionVelocityY)
    {
        SoundFXManager.Instance.PlaySoundFX(SoundType.Boing);
        float bounceScale = 2f;
        float maxBounceforce = 5;
        float upBounceforce = Mathf.Abs(collisionVelocityY) * bounceScale;
        upBounceforce = Mathf.Clamp(upBounceforce, 0f, maxBounceforce);

        movement.rb.AddForce(new Vector2(collisionVelocityX + 2f, upBounceforce), ForceMode2D.Impulse);
        Debug.Log(collisionVelocityX);

    }
}
