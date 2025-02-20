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
        [SerializeField] private float normalGravityScale = 1.2f;
        public ScoreController scoreController;
        protected int score = 1;
        bool breakPlat = false;
        private Transform platform;
        public Transform platformTransform;
        public bool onNormalPlatform;

        public bool OnNormalPlatform => onNormalPlatform;
        public Vector3 lastPlayerPosition;
        public Vector3 LastPlayerPosition => lastPlayerPosition;
        public Transform PlatformTransform => platformTransform;
        private HashSet<GameObject> scoredObjects;
        private float spriteOffset;

        SpriteRenderer spriteRenderer;
        float platformWidth;
        float playerWidth;
        Vector2 collisionNormal;
        bool top = false;
        bool onPlat = false;
        public float SpriteOffset { get { return spriteOffset; } set { spriteOffset = value; } }
        public Vector3 positionOffset;
        public bool isOnPlatform = false;
        public bool launched = false;
        public bool Launched => launched;
        public bool IsOnPlatform => isOnPlatform;
        private bool hasbounced = false;
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
            // # # # # # # # # # # # # # # # # # NORMAL PLATFORM # # # # # # # # # # # # # # # # # # # # # # # # # 
            if (collision.gameObject.CompareTag("Platform"))
            {
                collisionNormal = collision.contacts[0].normal;

                if (!movement.IsAttached)
                {

                    if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x)) // Prioritize vertical hits
                    {
                        if (collisionNormal.y > 0.4f) // Top of the platform
                        {
                            movement.IsAttached = true;
                            onNormalPlatform = true;
                            Debug.Log("Hit the top of the platform!");
                            rb.velocity = Vector2.zero;
                            SoundFXManager.Instance.PlaySoundFX(SoundType.Smack);
                            Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 2f);

                            if (!scoredObjects.Contains(collision.gameObject))
                            {
                                scoredObjects.Add(collision.gameObject);
                                scoreController.AddScore(score);
                            }

                        }
                        else if (collisionNormal.y < -0.4f) // Bottom of the platform
                        {
                            Debug.Log("Hit the bottom of the platform!");
                            BounceSmall(collision.relativeVelocity.x, collision.relativeVelocity.y);
                        }
                    }

                    else if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
                    {
                        if (collisionNormal.x > 0.4f) // right side of the platform
                        {
                            Debug.Log("Hit the Right side!");
                            Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 2f);
                            BounceSmall(collision.relativeVelocity.x, collision.relativeVelocity.y);

                        }
                        else if (collisionNormal.x < -0.4f) // left side of the platform
                        {
                            Debug.Log("Hit the Left side!");
                            BounceSmall(collision.relativeVelocity.x, collision.relativeVelocity.y);

                        }

                    }
                }

            }
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
                    if (!movement.IsAttached)
                    {
                        if (collisionNormal.x > 0.4f) // Top of the platform
                        {
                            Debug.Log("Hit the rightSide of the moving platform!");
                            platformTransform = collision.transform; // Store the platform's transform
                            isOnPlatform = true;
                            movement.IsAttached = true;

                            AttachPlayer();
                            Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 2f);
                            onPlat = true;
                            if (!scoredObjects.Contains(collision.gameObject))
                            {
                                scoredObjects.Add(collision.gameObject);
                                scoreController.AddScore(score);
                            }
                        }
                        else if (collisionNormal.x < -0.4f) // Bottom of the platform
                        {
                            movement.IsAttached = true;
                            onPlat = true;
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


                }

                if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
                {

                    if (collisionNormal.y > 0.4f) // Top of the platform
                    {
                        onPlat = true;
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
            // # # # # # # # # # # # # # # # # # # # # # # START PLATFORM # # # # # # # # # # # # # # # # # # # # # # # # # # 

            if (collision.gameObject.CompareTag("Start"))
            {
                collisionNormal = collision.contacts[0].normal;

                if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x)) // Prioritize vertical hits
                {
                    if (collisionNormal.y > 0.4f) // Top of the platform
                    {
                        onNormalPlatform = true;
                        Debug.Log("Start Hit the top of the platform!");
                        rb.velocity = Vector2.zero;
                        Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 2f);
                    }
                    else if (collisionNormal.y < -0.4f) // Bottom of the platform
                    {
                        Debug.Log("Start Hit the bottom of the platform!");
                    }
                }
            }

            // # # # # # # # # # # # # # # # # # # # # # # STICKY PLATFORM # # # # # # # # # # # # # # # # # # # # # # # # # # 

            if (collision.gameObject.CompareTag("Sticky"))
            {
                collisionNormal = collision.contacts[0].normal;
                if (!movement.IsAttached)
                {

                    if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
                    {
                        if (collisionNormal.x > 0.2f)
                        {

                            if (top != true)
                            {
                                movement.IsAttached = true;
                                launched = true;
                                rb.velocity = Vector2.zero;
                                rb.gravityScale = 0;
                                StartCoroutine(FallDown());

                                SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                                spriteOffset = 90;
                                Debug.Log("StickyRight");
                                onPlat = true;

                                if (!scoredObjects.Contains(collision.gameObject))
                                {
                                    scoredObjects.Add(collision.gameObject);
                                    scoreController.AddScore(score);
                                }

                            }
                        }
                        if (collisionNormal.x < -0.2f)
                        {
                            if (top != true)
                            {
                                movement.IsAttached = true;
                                launched = true;
                                rb.velocity = Vector2.zero;
                                rb.gravityScale = 0;
                                StartCoroutine(FallDown());
                                SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                                Debug.Log("StickyLeft");
                                spriteOffset = -90;
                                onPlat = true;
                                

                                if (!scoredObjects.Contains(collision.gameObject))
                                {
                                    scoredObjects.Add(collision.gameObject);
                                    scoreController.AddScore(score);
                                }

                            }
                        }


                    }
                    if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
                    {

                        if (collisionNormal.y > 0.2f)
                        {
                            if (top != true)
                            {
                                movement.IsAttached = true;
                                playerWidth = rb.GetComponent<Collider2D>().bounds.size.x;
                                platform = collision.transform;
                                rb.velocity = Vector2.zero;
                                platformWidth = platform.GetComponent<Collider2D>().bounds.size.x;
                                StartCoroutine(SnapAndFallCoroutine(PlatformType.Sticky));
                                
                                Debug.Log("StickyTop");

                            }
                        }

                    }

                    if (collisionNormal.y < -0.2f)
                    {
                        BounceSmall(collision.relativeVelocity.x, collision.relativeVelocity.y);
                        Debug.Log("StickyBottom");
                    }

                }
            }
            // # # # # # # # # # # # # # # # # # # # # # # BOUNCY PLATFORM # # # # # # # # # # # # # # # # # # # # # # # # # # 

           /* if (collision.gameObject.CompareTag("Bouncy"))
            {
                Vector2 collisionNormal = collision.contacts[0].normal;

                if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
                {
                    if (!hasbounced)
                    {
                        if (collisionNormal.x > 0.2f) // RightSide of the platform
                        {
                            hasbounced = true;
                            Bounce(collision.relativeVelocity.x, collision.relativeVelocity.y);
                            Invoke(nameof(ResetBounce), 0.1f);
                            Debug.Log("RightBounce");

                            if (!scoredObjects.Contains(collision.gameObject))
                            {
                                scoredObjects.Add(collision.gameObject);
                                scoreController.AddScore(score);
                            }
                        }
                        else if (collisionNormal.x < -0.2f) // Leftside of the platform
                        {
                            hasbounced = true;
                            Bounce(collision.relativeVelocity.x, collision.relativeVelocity.y);
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
                    if (collisionNormal.y > 0.2f) // Top of the platform
                    {
                        hasbounced = true;
                        BounceSmall(collision.relativeVelocity.x, collision.relativeVelocity.y);
                        Invoke(nameof(ResetBounce), 0.1f);

                        Debug.Log("TopBounce");
                    }
                    else if (collisionNormal.y < -0.2f) // Bottom of the platform
                    {
                        hasbounced = true;
                        BounceSmall(collision.relativeVelocity.x, collision.relativeVelocity.y);
                        Invoke(nameof(ResetBounce), 0.1f);

                        Debug.Log("Bottombounce");
                    }
                }
            }
            // # # # # # # # # # # # # # # # # # # # # # # SPIKY PLATFORM # # # # # # # # # # # # # # # # # # # # # # # # # # 


            if (collision.gameObject.CompareTag("StickySpike"))
            {
                collisionNormal = collision.contacts[0].normal;
                if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
                {
                    if (collisionNormal.x > 0.4f)
                    {
                        if (top != true)
                        {
                            movement.IsAttached = true;
                            launched = true;
                            rb.velocity = Vector2.zero;
                            rb.gravityScale = 0;
                            StartCoroutine(FallDown());
                            SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                            spriteOffset = 90;
                            Debug.Log("Sticky Spike Right");
                            onPlat = true;
                            
                            if (!scoredObjects.Contains(collision.gameObject))
                            {
                                scoredObjects.Add(collision.gameObject);
                                scoreController.AddScore(score);
                            }

                        }
                    }
                    if (collisionNormal.x < -0.4f)
                    {
                        if (top != true)
                        {
                            movement.IsAttached = true;
                            launched = true;
                            rb.velocity = Vector2.zero;
                            rb.gravityScale = 0;
                            StartCoroutine(FallDown());
                            SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                            Debug.Log("Sticky Spike Left");
                            spriteOffset = -90;
                            onPlat = true;
                            if (!scoredObjects.Contains(collision.gameObject))
                            {
                                scoredObjects.Add(collision.gameObject);
                                scoreController.AddScore(score);
                            }

                        }
                    }


                }
                if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
                {

                    if (collisionNormal.y > 0.4f)
                    {

                        if (top != true || !movement.IsAttached)
                        {
                            movement.IsAttached = true;
                            playerWidth = rb.GetComponent<Collider2D>().bounds.size.x;
                            platform = collision.transform;
                            rb.velocity = Vector2.zero;
                           
                            platformWidth = platform.GetComponent<Collider2D>().bounds.size.x;
                            StartCoroutine(SnapAndFallCoroutine(PlatformType.Spiky));
                            Debug.Log("Sticky Spike Top");

                        }
                    }

                }

                if (collisionNormal.y < -0.4f)
                {
                    BounceSmall(collision.relativeVelocity.x, collision.relativeVelocity.y);
                    Debug.Log("Sticky Spike Bottom");
                }

            }*/
            // # # # # # # # # # # # # # # # # # # # # # # SPIKY FLIPPED PLATFORM # # # # # # # # # # # # # # # # # # # # # # # # # # 


            if (collision.gameObject.CompareTag("StickySpikeFlip"))
            {
                collisionNormal = collision.contacts[0].normal;
                if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
                {

                    if (collisionNormal.x > 0.4f)
                    {
                        if (top != true)
                        {
                            movement.IsAttached = true;
                            launched = true;
                            rb.velocity = Vector2.zero;
                            rb.gravityScale = 0;
                            StartCoroutine(FallDown());

                            SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                            spriteOffset = 90;
                            Debug.Log("StickySpike Flip Right");
                            onPlat = true;
                            
                            if (!scoredObjects.Contains(collision.gameObject))
                            {
                                scoredObjects.Add(collision.gameObject);
                                scoreController.AddScore(score);
                            }

                        }
                    }
                    if (collisionNormal.x < -0.4f)
                    {
                        if (top != true)
                        {
                            movement.IsAttached = true;
                            launched = true;
                            rb.velocity = Vector2.zero;
                            rb.gravityScale = 0;
                            StartCoroutine(FallDown());
                            SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                            Debug.Log("StickySpike Flip Left");
                            spriteOffset = -90;
                            onPlat = true;

                            if (!scoredObjects.Contains(collision.gameObject))
                            {
                                scoredObjects.Add(collision.gameObject);
                                scoreController.AddScore(score);
                            }

                        }
                    }


                }
                if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
                {

                    if (collisionNormal.y > 0.4f)
                    {
                        if (top != true)
                        {
                            movement.IsAttached = true;
                            playerWidth = rb.GetComponent<Collider2D>().bounds.size.x;
                            platform = collision.transform;
                            rb.velocity = Vector2.zero;
                            platformWidth = platform.GetComponent<Collider2D>().bounds.size.x;
                            StartCoroutine(SnapAndFallCoroutine(PlatformType.SpikyFlip));
                            Debug.Log("StickySpike Flip Top");

                        }
                    }



                    if (collisionNormal.y < -0.4f)
                    {
                        BounceSmall(collision.relativeVelocity.x, collision.relativeVelocity.y);
                        Debug.Log("StickySpike Flip Bottom");


                    }
                }
            }
            // # # # # # # # # # # # # # # # # # # # # # # BREAKING PLATFORM # # # # # # # # # # # # # # # # # # # # # # # # # # 


            if (collision.gameObject.CompareTag("Breaking"))
            {
                collisionNormal = collision.contacts[0].normal;
                if (!movement.IsAttached)
                {

                    if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
                    {

                        if (collisionNormal.x > 0.4f)
                        {
                            if (top != true)
                            {
                                movement.IsAttached = true;
                                launched = true;
                                rb.velocity = Vector2.zero;
                                rb.gravityScale = 0;
                                breakPlat = true;
                                StartCoroutine(FallDown());
                                SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                                Debug.Log("BreakingRight");
                                spriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
                                spriteOffset = (rb.transform.position.x < collision.transform.position.x) ? 90 : -90; // Adjust as needed (e.g., 90 or -90 degrees)
                                onPlat = true;
                                if (!scoredObjects.Contains(collision.gameObject))
                                {
                                    scoredObjects.Add(collision.gameObject);
                                    scoreController.AddScore(score);
                                }
                            }
                        }
                        if (collisionNormal.x < -0.4f)
                        {
                            if (top != true)
                            {
                                movement.IsAttached = true;
                                launched = true;
                                rb.velocity = Vector2.zero;
                                rb.gravityScale = 0;
                                breakPlat = true;
                                StartCoroutine(FallDown());

                                spriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
                                SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                                Debug.Log("BreakingLeft");
                                spriteOffset = (rb.transform.position.x < collision.transform.position.x) ? 90 : -90; // Adjust as needed (e.g., 90 or -90 degrees)
                                onPlat = true;

                                if (!scoredObjects.Contains(collision.gameObject))
                                {
                                    scoredObjects.Add(collision.gameObject);
                                    scoreController.AddScore(score);
                                }

                            }
                        }


                    }
                    if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
                    {

                        if (collisionNormal.y > 0.4f)
                        {

                            movement.IsAttached = true;
                            playerWidth = rb.GetComponent<Collider2D>().bounds.size.x;
                            platform = collision.transform;
                            rb.velocity = Vector2.zero;
                            platformWidth = platform.GetComponent<Collider2D>().bounds.size.x;
                            breakPlat = true;
                            spriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
                            StartCoroutine(SnapAndFallCoroutine(PlatformType.Sticky));
                            Debug.Log("BreakingTop");
                        }
                    }

                    if (collisionNormal.y < -0.4f)
                    {
                        BounceSmall(collision.relativeVelocity.x, collision.relativeVelocity.y);
                        Debug.Log("BreakingBottom");
                    }

                }
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            // # # # # # # # # # # # # # # # # # # # # # # STICKY PLATFORM # # # # # # # # # # # # # # # # # # # # # # # # # # 
            if (collision.gameObject.CompareTag("Sticky"))
            {
                if (collision.contactCount >= 0)
                {
                    if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
                    {

                        if (collisionNormal.x > 0.4f)
                        {

                            Debug.Log("StickyLeavingRight");
                            //isStuckToStickyPlatform = false;
                            if (top != true)
                            {
                                launched = false;
                                //rb.gravityScale = normalGravityScale;
                                top = false;
                                onPlat = false;

                            }

                        }
                        if (collisionNormal.x < -0.4f)
                        {
                            if (top != true)
                            {


                                Debug.Log("StickyExitLeft");
                                //isStuckToStickyPlatform = false;
                                launched = false;
                               // rb.gravityScale = normalGravityScale;
                                top = false;
                                onPlat = false;

                            }

                        }


                    }
                    if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
                    {
                        if (collisionNormal.y > 0.4f)
                        {
                            onPlat = false;

                            Debug.Log("Sticky");
                            platform = null;
                            launched = false;
                            top = true;
                            onPlat = false;

                            //rb.gravityScale = stickyGravityScale;
                            if (!scoredObjects.Contains(collision.gameObject))
                            {
                                scoredObjects.Add(collision.gameObject);
                                scoreController.AddScore(score);
                            }
                        }
                    }
                }
            }
            // # # # # # # # # # # # # # # # # # # # # # # SPIKY PLATFORM # # # # # # # # # # # # # # # # # # # # # # # # # # 
            if (collision.gameObject.CompareTag("StickySpike") || collision.gameObject.CompareTag("StickySpikeFlip"))
            {
                if (collision.contactCount >= 0)
                {

                    if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
                    {
                        if (collisionNormal.x > 0.4f)
                        {
                            Debug.Log("SpickyLeavingRight");
                            //isStuckToStickyPlatform = false;
                            if (top != true)
                            {
                                launched = false;
                               // rb.gravityScale = normalGravityScale;
                                top = false;
                                onPlat = false;

                            }

                        }
                        if (collisionNormal.x < -0.4f)
                        {
                            onPlat = false;

                            if (top != true)
                            {
                                launched = false;
                                //rb.gravityScale = normalGravityScale;
                                top = false;
                            }
                        }
                    }
                    if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
                    {
                        if (collisionNormal.y > 0.4f)
                        {
                            onPlat = false;

                            Debug.Log("Sticky Spiky Top");
                            platform = null;
                            launched = false;
                            top = true;

                            //rb.gravityScale = stickyGravityScale;
                            if (!scoredObjects.Contains(collision.gameObject))
                            {
                                scoredObjects.Add(collision.gameObject);
                                scoreController.AddScore(score);
                            }
                        }
                    }


                }
            }
            // # # # # # # # # # # # # # # # # # # # # # # BREAKING PLATFORM # # # # # # # # # # # # # # # # # # # # # # # # # # 

            if (collision.gameObject.CompareTag("Breaking"))
            {
                if (collision.contactCount >= 0)
                {
                    if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
                    {

                        if (collisionNormal.x > 0.4f)
                        {
                            Debug.Log("BrekingExitRight");
                            launched = false;
                          //  rb.gravityScale = normalGravityScale;
                            top = false;
                            onPlat = false;
                            if (breakPlat)
                            {
                                StartCoroutine(FadeOut(0.7f));
                                Destroy(collision.gameObject, 0.8f);
                            }
                        }
                        if (collisionNormal.x < -0.4f)
                        {
                            Debug.Log("BrekingExitleft");
                            launched = false;
                           // rb.gravityScale = normalGravityScale;
                            top = false;
                            onPlat = false;

                            if (breakPlat)
                            {
                                StartCoroutine(FadeOut(0.7f));
                                Destroy(collision.gameObject, 0.8f);
                            }
                        }
                    }
                    if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
                    {
                        if (collisionNormal.y > 0.4f)
                        {
                            onPlat = false;

                            Debug.Log("BreakingExit");
                            platform = null;
                            launched = false;
                            top = true;

                            if (!scoredObjects.Contains(collision.gameObject))
                            {
                                scoredObjects.Add(collision.gameObject);
                                scoreController.AddScore(score);
                            }
                            if (breakPlat)
                            {
                                StartCoroutine(FadeOut(3));
                                Destroy(collision.gameObject, 3.2F);
                            }
                        }
                    }
                }
            }
            // # # # # # # # # # # # # # # # # # # # # # # NORMAIL PLATFORM # # # # # # # # # # # # # # # # # # # # # # # # # # 

            if (collision.gameObject.CompareTag("Platform"))
            {
                onNormalPlatform = false;
                isOnPlatform = false;

                DetachPlayer();
            }

            if (collision.gameObject.CompareTag("Start"))
            {

                onNormalPlatform = false;
                isOnPlatform = false;

                DetachPlayer();
            }
            Debug.Log("Exited collision with: " + collision.gameObject.name);
            /*if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                Debug.Log("Detached");
                movement.IsAttached = false;
                isOnPlatform = false;
                DetachPlayer(); 
            }
            if (transform.parent != null && collision.gameObject.CompareTag("MovingPlatformVertical"))
            {
                movement.IsAttached = false;

                Debug.LogWarning("Moving Exit");
                /*onNormalPlatform = false;
                platformTransform = null;
                isOnPlatform = false;
                launched = false;

                DetachPlayer();
            }*/
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
        private IEnumerator FadeOut(float duration)
        {
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                float startAlpha = color.a;
                float elapsedTime = 0f;

                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    color.a = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration);
                    spriteRenderer.color = color;
                    yield return null;
                }
                // Ensure it's fully opaque at the end
                color.a = 0f;
                spriteRenderer.color = color;
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
                Debug.LogWarning("Detach Exit");
                isOnPlatform = false;
                platformTransform = null;
               // rb.gravityScale = normalGravityScale; // Restore gravity
                positionOffset = Vector3.zero; // Reset offset

            }
        }

        private void ResetBounce()
        {
            hasbounced = false;
        }
        private IEnumerator SnapAndFallCoroutine(PlatformType platformType)
        {
            yield return new WaitForSeconds(0.1f); // Brief delay for stability

            switch (platformType)
            {
                case PlatformType.Sticky:
                    if (platform != null)
                    {
                        float targetX;
                        if (rb.transform.position.x < platform.transform.position.x)
                        {
                            // Snap to the left side
                            targetX = platform.transform.position.x - (platformWidth / 2) - (playerWidth / 2) - 0.1f;
                            spriteOffset = 90; // Rotate for left side
                            Debug.Log("Falling SnaperLeft");
                        }
                        else
                        {
                            // Snap to the right side
                            targetX = platform.transform.position.x + (platformWidth / 2) + (playerWidth / 2) + 0.1f;
                            spriteOffset = -90; // Rotate for right side
                            Debug.Log("Falling SnaperRight");
                        }


                        rb.MovePosition(new Vector2(targetX, rb.position.y));

                        // Rotate the player to align with the side
                        rb.transform.rotation = Quaternion.Euler(0, 0, spriteOffset);

                        yield return new WaitForSeconds(0.2f); // Short cling effect before falling

                        // Apply gravity + downward force to start falling
                        rb.gravityScale = 0;

                        rb.velocity = falling;


                        SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                        launched = true;
                        yield return new WaitForSeconds(1);

                        top = false;
                        Debug.Log("Player is sliding down after snapping.");

                        yield return new WaitForSeconds(6);
                        Debug.Log("normal gravity");
                        if (onPlat != true)
                        {
                            rb.gravityScale = normalGravityScale;

                        }
                    }
                    break;

                case PlatformType.Moving:
                    if (platformTransform != null)
                    {
                        float targetX;
                        if (rb.transform.position.x < platformTransform.transform.position.x)
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

                        rb.MovePosition(new Vector2(targetX, rb.position.y));

                        // Rotate the player to align with the side
                        rb.transform.rotation = Quaternion.Euler(0, 0, spriteOffset);
                        yield return new WaitForSeconds(0.2f); // Short cling effect before falling
                        AttachPlayer();
                    }
                    break;

                case PlatformType.Spiky:
                    if (platform != null)
                    {
                        float targetX;
                        if (rb.transform.position.x < platform.transform.position.x)
                        {
                            // Snap to the left side
                            targetX = platform.transform.position.x + (platformWidth / 2) + (playerWidth / 2) + 0.1f;
                            spriteOffset = -90; // Rotate for left side
                            Debug.Log("Falling SpikyLeft");
                        }
                        else
                        {
                            // Snap to the right side
                            targetX = platform.transform.position.x + (platformWidth / 2) + (playerWidth / 2) + 0.1f;
                            spriteOffset = -90; // Rotate for right side
                            Debug.Log("Falling SpikyRight");
                        }


                        rb.MovePosition(new Vector2(targetX, rb.position.y));

                        yield return new WaitForSeconds(0.2f); // Short cling effect before falling

                        // Apply gravity + downward force to start falling
                        rb.gravityScale = 0;

                        rb.velocity = falling;


                        SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                        launched = true;
                        yield return new WaitForSeconds(1);

                        top = false;
                        Debug.Log("Player is sliding down after snapping.");

                        yield return new WaitForSeconds(6);
                        Debug.Log("normal gravity");
                        if (onPlat != true)
                        {
                            rb.gravityScale = normalGravityScale;

                        }
                    }
                    break;

                case PlatformType.SpikyFlip:
                    if (platform != null)
                    {
                        float targetX;
                        if (rb.transform.position.x < platform.transform.position.x)
                        {
                            // Snap to the left side
                            targetX = platform.transform.position.x - (platformWidth / 2) - (playerWidth / 2) - 0.1f;
                            spriteOffset = 90; // Rotate for left side
                            Debug.Log("Falling SpikyLeft");
                        }
                        else
                        {
                            // Snap to the right side
                            targetX = platform.transform.position.x - (platformWidth / 2) - (playerWidth / 2) - 0.1f;
                            spriteOffset = 90; // Rotate for right side
                            Debug.Log("Falling SpikyRight");
                        }


                        rb.MovePosition(new Vector2(targetX, rb.position.y));

                        yield return new WaitForSeconds(0.2f); // Short cling effect before falling

                        rb.gravityScale = 0;

                        rb.velocity = falling;

                        SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                        launched = true;
                        yield return new WaitForSeconds(1);

                        top = false;
                        Debug.Log("Player is sliding down after snapping.");

                        yield return new WaitForSeconds(6);
                        Debug.Log("normal gravity");
                        if (onPlat != true)
                        {
                            rb.gravityScale = normalGravityScale;
                        }
                    }
                    break;
                default:
                    // Handle any unexpected case
                    Debug.LogWarning("Somethings not working");
                    break;
            }

        }
        private IEnumerator FallDown()
        {
            yield return new WaitForSeconds(0.1f); // Brief delay for stability

            rb.velocity = falling;
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