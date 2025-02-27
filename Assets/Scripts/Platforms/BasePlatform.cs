using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
//using UnityEngine.WSA;

public class BasePlatform : MonoBehaviour
{
    protected bool hasBounced;
    protected bool top;
    protected bool breaking;
    protected float spriteOffset;
    protected Vector2 collisionNormal;
    protected Vector2 positionOffset;
    protected bool isOnPlatform;
    protected float normalGravityScale = 1.2f;
    protected bool onPlat = false;
    protected float platformWidth;
    protected float playerWidth;
    protected bool onStickyPlatform;
    protected Rigidbody2D playerRB;
    protected Vector2 falling = new Vector2(0, -0.9f);
    protected SpriteRenderer spriteRenderer;
    float sideForce = 0;
   // protected Rigidbody2D rb;
    protected HashSet<GameObject> scoredObjects;
    [SerializeField] protected Transform platformTransform;
    protected int point = 0;
    protected bool isLeft;
    protected bool isRight;

    public float SpriteOffset { get { return spriteOffset; } set { spriteOffset = value; } }
    public Movement movement;

    protected int score = 1;
    protected ScoreController scoreController;

    public virtual void Start()
    {
      //  rb = GetComponent<Rigidbody2D>();
        scoreController = FindAnyObjectByType<ScoreController>();
        scoredObjects = new HashSet<GameObject>();
        movement = FindAnyObjectByType<Movement>();
    }

