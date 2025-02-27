using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.WSA;

public class BreakingPlatform : BasePlatform
{
    // Start is called before the first frame update
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
                    spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                    Debug.Log("Breaking Left");
                    breaking = true;
                    spriteOffset = 90;
                    SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                    playerRB.velocity = Vector2.zero;
                    playerRB.gravityScale = 0;
                    StartCoroutine(FallDown());
                }
                else if (collisionNormal.x < -0.2f) // Right of the platform
                {
                    spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                    Debug.Log("Breaking Right");
                    spriteOffset = -90;
                    breaking = true;
                    SoundFXManager.Instance.PlaySoundFX(SoundType.Splat);
                    playerRB.velocity = Vector2.zero;
                    playerRB.gravityScale = 0;

                    StartCoroutine(FallDown());
                }
            }
            if (Mathf.Abs(collisionNormal.y) > Mathf.Abs(collisionNormal.x))
            {
                if (collisionNormal.y > 0.2f)
                {
                    Debug.Log("Breaking Bottom");

                    BounceSmall(-collision.relativeVelocity.x, -collision.relativeVelocity.y);
                }
                else if (collisionNormal.y < -0.2f)
                {
                    Debug.Log("Breaking Top");

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
            if (gameObject.CompareTag("Breaking"))
            {
                if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
                {
                    if (collisionNormal.x > 0.1f)
                    {
                        Debug.Log("BrekingExitRight");
                        //  rb.gravityScale = normalGravityScale;
                       
                        if (breaking)
                        {
                            StartCoroutine(FadeOut(0.7f));
                            Destroy(gameObject, 0.8f);
                        }
                    }
                    if (collisionNormal.x < -0.1f)
                    {
                        Debug.Log("BrekingExitleft");
                        // rb.gravityScale = normalGravityScale;
                        onPlat = false;
                        top = false;
                        
                        if (breaking)
                        {
                            StartCoroutine(FadeOut(0.7f));
                            Destroy(gameObject, 0.8f);
                        }
                    }
                }
            }
        }
    }

    IEnumerator FadeOut(float duration)
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
}
