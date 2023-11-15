using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleFallDown : MonoBehaviour
{
    public Ease ease;
    public float targetX;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.DOMoveY(targetX, 1.5f).SetEase(ease);
    }

    // Update is called once per frame
    void Update()
    {
       //this.transform.DOMoveY(targetX, 0.3f).SetEase(ease);
    }
}
