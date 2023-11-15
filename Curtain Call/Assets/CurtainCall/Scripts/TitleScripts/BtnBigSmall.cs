using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BtnBigSmall : MonoBehaviour
{
    public Ease ease;

    public float scale;
    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.DOScale(scale, duration).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
