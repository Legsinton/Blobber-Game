using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class MovingSpikes : MonoBehaviour
{
    [SerializeField] private Transform spikes;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endingposition;

    [SerializeField] float speed = 5f;
    private void Start()
    {
        if (spikes == null || startPosition == null || endingposition == null)
        {

            return;
        }


        StartCoroutine(MovePlatform());
    }

    private Transform currentTargetTransform;
    private IEnumerator MovePlatform()
    {
        // Initialize the first target
        currentTargetTransform = startPosition;

        while (true)
        {
            // Move the platform towards the current target
            while (Vector3.Distance(spikes.position, currentTargetTransform.position) > 0.1f)
            {
                spikes.position = Vector3.MoveTowards(spikes.position, currentTargetTransform.position, speed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            // Switch to the other target once the platform reaches the current target
            currentTargetTransform = (currentTargetTransform == startPosition) ? endingposition : startPosition;
        }
    }
    private void OnDrawGizmos()
    {
        if (spikes != null && endingposition != null && startPosition != null)
        {
            Gizmos.DrawLine(spikes.transform.position, startPosition.position);
            Gizmos.DrawLine(spikes.transform.position, endingposition.position);
        }
    }

    public void ShiftPositions(Vector3 shiftAmount)
    {
        startPosition.position += shiftAmount;
        endingposition.position += shiftAmount;
        spikes.position += shiftAmount;
        ResetMovementPath();
    }

    private void ResetMovementPath()
    {
        // If the platform is closer to startPosition, target startPosition
        if (Vector3.Distance(spikes.position, startPosition.position) <
            Vector3.Distance(spikes.position, endingposition.position))
        {
            currentTargetTransform = startPosition;
        }
        else
        {
            currentTargetTransform = endingposition;
        }
    }


    private void OnDisable()
    {

        StopAllCoroutines();
    }

    private void OnDestroy()
    {

        StopAllCoroutines();
    }
}


