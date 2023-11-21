using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;

public class Lever : Trigger
{
    [SerializeField]
    private Transform _stick;
    public override void ConnectImpl(IObservable<Unit> stream)
    {
        stream.Where(_ => Input.GetAxis("Horizontal") > 0)
            .Subscribe(_ =>
            {
                progress += 0.1f;
            });

        stream.Where(_ => Input.GetAxis("Horizontal") < 0)
            .Subscribe(_ =>
            {
                progress -= 0.1f;
            });
    }

    public override void OnProgressUpdate(float value)
    {
        if (_stick)
        {
            _stick.Rotate(Vector3.forward, value * 180 * Mathf.Deg2Rad);
        }
    }
}
