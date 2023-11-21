using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;



[RequireComponent(typeof(BoxCollider2D))]
public abstract class Trigger : MonoBehaviour
{
    [SerializeField] private List<Reactant> _reactants = new List<Reactant>();
    private ReactiveProperty<float> _progress = new ReactiveProperty<float>();

    #region Progress
    public float minProgress;
    public float maxProgress;
    public float defaultProgress;

    public float progress
    {
        get
        {
            return _progress.Value;
        }
        set
        {
            SetProgress(value);
        }
    }
    private float GetProgress() => _progress.Value;
    private void SetProgress(float value)
    {

        if (value <= minProgress)
        {
            _progress.Value = minProgress;
        }
        else if (value >= maxProgress)
        {
            _progress.Value = maxProgress;
        }
        else
        {
            _progress.Value = value;
        }
    }
    #endregion
    public void Start()
    {
        _progress.Value = defaultProgress;
        _progress.Subscribe(value =>
        {
            OnProgressUpdate(value);
            foreach (var reactant in _reactants)
            {
                reactant.OnProgressUpdate(value);
            }
        });
    }

    public void Update()
    {
        Debug.Log(progress.ToString());
    }

    public virtual void ConnectImpl(ref IObservable<Unit> stream)
    {

    }//�÷��̾�� ���� �� �κ� ������ ������� �÷��̾ �ֵ��� Stream ��ü�� �ѱ�
    public void Connect(ref IObservable<Unit> stream) //�÷��̾�� ���� �� �κ� ������ ������� �÷��̾ �ֵ��� Stream ��ü�� �ѱ�
    {
        stream.Subscribe(_=>{},null, () =>
        {
            OnDisconnect();
            foreach (var reactant in _reactants)
            {
                reactant.OnDisconnect();
            }
        });//OnDisconnect

        OnConnect(); 
        foreach (var reactant in _reactants)
        {
            reactant.OnConnect();
        }//OnConnect

        ConnectImpl(ref stream);
       
    }

    public virtual void OnConnect() //Connect �ɶ� ȣ��
    {

    }
    public virtual void OnDisconnect() //Disonnect �ɶ� ȣ��
    {

    }
    public virtual void OnProgressUpdate(float value) // progress�� �ٲ� �� ȣ��  
    {

    }



   
}
