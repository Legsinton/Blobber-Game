using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyPlatform : BasePlatform
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
                    Debug.Log("Sticky Left");
                    onStickyPlatform = true;
                    SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                    playerRB.velocity = Vector2.zero;
                    playerRB.gravityScale = 0;
                    spriteOffset = 90;
                    StartCoroutine(FallDown());
                }
                else if (collisionNormal.x < -0.2f) // Right of the platform
                {
                    onStickyPlatform = true;
                    spriteOffset = -90;

                    Debug.Log("Sticky Right");
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
                    Debug.Log("Sticky Bottom");

                    BounceSmall(-collision.relativeVelocity.x, -collision.relativeVelocity.y);
                }
                else if (collisionNormal.y < -0.1f)
                {
                    Debug.Log("Sticky Top");

                    onPlat = true;
                    playerWidth = playerRB.GetComponent<Collider2D>().bounds.size.x;
                    platformTransform = transform;
                    platformWidth = platformTransform.GetComponent<Collider2D>().bounds.size.x;
                    playerRB.velocity = Vector2.zero;
                    StartCoroutine(SnapAndFallCoroutine(PlatformType.Sticky));
                }
            }
        }    
    }

    protected override void HandleCollisionExit(Collision2D collision)
    {
        if (playerRB != null)
        {
            Debug.Log("Sticky leave");
            onStickyPlatform = false;
            playerRB.gravityScale = 1.2f;
            // spriteOffset = 0;
            onPlat = false;
        }
    }
}
