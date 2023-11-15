using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;



public abstract class Trigger : MonoBehaviour
{
    [SerializeField]
    private List<Reactant> _reactants = new List<Reactant>();
    private ReactiveProperty<float> _progress = new ReactiveProperty<float>();

    public float GetProgress() => _progress.Value;
    public void SetProgress(float value) => _progress.Value = value;

    public void Start()
    {
        _progress.Subscribe(value =>
        {
            foreach (var reactant in _reactants)
            {
                reactant.OnProgressUpdate(value);
            }
        });
    }
}
