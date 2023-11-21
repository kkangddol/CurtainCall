using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ArchMove : Reactant
{
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform goalTransform;
    [SerializeField] private float duration;
    public override void OnProgressUpdate(float value)
    {
        if (value >= 1f)
        {
            if (!DOTween.IsTweening(transform))
            {
                transform.DOMoveY(goalTransform.position.y, duration);
            }

        }
        else if (value <= -1f)
        {
            if (!DOTween.IsTweening(transform))
            {
                transform.DOMoveY(startTransform.position.y, duration);
            }
        }

    }
}
