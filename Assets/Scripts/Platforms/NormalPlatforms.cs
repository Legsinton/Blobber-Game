using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalPlatforms : BasePlatform
{
    public override void Start()
    {
        base.Start();
    }

    protected override void HandleCollision(Collision2D collision)
    {
        base.HandleCollision(collision);
        if (playerRB != null && gameObject.CompareTag("Platform"))
        {
            if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
            {
                if (!hasBounced)
                {
                    if (collisionNormal.x > 0.1f) // RightSide of the platform
                    {
                        hasBounced = true;
                        Bounce(-collision.relativeVelocity.x, -collision.relativeVelocity.y);
                        Invoke(nameof(ResetBounce), 0.5f);
                        Debug.Log("Normal RightBounce");
                    }
                    else if (collisionNormal.x < -0.1f) // Leftside of the platform
                    {
                        hasBounced = true;
                        Bounce(-collision.relativeVelocity.x, -collision.relativeVelocity.y);
                        Invoke(nameof(ResetBounce), 0.1f);
                        Debug.Log("Normal LeftBounce");
                    }

                }
            }
            else if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
            {
                if (collisionNormal.y < -0.1f) // Top of the platform
                {
                    SoundFXManager.Instance.PlaySoundFX(SoundType.Smack);
                    playerRB.gravityScale = 0;
                    playerRB.velocity = Vector2.zero;
                    spriteOffset = 0;
                    Debug.Log("normalPlatform");
                }
                else if (collisionNormal.y > 0.1f) // Bottom of the platform
                {
                    hasBounced = true;
                    BounceSmall(-collision.relativeVelocity.x, -collision.relativeVelocity.y);
                    Invoke(nameof(ResetBounce), 0.1f);

                    Debug.Log("Normal Bottom Bounce");
                }
            }
        }
        if (playerRB != null && gameObject.CompareTag("Start"))
        {
            playerRB.gravityScale = 0;
            playerRB.velocity = Vector2.zero;
            Debug.Log("StartPlatform");
        }
    }

    protected override void HandleCollisionExit(Collision2D collision)
    {
        base.HandleCollisionExit(collision);
        //playerRB.gravityScale = 1.2f;
    }
}
