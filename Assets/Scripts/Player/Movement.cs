using System.Collections;
using System.Runtime.CompilerServices;
//using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Player
{
    public class Movement : MonoBehaviour
    {
        //Private Variables
        private Vector3 dragVector;
        private Vector3 initialTouchPos;
        private Vector3 lastPlayerPosition;
        private Vector2 launchDirection;
        private Vector2 endPosition;
        float angle;
        int reference;

        //Public Variables
        public float maxLaunchForce = 10f;
        public float speed = 2f;
        public float lineLengthMultiplier = 0.5f;
        private int jump = 0;
        //Public Objects
        public Rigidbody2D rb;
        public LineRenderer lineRenderer;
        public GameObject player;
        public BasePlatform basePlatform;

        //SerializeField variables
        [SerializeField] Transform movementPlatformTransform;
        [SerializeField] float normalGravityScale = 1f;
        [SerializeField] float addingGravityScale = 0.2f;

        [SerializeField] float tempGravityScale = 2f;
        [SerializeField] float horizontalJumpForce = 15;
        [SerializeField] float verticalJumpForce = -10;
        [SerializeField] Transform spriteTransform;
        [SerializeField] Animator animator;

        //Private Bools
        bool isDragging = false;

        bool jumpBool = false;
        bool isLaunched = false;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = normalGravityScale;

            lineRenderer.enabled = false;
            lineRenderer.positionCount = 2;

            gameObject.SetActive(true);
            basePlatform = null;
        }

        void Update()
        {

            // Stick the sprite to match the platform's side
            if (basePlatform != null)
            {
                spriteTransform.rotation = Quaternion.Euler(0, 0, basePlatform.SpriteOffset);
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
            if (rb.velocity.y < 0 && isLaunched && rb.velocity.y != -0.9f)
            {
                rb.gravityScale += addingGravityScale * Time.deltaTime;
            }
            if (isLaunched)
            {
                rb.mass += 0.2f * Time.deltaTime;
            }
            else
            {
                rb.mass = 1;
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
                // Use the same drag vector calculated during dragging
                float dragDistance = Mathf.Clamp(dragVector.magnitude, 0, maxLaunchForce);

                if(basePlatform != null)
                {
                    basePlatform.DetachPlayer();

                }
           
                
           
                // Apply the force for the launch
                rb.AddForce(launchDirection * (dragDistance * speed), ForceMode2D.Impulse);
                isLaunched = true;

                // Reset dragging state and disable LineRenderer
                lineRenderer.enabled = false;
                jump++; // Reset jump count for single jump
                jumpBool = true;
                rb.gravityScale = normalGravityScale;

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
    }
}
