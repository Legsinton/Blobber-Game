using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Player
{
    public class Collision : MonoBehaviour
    {
        public Vector2 falling = new Vector2(0, 0.9f);
        [SerializeField] Rigidbody2D rb;
        [SerializeField] Movement moveScript;
        public ScoreController scoreController;
        protected int score = 1;
        private Transform platform;
        public Transform platformTransform;
        public bool onNormalPlatform;

        public bool OnNormalPlatform => onNormalPlatform;
        public Vector3 lastPlayerPosition;
        public Vector3 LastPlayerPosition => lastPlayerPosition;
        public Transform PlatformTransform => platformTransform;
        private HashSet<GameObject> scoredObjects;

        SpriteRenderer spriteRenderer;
        float platformWidth;
        float playerWidth;
        Vector2 collisionNormal;

        public Vector3 positionOffset;
        public bool isOnPlatform = false;
        public bool launched = false;
        public bool Launched => launched;
        public bool IsOnPlatform => isOnPlatform;
        Movement movement;

        void Start()
        {
            scoredObjects = new HashSet<GameObject>();
            rb = GetComponent<Rigidbody2D>();
            movement = FindAnyObjectByType<Movement>();
            if (scoreController == null)
            {
                scoreController = FindObjectOfType<ScoreController>();
            }
        }
        void FixedUpdate()
        {
            if (isOnPlatform && platformTransform != null)
            {
                // Keep the player stuck to the platform's position while maintaining the offset
                rb.position = (Vector3)platformTransform.position + (Vector3)positionOffset;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {

            // # # # # # # # # # # # # # # # # # # # # # # MOVING PLATFORM VERTICAL # # # # # # # # # # # # # # # # # # # # # # # # # # 
            if (collision.gameObject.CompareTag("MovingPlatformVertical") && isOnPlatform != true)
            {
                collisionNormal = collision.contacts[0].normal;


                if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y)) // Prioritize vertical hits
                {
                    if (collisionNormal.x > 0.4f) // Top of the platform
                    {
                        Debug.Log("Hit the rightSide of the platform!");
                        BounceSmall(collision.relativeVelocity.x, collision.relativeVelocity.y);
                        Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 2f);


                    }
                    else if (collisionNormal.x < -0.4f) // Bottom of the platform
                    {
                        Debug.Log("Hit the lefttSide of the platform!");
                        BounceSmall(collision.relativeVelocity.x, collision.relativeVelocity.y);
                        Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 2f);

                    }
                }

                else if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
                {

                    if (collisionNormal.y > 0.4f) // Top of the platform
                    {
                        Debug.Log("Hit the top side!");
                        Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 2f);
                        platformTransform = collision.transform; // Store the platform's transform
                        onNormalPlatform = true;
                        rb.gravityScale = 0;
                        AttachPlayer();
                        if (!scoredObjects.Contains(collision.gameObject))
                        {
                            scoredObjects.Add(collision.gameObject);
                            scoreController.AddScore(score);
                        }
                    }

                    else if (collisionNormal.y < -0.4f) // Bottom of the platform
                    {
                        rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(0, rb.velocity.y));
                        Debug.Log("Hit the bottom side!");
                        BounceSmall(collision.relativeVelocity.x, collision.relativeVelocity.y);
                    }
                }


            }
            // # # # # # # # # # # # # # # # # # # # # # # MOVING PLATFORM # # # # # # # # # # # # # # # # # # # # # # # # # # 

            if (collision.gameObject.CompareTag("MovingPlatform") && isOnPlatform != true)
            {
                collisionNormal = collision.contacts[0].normal;

                if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y)) // Prioritize vertical hits
                {

                    if (collisionNormal.x > 0.4f) // Top of the platform
                    {
                        Debug.Log("Hit the rightSide of the moving platform!");
                        platformTransform = collision.transform; // Store the platform's transform
                        isOnPlatform = true;
                        movement.IsAttached = true;

                        AttachPlayer();
                        Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 2f);
                        if (!scoredObjects.Contains(collision.gameObject))
                        {
                            scoredObjects.Add(collision.gameObject);
                            scoreController.AddScore(score);
                        }
                    }
                    else if (collisionNormal.x < -0.4f) // Bottom of the platform
                    {
                        movement.IsAttached = true;
                        Debug.Log("Hit the lefttSide of the moving platform!");
                        platformTransform = collision.transform; // Store the platform's transform
                        isOnPlatform = true;
                        AttachPlayer();
                        Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 2f);

                        if (!scoredObjects.Contains(collision.gameObject))
                        {
                            scoredObjects.Add(collision.gameObject);
                            scoreController.AddScore(score);
                        }
                    }
                }

                if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
                {
                    if (collisionNormal.y > 0.4f) // Top of the platform
                    {
                        playerWidth = rb.GetComponent<Collider2D>().bounds.size.x;
                        platformTransform = collision.transform;
                        platformWidth = platformTransform.GetComponent<Collider2D>().bounds.size.x;
                        rb.velocity = Vector2.zero;
                        movement.IsAttached = true;
                        StartCoroutine(SnapAndFallCoroutine(PlatformType.Moving));
                        Debug.Log("Moving Top");
                    }

                }
                else if (collisionNormal.y < -0.4f) // Bottom of the platform
                {
                    rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(0, rb.velocity.y));
                    Debug.Log("Hit the bottom side moving platform!");
                    BounceSmall(collision.relativeVelocity.x, collision.relativeVelocity.y);
                }
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // # # # # # # # # # # # # # # # # # # # # # # DEATH PLATFORM # # # # # # # # # # # # # # # # # # # # # # # # # # 

            if (collision.gameObject.CompareTag("Death"))
            {
                if (TryGetComponent<Movement>(out var player))
                {
                    scoredObjects.Clear();
                    player.Die();
                }
            }
            // # # # # # # # # # # # # # # # # # # # # # # SPIKES # # # # # # # # # # # # # # # # # # # # # # # # # # 

            if (collision.gameObject.CompareTag("Spikes"))
            {
                if (TryGetComponent<Movement>(out var player))
                {
                    scoredObjects.Clear();
                    player.InstantDeath();
                }
            }
        }
        private void AttachPlayer()
        {

            Debug.Log("Attached");
            SoundFXManager.Instance.PlaySoundFX(SoundType.Smack);
            positionOffset = rb.transform.position - platformTransform.position; // Calculate offset
            rb.gravityScale = 0f; // Disable gravity while on the platform
            rb.velocity = Vector2.zero; // Stop velocity to prevent sliding
            isOnPlatform = true;
        }

        public void DetachPlayer()
        {
            if (IsOnPlatform)
            {
                isOnPlatform = false;
                platformTransform = null;
               // rb.gravityScale = normalGravityScale; // Restore gravity
                positionOffset = Vector3.zero; // Reset offset
                Debug.LogWarning("Detach Exit");
            }
        }

        private IEnumerator SnapAndFallCoroutine(PlatformType platformType)
        {
            yield return new WaitForSeconds(0.1f); // Brief delay for stability

            switch (platformType)
            {
                case PlatformType.Moving:
                    if (platformTransform != null)
                    {
                        float targetX;
                        if (rb.transform.position.x < platformTransform.transform.position.x)
                        {
                            // Snap to the left side
                            targetX = platformTransform.transform.position.x - (platformWidth / 2) + (playerWidth / 2) - 0.1f;
                           // spriteOffset = -90; // Rotate for left side
                            Debug.Log("Falling MovingLeft");
                        }
                        else
                        {
                            // Snap to the right side
                            targetX = platformTransform.transform.position.x + (platformWidth / 2) + (playerWidth / 2) + 0.1f;
                           // spriteOffset = 90; // Rotate for right side
                            Debug.Log("Falling MovingRight");
                        }

                        rb.MovePosition(new Vector2(targetX, rb.position.y));

                        // Rotate the player to align with the side
                       // rb.transform.rotation = Quaternion.Euler(0, 0, spriteOffset);
                        yield return new WaitForSeconds(0.2f); // Short cling effect before falling
                    }
                    break;
                default:
                    // Handle any unexpected case
                    Debug.LogWarning("Somethings not working");
                    break;
            }

        }
        public void ResetScoredObjects()
        {
            scoredObjects.Clear();
        }
        public void Bounce(float collisionVelocityX, float CollisionVelocityY)
        {
            SoundFXManager.Instance.PlaySoundFX(SoundType.Boing);
            float bounceScale = 2f;
            float maxBounceforce = 8;
            float upBounceforce = Mathf.Abs(CollisionVelocityY) * bounceScale;
            upBounceforce = Mathf.Clamp(upBounceforce, 0f, maxBounceforce);
            rb.AddForce(new Vector2(collisionVelocityX * 1f, upBounceforce), ForceMode2D.Impulse);
        }
        public void BounceSmall(float collisionVelocityX, float collisionVelocityY)
        {
            SoundFXManager.Instance.PlaySoundFX(SoundType.Boing);
            float bounceScale = 2f;
            float maxBounceforce = 5;
            float upBounceforce = Mathf.Abs(collisionVelocityY) * bounceScale;
            upBounceforce = Mathf.Clamp(upBounceforce, 0f, maxBounceforce);

            rb.AddForce(new Vector2(collisionVelocityX + 2f, upBounceforce), ForceMode2D.Impulse);
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