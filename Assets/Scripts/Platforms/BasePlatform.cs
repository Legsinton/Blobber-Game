using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlatform : MonoBehaviour
{
    protected bool hasBounced;
    protected bool top;
    protected bool breaking;
    protected float spriteOffset;
    protected Vector2 collisionNormal;
    protected Vector2 positionOffset;
    protected bool isOnPlatform = false;
    [SerializeField] protected Transform platformTransform;


    public bool onStickyPlatform;
    public float SpriteOffset { get { return spriteOffset; } set { spriteOffset = value; } }
    protected Rigidbody2D rb;
    protected HashSet<GameObject> scoredObjects;
    readonly int score = 1;
    ScoreController scoreController;
    protected Rigidbody2D playerRB;
    protected Vector2 falling = new Vector2(0, -0.9f);
    protected SpriteRenderer spriteRenderer;


    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scoreController = FindAnyObjectByType<ScoreController>();
        scoredObjects = new HashSet<GameObject>();
    }

    protected void FixedUpdate()
    {
        if (isOnPlatform && platformTransform != null)
        {
            playerRB.transform.position = (Vector3)platformTransform.position + (Vector3)positionOffset;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisionNormal = collision.contacts[0].normal;
        playerRB = collision.gameObject.GetComponent<Rigidbody2D>();
        if (collision.transform.CompareTag("Player"))
        {
            HandleCollision(collision);

            if (!scoredObjects.Contains(collision.gameObject) && !gameObject.CompareTag("Start"))
            {
                scoreController.AddScore(score);
                scoredObjects.Add(collision.gameObject);

            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        playerRB = collision.gameObject.GetComponent<Rigidbody2D>();

        HandleCollisionExit(collision);

    }

    protected virtual void HandleCollision(Collision2D collision)
    {

    }

    protected virtual void HandleCollisionExit(Collision2D collision)
    {

    }

    protected void Bounce(float collisionVelocityX, float CollisionVelocityY)
    {
        SoundFXManager.Instance.PlaySoundFX(SoundType.Boing);
        float bounceScale = 2f;
        float maxBounceforce = 8;
        float upBounceforce = Mathf.Abs(CollisionVelocityY) * bounceScale;
        upBounceforce = Mathf.Clamp(upBounceforce, 0f, maxBounceforce);
        playerRB.AddForce(new Vector2(collisionVelocityX * 1f, upBounceforce), ForceMode2D.Impulse);
        Debug.Log(collisionVelocityX);
    }
    protected void BounceSmall(float collisionVelocityX, float collisionVelocityY)
    {
        SoundFXManager.Instance.PlaySoundFX(SoundType.Boing);
        float bounceScale = 2f;
        float maxBounceforce = 5;
        float upBounceforce = Mathf.Abs(collisionVelocityY) * bounceScale;
        upBounceforce = Mathf.Clamp(upBounceforce, 0f, maxBounceforce);

        playerRB.AddForce(new Vector2(collisionVelocityX + 2f, upBounceforce), ForceMode2D.Impulse);
        Debug.Log(collisionVelocityX);

    }

    protected IEnumerator FallDown()
    {
        yield return new WaitForSeconds(0.2f);
        playerRB.velocity = falling;
    }

    protected void ResetBounce()
    {
        hasBounced = false;
    }

    protected void AttachPlayer()
    {
        Debug.Log("Attached");
        Debug.Log("Attached to: " + (platformTransform != null ? platformTransform.name : "NULL"));
        SoundFXManager.Instance.PlaySoundFX(SoundType.Smack);
        isOnPlatform = true;
        Debug.Log("isOnPlatform set to TRUE - Player attached to platform!");
        positionOffset = playerRB.transform.position - platformTransform.position; // Calculate offset
        playerRB.gravityScale = 0f; // Disable gravity while on the platform
        playerRB.velocity = Vector2.zero; // Stop velocity to prevent sliding
    }
    public void DetachPlayer()
    {
        if (!isOnPlatform)
        {
        platformTransform.GetComponent<Rigidbody2D>().simulated = false;
        isOnPlatform = false;
            platformTransform = null;
            positionOffset = Vector2.zero; // Reset offset
        transform.parent = null;
        Debug.Log("isOnPlatform set to FALSE - Player detached!");

        Debug.LogWarning("Detach Exit");

        }

        
    }
}
