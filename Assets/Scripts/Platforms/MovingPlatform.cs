using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform platform;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endingposition;

    [SerializeField] float speed = 5f;
    private void Start()
    {
        if (platform == null || startPosition == null || endingposition == null)
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
            while (Vector3.Distance(platform.position, currentTargetTransform.position) > 0.1f)
            {
                platform.position = Vector3.MoveTowards(platform.position, currentTargetTransform.position, speed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            // Switch to the other target once the platform reaches the current target
            currentTargetTransform = (currentTargetTransform == startPosition) ? endingposition : startPosition;
        }
    }
    private void OnDrawGizmos()
    {
        if (platform != null && endingposition != null && startPosition != null)
        {
            Gizmos.DrawLine(platform.transform.position, startPosition.position);
            Gizmos.DrawLine(platform.transform.position, endingposition.position);
        }
    }

    public void ShiftPositions(Vector3 shiftAmount)
    {
        startPosition.position += shiftAmount;
        endingposition.position += shiftAmount;
        platform.position += shiftAmount;
        ResetMovementPath();
    }

    private void ResetMovementPath()
    {
        // If the platform is closer to startPosition, target startPosition
        if (Vector3.Distance(platform.position, startPosition.position) <
            Vector3.Distance(platform.position, endingposition.position))
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


