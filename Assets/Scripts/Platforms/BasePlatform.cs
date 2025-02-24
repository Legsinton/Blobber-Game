using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;

public class BasePlatform : MonoBehaviour
{
    protected bool hasBounced;
    protected bool top;
    protected bool breaking;
    protected float spriteOffset;
    protected Vector2 collisionNormal;
    protected Vector2 positionOffset;
    protected bool isOnPlatform = false;
    protected float normalGravityScale = 1.2f;
    protected bool onPlat = false;
    protected float platformWidth;
    protected float playerWidth;
    protected bool onStickyPlatform;
    protected Rigidbody2D playerRB;
    protected Vector2 falling = new Vector2(0, -0.9f);
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb;
    protected HashSet<GameObject> scoredObjects;
    [SerializeField] protected Transform platformTransform;

    public bool OnStickyPlatform { get { return onStickyPlatform; } set { onStickyPlatform = value; } }
    public float SpriteOffset { get { return spriteOffset; } set { spriteOffset = value; } }
    public Movement movement;

    readonly int score = 1;
    ScoreController scoreController;

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scoreController = FindAnyObjectByType<ScoreController>();
        scoredObjects = new HashSet<GameObject>();
        movement = FindAnyObjectByType<Movement>();

    }

    protected void FixedUpdate()
    {
        /*if (isOnPlatform && platformTransform != null)
        {
            playerRB.transform.position = (Vector3)platformTransform.position + (Vector3)positionOffset;
        }*/
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisionNormal = collision.contacts[0].normal;
        playerRB = collision.gameObject.GetComponent<Rigidbody2D>();
        if (collision.transform.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent(out Movement comp))
            {
                comp.basePlatform = this;
            }
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

        if (collision.gameObject.TryGetComponent(out Movement comp))
        {
            comp.basePlatform = null;
        }

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
        playerRB.AddForce(new Vector2(collisionVelocityX * 1.1f, upBounceforce), ForceMode2D.Impulse);
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

    protected IEnumerator SnapAndFallCoroutine(PlatformType platformType)
    {
        yield return new WaitForSeconds(0.1f); // Brief delay for stability

        switch (platformType)
        {
            case PlatformType.Sticky:
                if (platformTransform != null)
                {
                    float targetX;
                    if (playerRB.transform.position.x < platformTransform.transform.position.x)
                    {
                        // Snap to the left side
                        spriteOffset = 90;
                        targetX = platformTransform.transform.position.x - (platformWidth / 2) - (playerWidth / 2) - 0.1f;
                        Debug.Log("Falling SnaperLeft");
                    }
                    else
                    {
                        // Snap to the right side
                        targetX = platformTransform.transform.position.x + (platformWidth / 2) + (playerWidth / 2) + 0.1f;
                        spriteOffset = -90;
                        Debug.Log("Falling SnaperRight");
                    }

                    playerRB.MovePosition(new Vector2(targetX, playerRB.position.y));

                    yield return new WaitForSeconds(0.2f); // Short cling effect before falling

                    // Apply downward force to start falling
                    playerRB.gravityScale = 0;

                    playerRB.velocity = falling;

                    SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                    yield return new WaitForSeconds(1);

                    top = false;
                    Debug.Log("Player is sliding down after snapping.");

                    yield return new WaitForSeconds(6);
                    if (onPlat != false)
                    {
                        Debug.Log("normal gravity");
                        playerRB.gravityScale = normalGravityScale;

                    }
                }
                break;

            case PlatformType.Moving:
                if (platformTransform != null)
                {
                    float targetX;
                    if (playerRB.transform.position.x < platformTransform.transform.position.x)
                    {
                        // Snap to the left side
                        targetX = platformTransform.transform.position.x - (platformWidth / 2) + (playerWidth / 2) - 0.1f;
                        spriteOffset = -90; // Rotate for left side
                        Debug.Log("Falling MovingLeft");
                    }
                    else
                    {
                        // Snap to the right side
                        targetX = platformTransform.transform.position.x + (platformWidth / 2) + (playerWidth / 2) + 0.1f;
                        spriteOffset = 90; // Rotate for right side
                        Debug.Log("Falling MovingRight");
                    }

                    playerRB.MovePosition(new Vector2(targetX, rb.position.y));

                    yield return new WaitForSeconds(0.2f); // Short cling effect before falling
                }
                break;

            case PlatformType.Spiky:
                if (platformTransform != null)
                {
                    float targetX;
                    if (playerRB.transform.position.x < platformTransform.transform.position.x)
                    {
                        // Snap to the left side
                        targetX = platformTransform.transform.position.x + (platformWidth / 2) + (playerWidth / 2) + 0.1f;
                        spriteOffset = -90; // Rotate for left side
                        Debug.Log("Falling SpikyLeft");
                    }
                    else
                    {
                        // Snap to the right side
                        targetX = platformTransform.transform.position.x + (platformWidth / 2) + (playerWidth / 2) + 0.1f;
                        spriteOffset = -90; // Rotate for right side
                        Debug.Log("Falling SpikyRight");
                    }


                    playerRB.MovePosition(new Vector2(targetX, playerRB.position.y));

                    yield return new WaitForSeconds(0.2f); // Short cling effect before falling

                    // Apply gravity + downward force to start falling
                    playerRB.gravityScale = 0;

                    playerRB.velocity = falling;


                    SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                    yield return new WaitForSeconds(1);

                    top = false;
                    Debug.Log("Player is sliding down after snapping.");

                    yield return new WaitForSeconds(6);
                    Debug.Log("normal gravity");
                    if (onPlat != false)
                    {
                        rb.gravityScale = normalGravityScale;

                    }
                }
                break;

            case PlatformType.SpikyFlip:
                if (platformTransform != null)
                {
                    float targetX;
                    if (playerRB.transform.position.x < platformTransform.transform.position.x)
                    {
                        // Snap to the left side
                        targetX = platformTransform.transform.position.x - (platformWidth / 2) - (playerWidth / 2) - 0.1f;
                        spriteOffset = 90; // Rotate for left side
                        Debug.Log("Falling SpikyLeft");
                    }
                    else
                    {
                        // Snap to the right side
                        targetX = platformTransform.transform.position.x - (platformWidth / 2) - (playerWidth / 2) - 0.1f;
                        spriteOffset = 90; // Rotate for right side
                        Debug.Log("Falling SpikyRight");
                    }


                    playerRB.MovePosition(new Vector2(targetX, rb.position.y));

                    yield return new WaitForSeconds(0.2f); // Short cling effect before falling

                    playerRB.gravityScale = 0;

                    playerRB.velocity = falling;

                    SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                    yield return new WaitForSeconds(1);

                    top = false;
                    Debug.Log("Player is sliding down after snapping.");

                    yield return new WaitForSeconds(6);
                    Debug.Log("normal gravity");
                    if (onPlat != false)
                    {
                        playerRB.gravityScale = normalGravityScale;
                    }
                }
                break;
            default:
                // Handle any unexpected case
                Debug.LogWarning("Somethings not working");
                break;
        }
    }
}
