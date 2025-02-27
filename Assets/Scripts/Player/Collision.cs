using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Player
{
    public class DeathCollision : MonoBehaviour
    {
        public Animator animator;
        bool hasPlayed;
        public GameUI gameUI;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // # # # # # # # # # # # # # # # # # # # # # # DEATH PLATFORM # # # # # # # # # # # # # # # # # # # # # # # # # # 

            if (collision.gameObject.CompareTag("Death"))
            {
                if (TryGetComponent<Movement>(out var player))
                {
                    Debug.Log("Death Death");
                    StartCoroutine(Respawn(player));
                }
            }
            // # # # # # # # # # # # # # # # # # # # # # # # # SPIKES # # # # # # # # # # # # # # # # # # # # # # # # # # # # 

            if (collision.gameObject.CompareTag("Spikes"))
            {
                if (TryGetComponent<Movement>(out var player))
                {
                    Debug.Log("Death spikes");
                    StartCoroutine(InstantDeath(player));
                }
            }
        }
        private IEnumerator InstantDeath(Movement collision)
        {

            animator.SetTrigger("Death");
            SoundFXManager.Instance.PlaySoundFX(SoundType.Death);
            yield return new WaitForSeconds(0.3f);
            if (!hasPlayed)
            {  
                hasPlayed = true;
                collision.gameObject.SetActive(false);
                gameUI.GameOver();
            }
        }
        private IEnumerator Respawn(Movement collider)
        {
            animator.SetTrigger("Death");
            yield return new WaitForSeconds(1);
            collider.gameObject.SetActive(false);
            gameUI.GameOver();
        }
    }
}