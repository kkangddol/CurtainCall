using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;

public class Lever : Trigger
{
    [SerializeField]
    private Transform _stick;
    public override void ConnectImpl(ref IObservable<Unit> stream)
    {
        stream.Where(_ => Input.GetAxis("Horizontal1") > 0)
            .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
            .Subscribe(_ =>
            {
                progress += 0.1f;
            });

        stream.Where(_ => Input.GetAxis("Horizontal1") < 0)
            .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
            .Subscribe(_ =>
            {
                progress -= 0.1f;
            });
    }

    public override void OnProgressUpdate(float value)
    {
        if (_stick)
        {
            _stick.rotation = UnityEngine.Quaternion.AngleAxis(value * 90, -Vector3.forward);
        }
    }
}
