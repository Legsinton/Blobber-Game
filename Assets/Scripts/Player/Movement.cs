using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Player
{
    public class Movement : MonoBehaviour
    {
        //Private Variables
       // private Vector3 initialMousePos;
        private Vector3 dragVector;
        private Vector3 initialTouchPos;
        private Vector3 lastPlayerPosition;
        private Vector2 launchDirection;
        private Vector2 endPosition;
        private GameUI gameUI;
        float angle;
        int reference;

        //Public Variables
        public float maxLaunchForce = 10f;
        public float speed = 2f;
        public float lineLengthMultiplier = 0.5f;
        private int jump = 0;
        bool jumpBool = false;
        //Public Objects
        public Rigidbody2D rb;
        public LineRenderer lineRenderer;
        public GameObject player;
        public Collision collisionGame;
        public BasePlatform basePlatform;

        //SerializeField variables
        [SerializeField] Transform MovementplatformTransform;
        [SerializeField] float normalGravityScale = 1f;
        [SerializeField] float addingGravityScale = 0.2f;

        [SerializeField] float tempGravityScale = 2f;
        [SerializeField] float horizontalJumpForce = 15;
        [SerializeField] float verticalJumpForce = -10;
        [SerializeField] Transform spriteTransform;
        bool isGrounded = false;
        public Transform SpriteTransform { get { return spriteTransform; } set { spriteTransform = value; } }

        [SerializeField] Animator animator;
        private bool is_Attached;
        public bool IsAttached { get { return is_Attached; } set { is_Attached = value; } }

        //Private Bools
        bool onNormalPlatform = false;
        bool hasPlayed = false;
        bool isDragging = false;
        bool onPlatform = false;
        [SerializeField] bool isLaunched = false;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = normalGravityScale;

            lineRenderer.enabled = false;
            lineRenderer.positionCount = 2;

            gameUI = FindObjectOfType<GameUI>();
            gameObject.SetActive(true);
            collisionGame = FindAnyObjectByType<Collision>();
            basePlatform = null;
        }

        void Update()
        {

            // Stick the sprite to match the platform's side
            if (basePlatform != null)
            {
                spriteTransform.rotation = Quaternion.Euler(0, 0, basePlatform.SpriteOffset);
                Debug.Log("spriteOffset " + basePlatform.SpriteOffset);
            }

            if (isLaunched)
            {
                angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                spriteTransform.rotation = Quaternion.Euler(0, 0, angle + -90);
            }

            Controls();
            Jump();
        }

        void FixedUpdate()
        {

           

            if (MovementplatformTransform != null)
            {
                lastPlayerPosition = MovementplatformTransform.position; // Update position every frame
            }
            if ((onPlatform || onNormalPlatform) && MovementplatformTransform != null && isDragging) // Only update if standing on a platform
            {
                //Vector3 platformMovement = platformTransform.position - lastPlayerPosition;

                lastPlayerPosition = MovementplatformTransform.position; // Update last position
            }
            if (rb.velocity.y < 0 && rb.velocity.y != -0.9f)
            {
                rb.gravityScale += addingGravityScale * Time.deltaTime;
            }
        }
        private void Controls()
        {
            // Start dragging
            if (Input.GetMouseButtonDown(0) && !isLaunched && !jumpBool)
            {
                // Initial touch position, multiplied to adjust the scale
                initialTouchPos = Camera.main.ScreenToViewportPoint(Input.mousePosition) * 20;
                initialTouchPos.z = 0f;

                // Enable LineRenderer to show the drag path
                lineRenderer.enabled = true;
                isDragging = true;
            }

            // Dragging logic
            if (Input.GetMouseButton(0) && !isLaunched && !jumpBool)
            {

                // Update the LineRenderer position to show the drag's start point at the player's current position
               lineRenderer.SetPosition(0, player.transform.position);
                // Calculate the drag vector based on the initial touch and current mouse position
                dragVector = initialTouchPos - Camera.main.ScreenToViewportPoint(Input.mousePosition) * 20;
                dragVector.z = 0f; // Keep it in 2D

                // Normalize the drag vector to get a direction
                launchDirection = dragVector.normalized;

                float dragDistance = Mathf.Clamp(dragVector.magnitude, 0, maxLaunchForce);
                endPosition = rb.position + launchDirection * dragDistance;

                lineRenderer.SetPosition(1, endPosition); // Visual representation of drag

            }

            // Launch on release
            if (Input.GetMouseButtonUp(0) && !isLaunched && !jumpBool && isDragging)
            {
               // basePlatform.DetachPlayer();
                // Use the same drag vector calculated during dragging
                float dragDistance = Mathf.Clamp(dragVector.magnitude, 0, maxLaunchForce);

                collisionGame.DetachPlayer();
                // Apply the force for the launch
                rb.AddForce(launchDirection * (dragDistance * speed), ForceMode2D.Impulse);
                isLaunched = true;

                // Reset dragging state and disable LineRenderer
                lineRenderer.enabled = false;
                jump++; // Reset jump count for single jump
                jumpBool = true;
                rb.gravityScale = normalGravityScale;
                onPlatform = false;
                onNormalPlatform = false;
                isGrounded = false;
                is_Attached = false;

                Invoke(nameof(StopDragging), 0.05f);
                animator.SetTrigger("JumpUp");
                SoundFXManager.Instance.PlaySoundFX(SoundType.Launch);
            }

            // Reset if velocity stops
            if (isLaunched && rb.velocity.magnitude == 0f)
            {
                animator.SetTrigger("JumpDown");
                isLaunched = false;
                jumpBool = false;
                jump = 0;
                isGrounded = true;
            }
        }
        private void Jump()
        {
            if (Input.GetMouseButtonUp(0) && isLaunched && !isDragging && jumpBool == true && jump == 1)
            {
                if (rb.velocity.x < 0)
                {
                    reference = -1;
                }
                else if (rb.velocity.x > 0)
                {
                    reference = 1;
                }
                jump++;
                rb.gravityScale = tempGravityScale;
                rb.velocity = new Vector2((horizontalJumpForce * reference), verticalJumpForce);
                jumpBool = false; ;
                SoundFXManager.Instance.PlaySoundFX(SoundType.Bonk);
            }
        }
        private void StopDragging()
        {
            isDragging = false;
        }
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("MovingPlatform"))
            {
                onPlatform = true;
                MovementplatformTransform = collision.transform; // Store the platform's transform
                lastPlayerPosition = MovementplatformTransform.position; // Initialize position
               // initialMousePos = transform.position;
            }
            if (collision.gameObject.CompareTag("MovingPlatformVertical"))
            {
                onPlatform = true;
                MovementplatformTransform = collision.transform; // Store the platform's transform
                lastPlayerPosition = MovementplatformTransform.position; // Initialize position
            }
        }

        public void Die()
        {
            Invoke(nameof(Respawn), 1f);
        }
        public void InstantDeath()
        {
            animator.SetTrigger("Death");
            if (!hasPlayed)
            {
                SoundFXManager.Instance.PlaySoundFX(SoundType.Death);
                hasPlayed = true;
            }
            Invoke(nameof(Respawn), 0.3f);
        }
        private void Respawn()
        {
            gameObject.SetActive(false);
            gameUI.GameOver();
        }
    }
}
