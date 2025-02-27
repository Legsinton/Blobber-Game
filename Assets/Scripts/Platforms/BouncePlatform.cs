using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlatform : BasePlatform
{
    public override void Start()
    {
        base.Start();
    }
    protected override void HandleCollision(Collision2D collision)
    {
        if (playerRB != null)
        {           
            if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
            {
                if (!hasBounced)
                {
                    if (collisionNormal.x > 0.1f) // RightSide of the platform
                    {
                        isRight = true;
                        hasBounced = true;
                        playerRB.gravityScale = normalGravityScale;
                        Bounce(collision.relativeVelocity.x, -collision.relativeVelocity.y);
                        Invoke(nameof(ResetBounce), 0.5f);
                        Debug.Log("RightBounce");
                    }
                    else if (collisionNormal.x < -0.1f) // Leftside of the platform
                    {
                        hasBounced = true;
                        playerRB.gravityScale = normalGravityScale;
                        isLeft = true;
                        Bounce(collision.relativeVelocity.x, -collision.relativeVelocity.y);
                        Invoke(nameof(ResetBounce), 0.1f);
                        Debug.Log("LeftBounce");
                    }

                }
            }
            else if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
            {
                if (collisionNormal.y > 0.1f) // Top of the platform
                {
                    hasBounced = true;
                    BounceSmall(-collision.relativeVelocity.x, -collision.relativeVelocity.y);
                    Invoke(nameof(ResetBounce), 0.1f);

                    Debug.Log("TopBounce");
                }
                else if (collisionNormal.y < -0.1f) // Bottom of the platform
                {
                    hasBounced = true;
                    BounceSmall(-collision.relativeVelocity.x, -collision.relativeVelocity.y);
                    Invoke(nameof(ResetBounce), 0.1f);

                    Debug.Log("Bottombounce");
                }
            }
        }
    }
}
