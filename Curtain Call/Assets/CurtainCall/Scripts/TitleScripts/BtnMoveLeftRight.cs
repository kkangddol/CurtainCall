using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Configuration;

public class BtnMoveLeftRight : MonoBehaviour
{
    //Easing 함수 사용
    [SerializeField]
    private Ease ease;

    public float targetX;

    //속도
    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.DOMoveX(targetX ,duration).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
