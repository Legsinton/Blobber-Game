using System.Collections;
using System.Collections.Generic;
//using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PointTrigger : BasePlatform
{
    public override void Start()
    {
        base.Start();
    }

    protected override void HandleTrigger(Collider2D collider)
    {

        base.HandleTrigger(collider);
        scoreController.AddScore(score);
        scoredObjects.Add(gameObject);
        gameObject.SetActive(false);
        Destroy(gameObject,0.5f);
    }
}
