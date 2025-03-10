using System.Collections;
using Player;
using System.Collections.Generic;
//using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class MovingPlatformCollision : BasePlatform
{
    public override void Start()
    {
        base.Start();
    }
    protected override void HandleCollision(Collision2D collision)
    {
        base.HandleCollision(collision);

        if (playerRB != null && gameObject.CompareTag("MovingPlatform"))
        {
            if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y)) // Prioritize vertical hits
            {
                if (!isOnPlatform)
                {
                    if (collisionNormal.x > 0.2f) // Top of the platform
                    {

                        Debug.Log("Hit the rightSide of the platform!");
                        platformTransform = transform; // Store the platform's transform
                        playerRB.gravityScale = 0;
                        spriteOffset = 90;
                        AttachPlayer();
                        Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 2f);
                    }
                    else if (collisionNormal.x < -0.2f) // Bottom of the platform
                    {
                        Debug.Log("Hit the lefttSide of the platform!");
                        platformTransform = transform; // Store the platform's transform
                        spriteOffset = -90;
                        playerRB.gravityScale = 0;
                        AttachPlayer();
                        Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 2f);

                    }
                }
            }

            else if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
            {
                if (collisionNormal.y > 0.2f) // Top of the platform
                {
                    Debug.Log("Hit the bottom side!");
                    BounceSmall(-collision.relativeVelocity.x, -collision.relativeVelocity.y);
                    Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 2f);
                }

                else if (collisionNormal.y < -0.2f) // Bottom of the platform
                {

                    Debug.Log("Hit the top side!");
                    onPlat = true;
                    playerWidth = playerRB.GetComponent<Collider2D>().bounds.size.x;
                    platformTransform = transform;
                    platformWidth = platformTransform.GetComponent<Collider2D>().bounds.size.x;
                    playerRB.velocity = Vector2.zero;
                    StartCoroutine(SnapAndFallCoroutine(PlatformType.Moving));

                }
            }
        }

        if (playerRB != null && gameObject.CompareTag("MovingPlatformVertical"))
        {
            if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x)) // Prioritize vertical hits
            {
                if (!isOnPlatform)
                {
                    if (collisionNormal.y < -0.2f) // Top of the platform
                    {
                        Debug.Log("Hit the Top of the Moving platform!");
                        platformTransform = transform; // Store the platform's transform
                        playerRB.gravityScale = 0;
                        spriteOffset = 0;
                        AttachPlayer();
                        Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 2f);
                    }
                    else if (collisionNormal.y > 0.2f) // Bottom of the platform
                    {
                        playerRB.velocity = new Vector2(playerRB.velocity.x, Mathf.Max(0, playerRB.velocity.y));
                        Debug.Log("Hit the Bottom of the Moving Platform!");
                        BounceSmall(-collision.relativeVelocity.x, -collision.relativeVelocity.y);
                    }
                }
            }
            else if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
            {
                if (collisionNormal.x < -0.2f) // Top of the platform
                {
                    playerRB.velocity = new Vector2(playerRB.velocity.x, Mathf.Max(0, playerRB.velocity.y));
                    Debug.Log("Hit the Right side Moving!");
                    BounceSmall(-collision.relativeVelocity.x, -collision.relativeVelocity.y);
                }

                else if (collisionNormal.x > 0.2f) // Bottom of the platform
                {
                    playerRB.velocity = new Vector2(playerRB.velocity.x, Mathf.Max(0, playerRB.velocity.y));
                    Debug.Log("Hit the Left side Moving!");
                    BounceSmall(-collision.relativeVelocity.x, -collision.relativeVelocity.y);
                }

            }
        }
    }
}
