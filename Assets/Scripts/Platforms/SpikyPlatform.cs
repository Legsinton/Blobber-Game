using Player;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
//using UnityEngine.WSA;

public class SpikyPlatform : BasePlatform
{
    public override void Start()
    {
        base.Start();
    }

    protected override void HandleCollision(Collision2D collision)
    {
        base.HandleCollision(collision);
        if (playerRB != null)
        {
            if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
            {
                if (collisionNormal.x > 0.2f) // Left of the platform
                {
                    Debug.Log("Spiky Left");
                    onStickyPlatform = true;
                    spriteOffset = 90;
                    SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                    playerRB.velocity = Vector2.zero;
                    playerRB.gravityScale = 0;
                    StartCoroutine(FallDown());
                }
                else if (collisionNormal.x < -0.2f) // Right of the platform
                {
                    onStickyPlatform = true;

                    Debug.Log("Spiky Right");
                    spriteOffset = -90;
                    SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                    playerRB.velocity = Vector2.zero;
                    playerRB.gravityScale = 0;
                    StartCoroutine(FallDown());
                }
            }
            if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
            {
                if (collisionNormal.y > 0.1f)
                {
                    Debug.Log("Spiky Bottom");

                    BounceSmall(-collision.relativeVelocity.x, -collision.relativeVelocity.y);
                }
                else if (collisionNormal.y < -0.1f && gameObject.CompareTag("StickySpike"))
                {
                    Debug.Log("Spiky Top");

                    onPlat = true;
                    playerWidth = playerRB.GetComponent<Collider2D>().bounds.size.x;
                    platformTransform = transform;
                    platformWidth = platformTransform.GetComponent<Collider2D>().bounds.size.x;
                    playerRB.velocity = Vector2.zero;
                    StartCoroutine(SnapAndFallCoroutine(PlatformType.Spiky));

                }
                else if (collisionNormal.y < -0.1f && gameObject.CompareTag("StickySpikeFlip"))
                {
                    Debug.Log("Spiky Top FLip");

                    onPlat = true;
                    playerWidth = playerRB.GetComponent<Collider2D>().bounds.size.x;
                    platformTransform = transform;
                    platformWidth = platformTransform.GetComponent<Collider2D>().bounds.size.x;
                    playerRB.velocity = Vector2.zero;
                    StartCoroutine(SnapAndFallCoroutine(PlatformType.SpikyFlip));

                }
            }
        }
    }

    protected override void HandleCollisionExit(Collision2D collision)
    {
        base.HandleCollisionExit(collision);
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
                       
                            // rb.gravityScale = normalGravityScale;
                            top = false;
                            onPlat = false;

                        

                    }
                    if (collisionNormal.x < -0.4f)
                    {
                        onPlat = false;

                       
                    }
                }
                if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
                {
                    if (collisionNormal.y > 0.4f)
                    {
                        onPlat = false;

                        Debug.Log("Sticky Spiky Top");
                        platformTransform = null;
                        top = true;
                    }
                }
            }
        }
    }
}
