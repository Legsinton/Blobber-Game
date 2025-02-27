using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private Vector3 Offset = new Vector3(0, 0, -10f); // Z offset for 2D
    private readonly float smoothTime = 0.2f;
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private float treashold = 70;

    [SerializeField] private Transform target;
    [SerializeField] private Transform deathPlatform;
    [SerializeField] private Vector2 deathPlatformOffset = new Vector2(0, -10f);
    [SerializeField] private float minY = 6.1f; // Minimum Y position for the camera
    [SerializeField] private float maxY = 2000f;  // Maximum Y position for the camera
    private float highestPointReached;
    [SerializeField] private Camera cameraPosition;
    [SerializeField] PlatformSpawner platformSpawner;
    [SerializeField] bool disablleScreenMove = false;
   

    private void Start()
    {
        platformSpawner = FindAnyObjectByType<PlatformSpawner>();

        // Target position including offset
        Vector3 targetPosition = target.position + Offset;

        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        highestPointReached = minY;

        // Smoothly move the camera from its current position to the target position
        transform.position = targetPosition;
    }

    private void LateUpdate() // Use LateUpdate for smoother movement
    {
        if (target != null)
        {

            if (target.position.y > highestPointReached)
            {
                highestPointReached = target.position.y;
            }

            if (highestPointReached > minY)
            {
                minY = highestPointReached;
            }

            // Target position including offset
            Vector3 targetPosition = target.position + Offset;

            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

            // Smoothly move the camera from its current position to the target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            Vector3 deathPlatformPosition = new Vector3(cameraPosition.transform.position.x, cameraPosition.transform.position.y - deathPlatformOffset.y);
            deathPlatform.transform.position = deathPlatformPosition;

            if (target.position.y > treashold && !disablleScreenMove)
            {
                PerformInvisibleReset(100f); // Shift everything down by 70 units
            }
        }
    }
    private void PerformInvisibleReset(float shiftOffset)
    {
        // Shift the player down
        target.position -= new Vector3(0, shiftOffset, 0);

        // Shift the camera down
        cameraPosition.transform.position -= new Vector3(0, shiftOffset, 0);

        // Notify the platform spawner to shift platforms
        platformSpawner.DespawnPlatforms(shiftOffset);

        // Update the tracking variables
        minY -= shiftOffset;
        highestPointReached -= shiftOffset;

    }
}