    protected void FixedUpdate()
    {
        if (isOnPlatform)
        {
            playerRB.transform.position = (Vector3)platformTransform.position + (Vector3)positionOffset;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            playerRB = collision.gameObject.GetComponent<Rigidbody2D>();
            HandleTrigger(collision);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisionNormal = collision.contacts[0].normal;
        if (collision.transform.CompareTag("Player"))
        {
            playerRB = collision.gameObject.GetComponent<Rigidbody2D>();
            if (collision.gameObject.TryGetComponent(out Movement comp))
            {
                comp.basePlatform = this;
            }
            HandleCollision(collision);
            if (playerRB == null)
            {
                Debug.LogWarning("Player Rigidbody2D is null!");
            }
            if (!scoredObjects.Contains(collision.gameObject) && gameObject.CompareTag("MovingPlatform") || gameObject.CompareTag("MovingPlatformVertical") && !scoredObjects.Contains(collision.gameObject))
            {
                scoreController.AddScore(score);
                scoredObjects.Add(collision.gameObject);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //playerRB = collision.gameObject.GetComponent<Rigidbody2D>();
        HandleCollisionExit(collision);

        if (collision.gameObject.TryGetComponent(out Movement comp))
        {
            comp.basePlatform = null;
        }


    }

    protected virtual void HandleCollision(Collision2D collision)
    {
    }

    protected virtual void HandleCollisionExit(Collision2D collision)
    {

    }

    protected virtual void HandleTrigger(Collider2D collider)
    {

    }

    protected void Bounce(float collisionVelocityX, float collisionVelocityY)
    {
        SoundFXManager.Instance.PlaySoundFX(SoundType.Boing);
        float bounceScale = 2f;
        float maxBounceforce = 8;
        float upBounceforce = Mathf.Abs(collisionVelocityY) * bounceScale;
        upBounceforce = Mathf.Clamp(upBounceforce, 0f, maxBounceforce);
        if(collisionVelocityX < 0)
        {
            Debug.Log("Hälldowgsrb");
            sideForce = Mathf.Abs(collisionVelocityX) * bounceScale;
            sideForce = Mathf.Clamp(sideForce, 1f, maxBounceforce);

        }
        else
        {
            Debug.Log("else");
            sideForce = Mathf.Abs(-collisionVelocityX) * bounceScale;
            sideForce = Mathf.Clamp(sideForce, 1f, -maxBounceforce);
        }
        playerRB.AddForce(new Vector2(sideForce, upBounceforce), ForceMode2D.Impulse);
        if (isRight && sideForce == 0)
        {
            playerRB.AddForce(new Vector2(-8, upBounceforce), ForceMode2D.Impulse);
        }
        if (isLeft && sideForce == 0)
        {
            playerRB.AddForce(new Vector2(8, upBounceforce), ForceMode2D.Impulse);
        }
    }
    protected void BounceSmall(float collisionVelocityX, float collisionVelocityY)
    {
        SoundFXManager.Instance.PlaySoundFX(SoundType.Boing);
        float bounceScale = 2f;
        float maxBounceforce = 5;
        float upBounceforce = Mathf.Abs(collisionVelocityY) * bounceScale;
        upBounceforce = Mathf.Clamp(upBounceforce, 0f, maxBounceforce);
        playerRB.AddForce(new Vector2(collisionVelocityX + 2f, upBounceforce), ForceMode2D.Impulse);
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
     
        SoundFXManager.Instance.PlaySoundFX(SoundType.Smack);
        isOnPlatform = true;
        Debug.Log("isOnPlatform set to TRUE - Player attached to platform!");
        positionOffset = playerRB.transform.position - platformTransform.position; // Calculate offset
        playerRB.gravityScale = 0f; // Disable gravity while on the platform
        playerRB.velocity = Vector2.zero; // Stop velocity to prevent sliding
    }
    public void DetachPlayer()
    {
        Debug.LogWarning("Detach Exit");
        isOnPlatform = false;
        platformTransform = null;
        positionOffset = Vector2.zero; // Reset offset
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
                        targetX = platformTransform.transform.position.x + (platformWidth / 2) - (playerWidth / 2) - 1f;
                        
                        Debug.Log("Falling SnaperLeft");
                    }
                    else
                    {
                        // Snap to the right side
                        targetX = platformTransform.transform.position.x - (platformWidth / 2) + (playerWidth / 2) + 1f;
                        spriteOffset = -90;
                        Debug.Log("Falling SnaperRight");
                    }

                    Debug.Log(targetX);
                    playerRB.position = new Vector2(targetX, playerRB.position.y);

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
                        targetX = platformTransform.transform.position.x + (platformWidth / 2) - (playerWidth / 2) - 1f;
                        spriteOffset = 90; // Rotate for left side
                        Debug.Log("Falling MovingLeft");
                    }
                    else
                    {
                        // Snap to the right side
                        targetX = platformTransform.transform.position.x - (platformWidth / 2) + (playerWidth / 2) + 1f;
                        spriteOffset = -90; // Rotate for right side
                        Debug.Log("Falling MovingRight");
                    }

                    playerRB.position = new Vector2(targetX, playerRB.position.y);
                    AttachPlayer();

                    yield return new WaitForSeconds(0.2f); // Short cling effect before falling
                }
                break;

            case PlatformType.Spiky:
                if (platformTransform != null)
                {
                    float targetX;
                    if (playerRB.transform.position.x < platformTransform.transform.position.x)
                    {
                        Debug.Log("Hellose");
                        // Snap to the left side
                        spriteOffset = -90;
                        targetX = platformTransform.transform.position.x - (platformWidth / 2) + (playerWidth / 2) + 1f;

                        Debug.Log("Falling SnaperLeft");
                    }
                    else
                    {
                        Debug.Log("Hellose");
                        // Snap to the left side
                        spriteOffset = -90;
                        targetX = platformTransform.transform.position.x - (platformWidth / 2) + (playerWidth / 2) + 1f;

                        Debug.Log("Falling SnaperLeft");
                    }

                    Debug.Log(targetX);

                    playerRB.MovePosition(new Vector2(targetX, playerRB.position.y));

                    yield return new WaitForSeconds(0.2f); // Short cling effect before falling

                    // Apply gravity + downward force to start falling
                    playerRB.gravityScale = 0;
                    spriteOffset = -90;

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

            case PlatformType.SpikyFlip:
                if (platformTransform != null)
                {
                    float targetX;
                    if (playerRB.transform.position.x < platformTransform.transform.position.x)
                    {
                        Debug.Log("Hellose");
                        // Snap to the left side
                        spriteOffset = 90;
                        targetX = platformTransform.transform.position.x + (platformWidth / 2) - (playerWidth / 2) - 1f;
                        Debug.Log("Falling SnaperLeft");
                    }
                    else
                    {
                        Debug.Log("Hellose");
                        // Snap to the left side
                        spriteOffset = 90;
                        targetX = platformTransform.transform.position.x + (platformWidth / 2) - (playerWidth / 2) - 1f;
                        Debug.Log("Falling SnaperLeft");
                    }

                    Debug.Log(targetX);
                    playerRB.position = new Vector2(targetX, playerRB.position.y);

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
    public enum PlatformType
    {
        Sticky,
        Spiky,
        SpikyFlip,
        Moving
    }
}
